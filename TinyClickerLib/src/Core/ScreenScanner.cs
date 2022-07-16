using OpenCvSharp;
using OpenCvSharp.Extensions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace TinyClickerLib;

public class ScreenScanner
{
    internal int _floorToRebuildAt;
    internal bool _acceptBuxVideoOffers;
    internal int _floorToStartWatchingAds;

    internal Dictionary<string, int> _matchedTemplates;
    internal Dictionary<string, Image> _samples;
    internal Dictionary<string, Mat> _templates;
    internal MainWindow _window;

    public readonly ClickerActionsRepo clickerActions;
    public readonly ConfigManager configManager;

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

        configManager = new ConfigManager();
        clickerActions = new ClickerActionsRepo(this);
        _matchedTemplates = new Dictionary<string, int>();
        _samples = clickerActions.GetSamples();
        _templates = clickerActions.MakeTemplates(_samples);
        _window = Application.Current.Windows.OfType<MainWindow>().First();

        _floorToRebuildAt = configManager.curConfig.RebuildAtFloor;
        _acceptBuxVideoOffers = configManager.curConfig.WatchBuxAds;
        _floorToStartWatchingAds = configManager.curConfig.WatchAdsFromFloor;

        _foundNothing = 0;
        _curHour = DateTime.Now.Hour - 1;
        _curSecond = DateTime.Now.Second - 1;
        _sameItemCounter = 0;
        _lastItemName = "";
        _dateTimeNow = DateTime.Now.ToString("HH:mm:ss");
    }

    public void StartIteration()
    {
        // Get an image of the game screen
        Image gameWindow = clickerActions.inputSim.MakeScreenshot();
        _currentFloor = configManager.curConfig.CurrentFloor;

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
                    clickerActions.inputSim.SendEscapeButton();
                    clickerActions.inputSim.SendEscapeButton();
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
                clickerActions.CloseHiddenAd();
            }
            // TODO: Fix the game restart
            //if (foundNothing >= 23)
            //    _clickerActionsRepo.RestartGame();
        }

        // Commence the turorial at the first floor
        if (_currentFloor == 1)
        {
            clickerActions.PassTheTutorial();
        }

        // Play the hourly raffle at the beginning of every hour and perform all actions
        if (_currentFloor != _floorToRebuildAt)
        {
            PerformActions();
            _curHour = clickerActions.PlayRaffle(_curHour);
        }

        // Check for a new floor every iteration
        if (gameWindow != null)
        {
            if (_curSecond != DateTime.Now.Second && _currentFloor != 1)
            {
                _curSecond = DateTime.Now.Second;
                clickerActions.CheckForNewFloor(_currentFloor, gameWindow);
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
                    _matchedTemplates.Add(template.Key, clickerActions.inputSim.MakeLParam(maxloc.X, maxloc.Y));
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
                case "freeBuxCollectButton": clickerActions.CollectFreeBux(); break;
                case "roofCustomizationWindow": clickerActions.ExitRoofCustomizationMenu(); break;
                case "hurryConstructionPrompt": clickerActions.CancelHurryConstruction(); break;
                case "closeAd":
                case "closeAd_2":
                case "closeAd_3":
                case "closeAd_4":
                case "closeAd_5":
                case "closeAd_6":
                case "closeAd_7":
                case "closeAd_8":
                case "closeAd_9": clickerActions.CloseAd(); break;
                case "continueButton": clickerActions.PressContinue(); break;
                case "foundCoinsChuteNotification": clickerActions.CloseChuteNotification(); break;
                case "restockButton": clickerActions.Restock(); break;
                case "freeBuxButton": clickerActions.PressFreeBuxButton(); break;
                case "giftChute": clickerActions.ClickOnChute(); break;
                case "backButton": clickerActions.PressExitButton(); break;
                case "elevatorButton": clickerActions.RideElevator(); break;
                case "questButton": clickerActions.PressQuestButton(); break;
                case "completedQuestButton": clickerActions.CompleteQuest(); break;
                case "watchAdPromptCoins": clickerActions.WatchCoinsAds(); break;
                case "watchAdPromptBux": clickerActions.WatchBuxAds(); break;
                case "findBitizens": clickerActions.FindBitizens(); break;
                case "deliverBitizens": clickerActions.DeliverBitizens(); break;
                case "newFloorMenu": clickerActions.CloseNewFloorMenu(); break;
                case "buildNewFloorNotification": clickerActions.CloseBuildNewFloorNotification(); break;
                case "gameIcon": clickerActions.OpenTheGame(); break;
                case "adsLostReward": clickerActions.CheckForLostAdsReward(); break;
                default: break;
            }
        }
    }
}
