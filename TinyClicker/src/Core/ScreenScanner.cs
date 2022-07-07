using OpenCvSharp;
using OpenCvSharp.Extensions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace TinyClicker;

public class ScreenScanner
{
    internal int _floorToRebuildAt;
    internal bool _acceptBuxVideoOffers;
    internal int _floorToStartWatchingAds;

    internal Dictionary<string, int> _matchedTemplates;
    internal Dictionary<string, Image> _samples;
    internal Dictionary<string, Mat> _templates;
    internal MainWindow _window;

    public readonly ClickerActionsRepo _clickerActionsRepo;
    readonly ConfigManager _configManager;

    internal bool _isBluestacks;
    internal bool _isLDPlayer;

    int _foundNothing;
    int _curHour;
    int _curSecond;
    int _sameItemCounter;
    string _lastItemName;
    int _currentFloor;
    string _dateTimeNow;

    public ScreenScanner(bool isBluestacks)
    {
        if (isBluestacks)
        {
            _isBluestacks = true;
            _isLDPlayer = false;
        }
        else
        {
            _isBluestacks = false;
            _isLDPlayer = true;
        }

        _configManager = new ConfigManager();
        _clickerActionsRepo = new ClickerActionsRepo(this);
        _matchedTemplates = new Dictionary<string, int>();
        _samples = _clickerActionsRepo.GetSamples();
        _templates = _clickerActionsRepo.MakeTemplates(_samples);
        _window = Application.Current.Windows.OfType<MainWindow>().First();

        _floorToRebuildAt = _configManager._curConfig.RebuildAtFloor;
        _acceptBuxVideoOffers = _configManager._curConfig.WatchBuxAds;
        _floorToStartWatchingAds = _configManager._curConfig.WatchAdsFromFloor;

        _foundNothing = 0;
        _curHour = DateTime.Now.Hour - 1;
        _curSecond = DateTime.Now.Second - 1;
        _sameItemCounter = 0;
        _lastItemName = "";
        _currentFloor = _configManager._curConfig.CurrentFloor;
        _dateTimeNow = DateTime.Now.ToString("HH:mm:ss");
    }

    public void StartIteration()
    {
        // Get an image of game screen
        Image gameWindow = _clickerActionsRepo.MakeScreenshot();

        // Update the list of found images on the screen
        TryFindAllOnScreen(gameWindow);

        // Print the name of the found object, if any
        foreach (var image in _matchedTemplates)
        {
            string msg = _dateTimeNow + " Found " + image.Key;
            _window.Log(msg);
            _foundNothing = 0;

            // Check if the clicker froze
            if (_lastItemName == image.Key)
            {
                _sameItemCounter++;
                if (_sameItemCounter > 5)
                {
                    _clickerActionsRepo.SendEscapeButton();
                    _clickerActionsRepo.SendEscapeButton();
                    _sameItemCounter = 0;
                }
            }
            else
            {
                _sameItemCounter = 0;
            }
            _lastItemName = image.Key;
        }

        // Print if nothing was found and restart the app if necessary
        if (_matchedTemplates.Count == 0)
        {
            _foundNothing++;
            string msg = _dateTimeNow + " Found nothing x" + _foundNothing;
            _window.Log(msg);

            // Try to close ads with improper close button location after 10 attempts
            if (_foundNothing >= 10)
            {
                _clickerActionsRepo.CloseHiddenAd();
            }
            // TODO: Fix the game restart
            //if (foundNothing >= 23)
            //    _clickerActionsRepo.RestartGame();
        }

        // Commence the turorial at the first floor
        if (_currentFloor == 1)
        {
            _clickerActionsRepo.PassTheTutorial();
        }

        // Play the hourly raffle at the beginning of every hour and perform all actions
        if (_currentFloor != _floorToRebuildAt)
        {
            PerformActions();
            _curHour = _clickerActionsRepo.PlayRaffle(_curHour);
        }

        // Check for a new floor every iteration
        if (gameWindow != null)
        {
            if (_curSecond != DateTime.Now.Second && _currentFloor != 1)
            {
                _curSecond = DateTime.Now.Second;
                _clickerActionsRepo.CheckForNewFloor(_currentFloor, gameWindow);
            }
            gameWindow.Dispose();
        }
        _matchedTemplates.Clear();
    }

    void TryFindAllOnScreen(Image gameWindow)
    {
        var windowBitmap = new Bitmap(gameWindow);
        Mat reference = BitmapConverter.ToMat(windowBitmap);
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
                    _matchedTemplates.Add(template.Key, _clickerActionsRepo.MakeLParam(maxloc.X, maxloc.Y));
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
                case "freeBuxCollectButton": _clickerActionsRepo.CollectFreeBux(); break;
                case "roofCustomizationWindow": _clickerActionsRepo.ExitRoofCustomizationMenu(); break;
                case "hurryConstructionPrompt": _clickerActionsRepo.CancelHurryConstruction(); break;
                case "closeAd":
                case "closeAd_2":
                case "closeAd_3":
                case "closeAd_4":
                case "closeAd_5":
                case "closeAd_6":
                case "closeAd_7":
                case "closeAd_8":
                case "closeAd_9": _clickerActionsRepo.CloseAd(); break;
                case "continueButton": _clickerActionsRepo.PressContinue(); break;
                case "foundCoinsChuteNotification": _clickerActionsRepo.CloseChuteNotification(); break;
                case "restockButton": _clickerActionsRepo.Restock(); break;
                case "freeBuxButton": _clickerActionsRepo.PressFreeBuxButton(); break;
                case "giftChute": _clickerActionsRepo.ClickOnChute(); break;
                case "backButton": _clickerActionsRepo.PressExitButton(); break;
                case "elevatorButton": _clickerActionsRepo.RideElevator(); break;
                case "questButton": _clickerActionsRepo.PressQuestButton(); break;
                case "completedQuestButton": _clickerActionsRepo.CompleteQuest(); break;
                case "watchAdPromptCoins": _clickerActionsRepo.WatchCoinsAds(); break;
                case "watchAdPromptBux": _clickerActionsRepo.WatchBuxAds(); break;
                case "findBitizens": _clickerActionsRepo.FindBitizens(); break;
                case "deliverBitizens": _clickerActionsRepo.DeliverBitizens(); break;
                case "newFloorMenu": _clickerActionsRepo.CloseNewFloorMenu(); break;
                case "buildNewFloorNotification": _clickerActionsRepo.CloseBuildNewFloorNotification(); break;
                case "gameIcon": _clickerActionsRepo.OpenTheGame(); break;
                case "adsLostReward": _clickerActionsRepo.CheckForLostAdsReward(); break;
                default: break;
            }
        }
    }
}
