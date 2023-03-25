using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using OpenCvSharp.Extensions;
using TinyClicker.Core.Logic;
using ImageMagick;
using System.IO;
using Point = OpenCvSharp.Point;

namespace TinyClicker.Core.Services;

public class OpenCvService : IOpenCvService
{
    private readonly IWindowsApiService _windowsApiService;
    private readonly IImageService _imageService;

    private const double OPEN_CV_THRESHOLD = 0.78;
    private const string SAMPLES_PATH = "./Samples/samples.dat";
    private const string BUTTON_NAMES_PATH = "./Samples/button_names.txt";


    public OpenCvService(IWindowsApiService windowsApiService, IImageService imageService)
    {
        _windowsApiService = windowsApiService;
        _imageService = imageService;
    }

    private Dictionary<string, Mat> Templates { get; set; } = new();

    private readonly HashSet<string> _skipButtons = new()
    {
        Button.GameIcon.GetName(),
        Button.BalanceCoin.GetName(),
        Button.RestockButton.GetName(),
        Button.MenuButton.GetName(),
        GameWindow.DeliverBitizens.GetName(),
        GameWindow.FindBitizens.GetName(),
        GameWindow.BuildNewFloorNotification.GetName(),
        GameWindow.HurryConstruction.GetName(),
        GameWindow.NewFloorNoCoinsNotification.GetName(),
        GameWindow.WatchAdPromptBux.GetName(),
        GameWindow.WatchAdPromptCoins.GetName(),
        GameWindow.FullyStockedBonus.GetName(),
        GameWindow.AdsLostReward.GetName()
    };

    public Dictionary<string, int> TryFindFirstOnScreen(Image gameScreen)
    {
        if (!Templates.Any())
        {
            Templates = MakeTemplates(gameScreen);
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
            if (string.IsNullOrEmpty(item.Key))
            {
                Task.Delay(15).Wait(); // Smooth the CPU peak load
                continue;
            }

            result.Add(item.Key, item.Location);
            break;
        }

        return result;
    }

    public (string Key, int Location) TryFindSingle(KeyValuePair<string, Mat> template, Mat reference)
    {
        var result = FindTemplateOnImage(reference, template.Value);

        if (result.MaxVal >= OPEN_CV_THRESHOLD)
        {
            if (_adjustableButtons.Contains(template.Key))
            {
                return (template.Key, MakeAdjustedLParam(result.MaxLoc.X, result.MaxLoc.Y));
            }

            return (template.Key, _windowsApiService.MakeLParam(result.MaxLoc.X, result.MaxLoc.Y + 10));
        }

        return ("", 0);
    }

    private readonly HashSet<string> _adjustableButtons = new()
    {
        Button.GiftChute.GetName()
    };

    private int MakeAdjustedLParam(int x, int y)
    {
        return _windowsApiService.MakeLParam(x + 40, y + 40);
    }

    public bool FindOnScreen(Enum image, Dictionary<string, Mat>? templates = null, Image? screenshot = null)
    {
        using var gameWindow = screenshot ?? _windowsApiService.GetGameScreenshot();
        using var windowBitmap = new Bitmap(gameWindow);

        var screen = windowBitmap.ToMat();
        var template = templates == null ? Templates[image.GetName()] : templates[image.GetName()];

        var result = FindTemplateOnImage(screen, template);

        return result.MaxVal >= OPEN_CV_THRESHOLD;
    }

    public bool FindOnScreen(Enum image, out Point location)
    {
        using var gameWindow = _windowsApiService.GetGameScreenshot();
        using var windowBitmap = new Bitmap(gameWindow);

        var screen = windowBitmap.ToMat();
        var template = Templates[image.GetName()];

        var result = FindTemplateOnImage(screen, template);
        location = result.MaxLoc;

        return result.MaxVal >= OPEN_CV_THRESHOLD;
    }

    private static (double MaxVal, Point MaxLoc) FindTemplateOnImage(Mat screen, Mat template)
    {
        using var result = new Mat(screen.Rows - template.Rows + 1, screen.Cols - template.Cols + 1, MatType.CV_16S);
        using var matReference = screen.CvtColor(ColorConversionCodes.BGR2GRAY);
        using var matTemplate = template.CvtColor(ColorConversionCodes.BGR2GRAY);

        Cv2.MatchTemplate(matReference, matTemplate, result, TemplateMatchModes.CCoeffNormed);
        Cv2.Threshold(result, result, 0.7, 1.0, ThresholdTypes.Tozero);
        Cv2.MinMaxLoc(result, out _, out var maxVal, out _, out var maxLoc);

        return (maxVal, maxLoc);
    }

    private static byte[][] LoadButtonData()
    {
        var buttons = Array.Empty<byte[]>();
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

            Array.Resize(ref buttons, buttons.Length + 1);
            buttons[^1] = data;
        }

        return buttons;
    }

    private static Dictionary<string, Image> GetSamples()
    {
        var dict = new Dictionary<string, Image>();
        var buttonNames = File.ReadAllLines(BUTTON_NAMES_PATH);
        var buttonData = LoadButtonData();

        for (int i = 0; i < buttonNames.Length; i++)
        {
            var sample = buttonData[i];
            using var ms = new MemoryStream(sample);
            Image.FromStream(ms);
            dict.Add(buttonNames[i], Image.FromStream(ms));
        }

        return dict;
    }

    public Dictionary<string, Mat> MakeTemplates(Image screenshot)
    {
        var images = GetSamples();
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