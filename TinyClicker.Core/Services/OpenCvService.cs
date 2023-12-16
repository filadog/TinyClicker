using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ImageMagick;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using TinyClicker.Core.Extensions;
using TinyClicker.Core.Logic;
using Point = OpenCvSharp.Point;

namespace TinyClicker.Core.Services;

public class OpenCvService : IOpenCvService
{
    private const double OPEN_CV_THRESHOLD_LOW = 0.78;
    private const double OPEN_CV_THRESHOLD_HIGH = 0.9;
    private const string SAMPLES_PATH = "./Samples/samples.dat";
    private const string SAMPLE_NAMES_PATH = "./Samples/button_names.txt";

    private readonly IWindowsApiService _windowsApiService;
    private readonly IImageToTextService _imageService;

    private readonly HashSet<string> _skipButtons =
    [
        GameButton.GameIcon.GetDescription(),
        GameButton.BalanceCoin.GetDescription(),
        GameButton.Restock.GetDescription(),
        GameButton.MenuButton.GetDescription(),
        GameButton.CalendarButton.GetDescription(),
        GameButton.TasksButton.GetDescription(),
        GameButton.FreeBuxGift.GetDescription(),
        GameButton.TowerManagementButton.GetDescription(),
        GameWindow.DeliverBitizensQuestPrompt.GetDescription(),
        GameWindow.FindBitizensQuestPrompt.GetDescription(),
        GameWindow.BuildNewFloorNotification.GetDescription(),
        GameWindow.HurryConstructionWithBux.GetDescription(),
        GameWindow.NewFloorNoCoinsNotification.GetDescription(),
        GameWindow.WatchBuxAdsPrompt.GetDescription(),
        GameWindow.WatchCoinsAdsPrompt.GetDescription(),
        GameWindow.FullyStockedBonus.GetDescription(),
        GameWindow.AdsLostRewardNotification.GetDescription(),
        GameWindow.BitizenMovedIn.GetDescription()
    ];

    private readonly HashSet<string> _highThresholdButtons =
    [
        GameButton.Gift.GetDescription()
    ];

    private readonly HashSet<string> _adjustableButtons =
    [
        GameButton.ParachuteGift.GetDescription()
    ];

    public OpenCvService(IWindowsApiService windowsApiService, IImageToTextService imageService)
    {
        _windowsApiService = windowsApiService;
        _imageService = imageService;
    }

    private Dictionary<string, Mat>? Templates { get; set; }

    public bool TryFindFirstImageOnScreen(Image gameScreen, out (string ItemName, int Location) result)
    {
        Templates ??= MakeTemplatesFromSamples(gameScreen);

        using var screenBitmap = new Bitmap(gameScreen);
        using var screenMat = screenBitmap.ToMat();

        foreach (var template in Templates.Where(x => !_skipButtons.Contains(x.Key)))
        {
            if (TryFindSingle(template, screenMat, out var scanResult))
            {
                Task.Delay(15).Wait(); // Smooth the CPU peak load
                result = (scanResult.Key, scanResult.Location);
                return true;
            }
        }

        result = default;
        return false;
    }

    public bool IsImageOnScreen(Enum image, Dictionary<string, Mat>? templates = null, Image? screenshot = null)
    {
        using var gameWindow = screenshot ?? _windowsApiService.GetGameScreenshot();
        using var windowBitmap = new Bitmap(gameWindow);

        Templates ??= MakeTemplatesFromSamples(gameWindow);

        var screen = windowBitmap.ToMat();
        var template = templates == null ? Templates[image.GetDescription()] : templates[image.GetDescription()];

        var result = FindTemplateOnImage(screen, template);
        var threshold = _highThresholdButtons.Contains(image.GetDescription())
            ? OPEN_CV_THRESHOLD_HIGH
            : OPEN_CV_THRESHOLD_LOW;

        return result.MaxVal >= threshold;
    }

    public bool TryFindOnScreen(Enum image, out Point location)
    {
        using var gameWindow = _windowsApiService.GetGameScreenshot();
        using var windowBitmap = new Bitmap(gameWindow);

        Templates ??= MakeTemplatesFromSamples(gameWindow);

        var screen = windowBitmap.ToMat();
        var template = Templates[image.GetDescription()];

        var result = FindTemplateOnImage(screen, template);
        location = result.MaxLoc;

        var threshold = _highThresholdButtons.Contains(image.GetDescription())
            ? OPEN_CV_THRESHOLD_HIGH
            : OPEN_CV_THRESHOLD_LOW;

        return result.MaxVal >= threshold;
    }

    public Dictionary<string, Mat> MakeTemplatesFromSamples(Image screenshot)
    {
        var images = LoadSampleImages();
        var percentage = _imageService.GetScreenDiffPercentageForTemplates(screenshot);

        var mats = new Dictionary<string, Mat>();

        foreach (var image in images)
        {
            using var imageOld = new MagickImage(_imageService.ImageToBytes(image.Value), MagickFormat.Png);
            imageOld.Resize(percentage.x, percentage.y);

            using var imageBitmap = new Bitmap(_imageService.BytesToImage(imageOld.ToByteArray()));
            var template = imageBitmap.ToMat();

            mats.Add(image.Key, template);
        }

        images.Clear();
        return mats;
    }

    private bool TryFindSingle(KeyValuePair<string, Mat> template, Mat reference, out (string Key, int Location) result)
    {
        var scanResult = FindTemplateOnImage(reference, template.Value);
        var threshold = _highThresholdButtons.Contains(template.Key)
            ? OPEN_CV_THRESHOLD_HIGH
            : OPEN_CV_THRESHOLD_LOW;

        if (scanResult.MaxVal < threshold)
        {
            result = default;
            return false;
        }

        result = _adjustableButtons.Contains(template.Key)
            ? (template.Key, MakeAdjustedLParam(scanResult.MaxLoc.X, scanResult.MaxLoc.Y))
            : (template.Key, _windowsApiService.MakeLParam(scanResult.MaxLoc.X, scanResult.MaxLoc.Y + 10));

        return true;
    }

    private int MakeAdjustedLParam(int x, int y)
    {
        return _windowsApiService.MakeLParam(x + 40, y + 40);
    }

    private static (double MaxVal, Point MaxLoc) FindTemplateOnImage(Mat screen, Mat template)
    {
        using var result = new Mat(screen.Rows - template.Rows + 1, screen.Cols - template.Cols + 1, MatType.CV_16S);
        using var matReference = screen.CvtColor(ColorConversionCodes.BGR2GRAY);
        using var matTemplate = template.CvtColor(ColorConversionCodes.BGR2GRAY);

        if (matReference.Height < matTemplate.Height || matReference.Width < matTemplate.Width)
        {
            return (0, new Point());
        }

        Cv2.MatchTemplate(matReference, matTemplate, result, TemplateMatchModes.CCoeffNormed);
        Cv2.Threshold(result, result, 0.7, 1.0, ThresholdTypes.Tozero);
        Cv2.MinMaxLoc(result, out _, out var maxVal, out _, out var maxLoc);

        return (maxVal, maxLoc);
    }

    private static byte[][] LoadSampleData()
    {
        var samples = Array.Empty<byte[]>();
        using var file = new FileStream(SAMPLES_PATH, FileMode.Open, FileAccess.Read);

        const int sizeInt = sizeof(int);
        var sizeRow = new byte[sizeInt];

        while (true)
        {
            var countRead = file.Read(sizeRow, 0, sizeInt);
            if (countRead != sizeInt)
            {
                break;
            }

            var countItems = BitConverter.ToInt32(sizeRow, 0);
            var data = new byte[countItems];
            countRead = file.Read(data, 0, countItems);

            if (countRead != countItems)
            {
                break;
            }

            Array.Resize(ref samples, samples.Length + 1);
            samples[^1] = data;
        }

        return samples;
    }

    private static Dictionary<string, Image> LoadSampleImages()
    {
        var samples = new Dictionary<string, Image>();
        var sampleNames = File.ReadAllLines(SAMPLE_NAMES_PATH);
        var sampleData = LoadSampleData();

        for (var i = 0; i < sampleNames.Length; i++)
        {
            var sample = sampleData[i];
            using var ms = new MemoryStream(sample);
            Image.FromStream(ms);
            samples.Add(sampleNames[i], Image.FromStream(ms));
        }

        return samples;
    }
}
