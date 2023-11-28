using ImageMagick;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using TinyClicker.Core.Logic;
using Point = OpenCvSharp.Point;

namespace TinyClicker.Core.Services;

public class OpenCvService : IOpenCvService
{
    private readonly IWindowsApiService _windowsApiService;
    private readonly IImageToTextService _imageService;

    private const double OPEN_CV_THRESHOLD = 0.78;
    private const string SAMPLES_PATH = "./Samples/samples.dat";
    private const string SAMPLE_NAMES_PATH = "./Samples/button_names.txt";


    public OpenCvService(IWindowsApiService windowsApiService, IImageToTextService imageService)
    {
        _windowsApiService = windowsApiService;
        _imageService = imageService;
    }

    private Dictionary<string, Mat> Templates { get; set; } = new();

    private readonly HashSet<string> _skipButtons = new()
    {
        Button.GameIcon.GetDescription(),
        Button.BalanceCoin.GetDescription(),
        Button.RestockButton.GetDescription(),
        Button.MenuButton.GetDescription(),
        Button.CalendarButton.GetDescription(),
        Button.TasksButton.GetDescription(),
        Button.FreeBuxGiftButton.GetDescription(),
        GameWindow.DeliverBitizens.GetDescription(),
        GameWindow.FindBitizens.GetDescription(),
        GameWindow.BuildNewFloorNotification.GetDescription(),
        GameWindow.HurryConstruction.GetDescription(),
        GameWindow.NewFloorNoCoinsNotification.GetDescription(),
        GameWindow.WatchAdPromptBux.GetDescription(),
        GameWindow.WatchAdPromptCoins.GetDescription(),
        GameWindow.FullyStockedBonus.GetDescription(),
        GameWindow.AdsLostReward.GetDescription(),
        GameWindow.BitizenMovedIn.GetDescription()
    };

    private readonly HashSet<string> _adjustableButtons = new()
    {
        Button.GiftChute.GetDescription()
    };

    public Dictionary<string, int> TryFindFirstOnScreen(Image gameScreen)
    {
        if (Templates.Count == 0)
        {
            Templates = MakeTemplatesFromSamples(gameScreen);
        }

        using var screenBitmap = new Bitmap(gameScreen);
        using var screenMat = screenBitmap.ToMat();

        var result = new Dictionary<string, int>();
        foreach (var template in Templates)
        {
            if (_skipButtons.Contains(template.Key))
            {
                continue;
            }

            var item = TryFindSingle(template, screenMat);
            Task.Delay(15).Wait(); // Smooth the CPU peak load

            if (item == default)
            {
                continue;
            }

            result.Add(item.Key, item.Location);
            break;
        }

        return result;
    }

    private (string Key, int Location) TryFindSingle(KeyValuePair<string, Mat> template, Mat reference)
    {
        var result = FindTemplateOnImage(reference, template.Value);

        if (result.MaxVal < OPEN_CV_THRESHOLD)
        {
            return default;
        }

        if (_adjustableButtons.Contains(template.Key))
        {
            return (template.Key, MakeAdjustedLParam(result.MaxLoc.X, result.MaxLoc.Y));
        }

        return (template.Key, _windowsApiService.MakeLParam(result.MaxLoc.X, result.MaxLoc.Y + 10));
    }

    private int MakeAdjustedLParam(int x, int y)
    {
        return _windowsApiService.MakeLParam(x + 40, y + 40);
    }

    public bool IsImageOnScreen(Enum image, Dictionary<string, Mat>? templates = null, Image? screenshot = null)
    {
        using var gameWindow = screenshot ?? _windowsApiService.GetGameScreenshot();
        using var windowBitmap = new Bitmap(gameWindow);

        var screen = windowBitmap.ToMat();
        var template = templates == null ? Templates[image.GetDescription()] : templates[image.GetDescription()];

        var result = FindTemplateOnImage(screen, template);

        return result.MaxVal >= OPEN_CV_THRESHOLD;
    }

    public bool TryFindOnScreen(Enum image, out Point location)
    {
        using var gameWindow = _windowsApiService.GetGameScreenshot();
        using var windowBitmap = new Bitmap(gameWindow);

        var screen = windowBitmap.ToMat();
        var template = Templates[image.GetDescription()];

        var result = FindTemplateOnImage(screen, template);
        location = result.MaxLoc;

        return result.MaxVal >= OPEN_CV_THRESHOLD;
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

        var sizeInt = sizeof(int);
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
        var dict = new Dictionary<string, Image>();
        var sampleNames = File.ReadAllLines(SAMPLE_NAMES_PATH);
        var sampleData = LoadSampleData();

        for (int i = 0; i < sampleNames.Length; i++)
        {
            var sample = sampleData[i];
            using var ms = new MemoryStream(sample);
            Image.FromStream(ms);
            dict.Add(sampleNames[i], Image.FromStream(ms));
        }

        return dict;
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
}