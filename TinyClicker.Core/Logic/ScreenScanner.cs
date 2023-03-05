using OpenCvSharp;
using OpenCvSharp.Extensions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using TinyClicker.Core.Extensions;
using TinyClicker.Core.Helpers;
using TinyClicker.Core.Logging;

namespace TinyClicker.Core.Logic;

public class ScreenScanner
{
    private readonly ILogger _logger;
    private readonly ConfigManager _configManager;
    private readonly ClickerActionsRepository _clickerActionsRepo;
    private readonly InputSimulator _inputSimulator;

    internal bool _isBluestacks;
    int _curSecond;

    public ScreenScanner(
        ConfigManager configManager,
        ClickerActionsRepository clickerActionsRepo,
        ILogger logger,
        InputSimulator inputSimulator)
    {
        _configManager = configManager;
        _clickerActionsRepo = clickerActionsRepo;
        _inputSimulator = inputSimulator;
        _logger = logger;

        AcceptBuxVideoOffers = _configManager.CurrentConfig.WatchBuxAds;
        FloorToStartWatchingAds = _configManager.CurrentConfig.WatchAdsFromFloor;
        FloorToRebuildAt = _configManager.CurrentConfig.RebuildAtFloor;
        _curSecond = DateTime.Now.Second - 1;
    }

    public Dictionary<string, Mat> Templates { get; private set; } = new();
    public Dictionary<string, int> FoundImages { get; private set; }
    public int FloorToRebuildAt { get; private set; }
    public int FloorToStartWatchingAds { get; private set; }
    public bool AcceptBuxVideoOffers { get; private set; }
    private int FoundCount { get; set; } = 0;

    public void Init(bool isBluestacks)
    {
        _isBluestacks = isBluestacks;
        _clickerActionsRepo.Init(this);
        _inputSimulator.Init(this);
    }

    public void StartIteration()
    {
        var gameWindow = _inputSimulator.MakeScreenshot();
        var currentFloor = _configManager.CurrentConfig.CurrentFloor;

        if (!Templates.Any())
        {
            Templates = _clickerActionsRepo.MakeTemplates(gameWindow);
        }

        FoundImages = TryFindFirstOnScreen(gameWindow, Templates);

        foreach (var image in FoundImages)
        {
            var msg = "Found " + image.Key;
            _logger.Log(msg);
            FoundCount = 0;
        }

        if (FoundImages.Count == 0)
        {
            FoundCount++;
            var msg = "Found nothing x" + FoundCount;
            _logger.Log(msg);

            if (FoundCount >= 20)
            {
                _clickerActionsRepo.CloseAd();
            }
        }

        if (currentFloor == 1)
        {
            _clickerActionsRepo.PassTheTutorial();
            _configManager.SetCurrentFloor(4); // 3 is the default old value
            return;
        }

        if (currentFloor != FloorToRebuildAt)
        {
            if (FoundImages.Any())
            {
                var image = FoundImages.First();
                PerformActions((image.Key, image.Value));
            }

            _clickerActionsRepo.PlayRaffle();
        }

        if (_curSecond != DateTime.Now.Second && currentFloor != 1)
        {
            if (_configManager.CurrentConfig.BuildFloors)
            {
                _curSecond = DateTime.Now.Second;
                _clickerActionsRepo.CheckForNewFloor(currentFloor, gameWindow);
            }
        }

        FoundImages.Clear();
        gameWindow!.Dispose();
    }

    public Dictionary<string, int> TryFindFirstOnScreen(Image gameScreen, Dictionary<string, Mat> templates)
    {
        var screenBitmap = new Bitmap(gameScreen);
        var screenMat = screenBitmap.ToMat();
        screenBitmap.Dispose();

        var result = new Dictionary<string, int>();
        foreach (var template in templates)
        {
            if (template.Key == "gameIcon" || template.Key == "balanceCoin" || template.Key == "restockButton")
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

        screenMat.Dispose();
        return result;
    }

    public (string Key, int Location) TryFindSingle(KeyValuePair<string, Mat> template, Mat reference)
    {
        using (Mat res = new(reference.Rows - template.Value.Rows + 1, reference.Cols - template.Value.Cols + 1, MatType.CV_8S))
        {
            Mat gref = reference.CvtColor(ColorConversionCodes.BGR2GRAY);
            Mat gtpl = template.Value.CvtColor(ColorConversionCodes.BGR2GRAY);

            Cv2.MatchTemplate(gref, gtpl, res, TemplateMatchModes.CCoeffNormed);
            Cv2.Threshold(res, res, 0.7, 1.0, ThresholdTypes.Tozero);

            gref.Dispose();
            gtpl.Dispose();

            while (true)
            {
                double threshold = 0.78;
                Cv2.MinMaxLoc(res, out _, out double maxval, out _, out OpenCvSharp.Point maxloc);
                res.Dispose();

                if (maxval >= threshold)
                {
                    if (template.Key == Button.GiftChute.GetName())
                    {
                        return new(template.Key, _inputSimulator.MakeLParam(maxloc.X + 40, maxloc.Y + 40));
                    }
                    else
                    {
                        return new(template.Key, _inputSimulator.MakeLParam(maxloc.X, maxloc.Y + 10));
                    }
                }
                else
                {
                    return new("", 0);
                }
            }
        }
    }

    public void PerformActions((string Key, int Location) item)
    {
        switch (item.Key)
        {
            case "closeAd":
            case "closeAd_2":
            case "closeAd_3":
            case "closeAd_4":
            case "closeAd_5":
            case "closeAd_6":
            case "closeAd_7":
            case "closeAd_8":
            case "closeAd_9": _clickerActionsRepo.CloseAd(); break;
            case "freeBuxCollectButton": _clickerActionsRepo.CollectFreeBux(); break;
            case "roofCustomizationWindow": _clickerActionsRepo.ExitRoofCustomizationMenu(); break;
            case "hurryConstructionPrompt": _clickerActionsRepo.CancelHurryConstruction(); break;
            case "continueButton": _clickerActionsRepo.PressContinue(); break;
            case "foundCoinsChuteNotification": _clickerActionsRepo.CloseChuteNotification(); break;
            case "restockButton": _clickerActionsRepo.Restock(); break;
            case "freeBuxButton": _clickerActionsRepo.PressFreeBuxButton(); break;
            case "giftChute": _clickerActionsRepo.ClickOnChute(); break;
            case "backButton": _clickerActionsRepo.PressExitButton(); break;
            case "elevatorButton": _clickerActionsRepo.RideElevator(); break;
            case "questButton": _clickerActionsRepo.PressQuestButton(); break;
            case "completedQuestButton": _clickerActionsRepo.CompleteQuest(); break;
            case "watchAdPromptCoins":
            case "watchAdPromptBux": _clickerActionsRepo.TryWatchAds(); break;
            case "findBitizens": _clickerActionsRepo.FindBitizens(); break;
            case "deliverBitizens": _clickerActionsRepo.DeliverBitizens(); break;
            case "newFloorMenu": _clickerActionsRepo.CloseNewFloorMenu(); break;
            case "buildNewFloorNotification": _clickerActionsRepo.CloseBuildNewFloorNotification(); break;
            case "gameIcon": _clickerActionsRepo.OpenTheGame(); break;
            case "adsLostReward": _clickerActionsRepo.CheckForLostAdsReward(); break;
            case "newScienceButton": _clickerActionsRepo.CollectNewScience(); break;
            default: break;
        }
    }
}
