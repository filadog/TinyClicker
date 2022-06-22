using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using OpenCvSharp;
using OpenCvSharp.Extensions;

namespace TinyClicker;

public class TinyClickerApp
{
    internal int _floorToRebuildAt;
    internal bool _acceptBuxVideoOffers;
    internal int _floorToStartWatchingAds;

    internal Dictionary<string, int> _matchedTemplates;
    internal Dictionary<string, Image> _images;
    internal Dictionary<string, Mat> _templates;
    internal MainWindow _window;

    readonly ClickerActionsRepo _clickerActionsRepo;
    readonly ConfigManager _configManager;

    internal bool _isBluestacks;
    internal bool _isLDPlayer;

    public TinyClickerApp(bool isBluestacks)
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
        _clickerActionsRepo = new ClickerActionsRepo(this, _configManager);

        _matchedTemplates = new Dictionary<string, int>();
        _images = _clickerActionsRepo.FindImages();
        _templates = _clickerActionsRepo.MakeTemplates(_images);
        _window = Application.Current.Windows.OfType<MainWindow>().First();

        _floorToRebuildAt = _configManager._curConfig.RebuildAtFloor;
        _acceptBuxVideoOffers = _configManager._curConfig.WatchBuxAds;
        _floorToStartWatchingAds = _configManager._curConfig.WatchAdsFromFloor;
    }

    public void StartInBackground(BackgroundWorker worker)
    {
        worker.WorkerSupportsCancellation = true;
        worker.DoWork += (s, e) =>
        {
            RunClickerLoop(worker);
        };
        worker.RunWorkerAsync();
    }

    // Main loop
    public void RunClickerLoop(BackgroundWorker worker)
    {
        int processId = _clickerActionsRepo._processId;
        int foundNothing = 0;
        int curHour = DateTime.Now.Hour - 1;
        int curSecond = DateTime.Now.Second - 1;
        int sameItemCounter = 0;
        string lastItemName = "";

        // TODO: Clean this up
        while (processId != -1 && !worker.CancellationPending)
        {
            var _currentFloor = _configManager._curConfig.CurrentFloor;
            var dateTimeNow = DateTime.Now.ToString("HH:mm:ss");

            Image gameWindow = _clickerActionsRepo.MakeScreenshot();

            // Update the static list of found images via template matching
            MatchTemplates(gameWindow);

            // Cancel the execution of the next loop iteration if termination is requested
            if (worker.CancellationPending)
            {
                _window.Log("Stopped!");
                break;
            }

            // Print the name of the found object
            foreach (var image in _matchedTemplates)
            {
                string msg = dateTimeNow + " Found " + image.Key;
                _window.Log(msg);
                foundNothing = 0;
                
                // Check if the clicker froze
                if (lastItemName == image.Key) 
                {
                    sameItemCounter++;
                    if (sameItemCounter > 5)
                    {
                        _clickerActionsRepo.SendEscapeButton();
                        sameItemCounter = 0;
                    }
                }
                else
                {
                    sameItemCounter = 0;
                }
                lastItemName = image.Key;
            }

            // Print if nothing was found and restart the app if necessary
            if (_matchedTemplates.Count == 0)
            {
                foundNothing++;
                string msg = dateTimeNow + " Found nothing x" + foundNothing;
                _window.Log(msg);

                // Try to close ads with improper close button location after 20 attempts
                if (foundNothing >= 20)
                    _clickerActionsRepo.CloseHiddenAd(); 
                if (foundNothing >= 23)
                    _clickerActionsRepo.RestartGame();
            }

            // Commence the turorial at the first floor
            if (_currentFloor == 1)
                _clickerActionsRepo.PassTheTutorial();

            // Play the hourly raffle at the beginning of every hour and perform all actions
            if (_currentFloor != _floorToRebuildAt)
            {
                PerformActions();
                curHour = _clickerActionsRepo.PlayRaffle(curHour);
            }

            // Check for a new floor every iteration
            if (gameWindow != null)
            {
                if (curSecond != DateTime.Now.Second && _currentFloor != 1)
                {
                    curSecond = DateTime.Now.Second;
                    _clickerActionsRepo.CheckForNewFloor(_currentFloor, gameWindow);
                }
                gameWindow.Dispose();
            }
            
            _matchedTemplates.Clear();
            Task.Delay(1500).Wait();
        }
    }

    void MatchTemplates(Image gameWindow)
    {
        var windowBitmap = new Bitmap(gameWindow);
        Mat reference = BitmapConverter.ToMat(windowBitmap);
        windowBitmap.Dispose();

        foreach (var template in _templates)
        {
            if (_matchedTemplates.Count == 0)
            {
                if (!_matchedTemplates.ContainsKey(template.Key))
                {
                    _clickerActionsRepo.MatchSingleTemplate(template, reference);
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

    void PerformActions()
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
