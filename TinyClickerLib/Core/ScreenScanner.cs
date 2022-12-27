using OpenCvSharp;
using OpenCvSharp.Extensions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using TinyClicker.Core;
using TinyClickerLib.Extensions;

namespace TinyClicker;

public class ScreenScanner
{
    private readonly Logger _logger;
    private readonly ConfigManager _configManager;
    private readonly ClickerActionsRepo _clickerActionsRepo;
    private readonly InputSimulator _inputSimulator;

    internal int floorToRebuildAt;
    internal bool acceptBuxVideoOffers;
    internal int floorToStartWatchingAds;

    internal Dictionary<string, int> _matchedTemplates = new();
    internal Dictionary<string, Mat> _templates;

    internal bool _isBluestacks;

    int _foundNothing;
    int _lastRaffleTime;
    int _curSecond;
    int _currentFloor;

    public ScreenScanner(ConfigManager configManager, ClickerActionsRepo clickerActionsRepo, Logger logger, InputSimulator inputSimulator)
    {
        _configManager = configManager;
        _clickerActionsRepo = clickerActionsRepo;
        _inputSimulator = inputSimulator;
        _logger = logger;
    }

    public void Init(bool isBluestacks)
    {
        _isBluestacks = isBluestacks;
        _clickerActionsRepo.Init(this);
        _inputSimulator.Init(this);

        floorToRebuildAt = _configManager.curConfig.RebuildAtFloor;
        acceptBuxVideoOffers = _configManager.curConfig.WatchBuxAds;
        floorToStartWatchingAds = _configManager.curConfig.WatchAdsFromFloor;

        _templates = _clickerActionsRepo.MakeTemplates();

        _foundNothing = 0;
        _lastRaffleTime = DateTime.Now.Hour - 1;
        _curSecond = DateTime.Now.Second - 1;
    }

    public void StartIteration()
    {
        var gameWindow = _inputSimulator.MakeScreenshot();

        _currentFloor = _configManager.curConfig.CurrentFloor;

        TryFindAllOnScreen(gameWindow);

        foreach (var image in _matchedTemplates)
        {
            var msg = "Found " + image.Key;
            _logger.Log(msg);
            _foundNothing = 0;
        }

        if (_matchedTemplates.Count == 0)
        {
            _foundNothing++;
            var msg = "Found nothing x" + _foundNothing;
            _logger.Log(msg);

            if (_foundNothing >= 20)
            {
                _clickerActionsRepo.CloseHiddenAd();
            }
        }

        if (_currentFloor == 1)
        {
            _clickerActionsRepo.PassTheTutorial();
            return;
        }

        if (_currentFloor != floorToRebuildAt)
        {
            PerformActions();
            _lastRaffleTime = _clickerActionsRepo.PlayRaffle(_lastRaffleTime);
        }
        
        if (_curSecond != DateTime.Now.Second && _currentFloor != 1)
        {
            _curSecond = DateTime.Now.Second;
            _clickerActionsRepo.CheckForNewFloor(_currentFloor, gameWindow);
        }
        
        gameWindow!.Dispose();
        _matchedTemplates.Clear();
    }

    private void TryFindAllOnScreen(Image gameWindow)
    {
        var windowBitmap = new Bitmap(gameWindow);
        var reference = BitmapConverter.ToMat(windowBitmap);
        windowBitmap.Dispose();

        foreach (var template in _templates)
        {
            if (_matchedTemplates.Count == 0)
            {
                if (template.Key == "gameIcon" || template.Key == "balanceCoin" || template.Key == "restockButton")
                {
                    continue;
                }

                if (!_matchedTemplates.ContainsKey(template.Key))
                {
                    TryFindSingle(template, reference);
                    Task.Delay(10).Wait(); // Smooth the CPU peak load
                }
            }
            else
            {
                break;
            }
        }

        reference.Dispose();
    }

    public void TryFindSingle(KeyValuePair<string, Mat> template, Mat reference)
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
                        _matchedTemplates.Add(template.Key, _inputSimulator.MakeLParam(maxloc.X + 40, maxloc.Y + 40));
                    }
                    else
                    {
                        _matchedTemplates.Add(template.Key, _inputSimulator.MakeLParam(maxloc.X, maxloc.Y));
                    }

                    break;
                }
                else
                {
                    break;
                }
            }
        }
    }

    public void PerformActions()
    {
        string key;
        if (_matchedTemplates.Keys.Count > 0)
        {
            key = _matchedTemplates.Keys.First();
            switch (key)
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
}
