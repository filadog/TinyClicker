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

public static class TinyClickerApp
{
    #region Fields

    internal static bool stopped;
    internal static Config currentConfig = ConfigManager.GetConfig();
    internal static int balance = currentConfig.Coins;
    internal static int currentFloor = currentConfig.FloorsNumber;
    internal static float elevatorSpeed = currentConfig.ElevatorSpeed;
    internal static bool vipPackage = currentConfig.VipPackage;
    internal static int floorToRebuildAt = 50;
    internal static bool acceptBuxVideoOffers = false; // Should be true by default
    internal static int floorToStartWatchingAds = 35;

    internal static Dictionary<string, int> matchedTemplates = new Dictionary<string, int>();
    internal static Dictionary<string, Image> images = ClickerActionsRepo.FindImages();
    internal static Dictionary<string, Mat> templates = ClickerActionsRepo.MakeTemplates(images);
    internal static MainWindow window = Application.Current.Windows.OfType<MainWindow>().First();

    // BlueStacks suppport

    internal static bool isBluestacks = true;
    internal static bool isLDPlayer = false;


    #endregion

    public static void StartInBackground(BackgroundWorker worker)
    {
        worker.DoWork += (s, e) =>
        {
            RunClickerLoop(worker);
        };
        worker.RunWorkerAsync();
    }

    // Main loop
    public static void RunClickerLoop(BackgroundWorker worker)
    {
        int processId = ClickerActionsRepo.processId;
        int foundNothing = 0;
        int curHour = DateTime.Now.Hour - 1;
        int curSecond = DateTime.Now.Second - 1;
        int sameItemCounter = 0;
        string lastItemName = "";

        // TODO: Clean this up
        while (processId != -1 && !worker.CancellationPending)
        {
            currentFloor = ConfigManager.GetConfig().FloorsNumber;
            string dateTimeNow = DateTime.Now.ToString("HH:mm:ss");

            // TODO: Add using statement 
            Image gameWindow = ClickerActionsRepo.MakeScreenshot();

            // Update the static list of found images via template matching
            MatchTemplates(gameWindow);

            // Cancel the execution of the next loop iteration if termination is requested
            if (worker.CancellationPending)
            {
                window.Log("Stopped!");
                break;
            }

            // Print the name of the found object
            foreach (var image in matchedTemplates)
            {
                string msg = dateTimeNow + " Found " + image.Key;
                window.Log(msg);
                foundNothing = 0;
                
                // Check if the clicker froze
                if (lastItemName == image.Key) 
                {
                    sameItemCounter++;
                    if (sameItemCounter > 5)
                    {
                        ClickerActionsRepo.SendEscapeButton();
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
            if (matchedTemplates.Count == 0)
            {
                foundNothing++;
                string msg = dateTimeNow + " Found nothing x" + foundNothing;
                window.Log(msg);

                // Try to close ads with improper close button location after 20 attempts
                if (foundNothing >= 20)
                    ClickerActionsRepo.CloseHiddenAd(); 
                if (foundNothing >= 23) 
                    ClickerActionsRepo.RestartGame();
            }

            // Commence the turorial at the first floor
            if (currentFloor == 1)
                ClickerActionsRepo.PassTheTutorial();

            // Play the hourly raffle at the beginning of every hour and perform all actions
            if (currentFloor != floorToRebuildAt)
            {
                PerformActions();
                curHour = ClickerActionsRepo.PlayRaffle(curHour);
            }

            // Check for a new floor every iteration
            if (gameWindow != null)
            {
                if (curSecond != DateTime.Now.Second && currentFloor != 1)
                {
                    curSecond = DateTime.Now.Second;
                    ClickerActionsRepo.CheckForNewFloor(currentFloor, gameWindow);
                }
                gameWindow.Dispose();
            }
            
            matchedTemplates.Clear();
            GC.Collect(0);
            Task.Delay(1500).Wait();
        }
    }

    static void MatchTemplates(Image gameWindow)
    {
        var windowBitmap = new Bitmap(gameWindow);
        Mat reference = BitmapConverter.ToMat(windowBitmap);
        windowBitmap.Dispose();

        foreach (var template in templates)
        {
            if (matchedTemplates.Count == 0)
            {
                if (!matchedTemplates.ContainsKey(template.Key))
                {
                    ClickerActionsRepo.MatchSingleTemplate(template, reference);
                    Task.Delay(15).Wait(); // Smooth the CPU peak load
                }
            }
            else
            {
                break;
            }
        }
        reference.Dispose();
    }

    static void PerformActions()
    {
        string key;
        if (matchedTemplates.Keys.Count > 0)
        {
            key = matchedTemplates.Keys.First();
            switch (key)
            {
                case "freeBuxCollectButton": ClickerActionsRepo.CollectFreeBux(); break;
                case "roofCustomizationWindow": ClickerActionsRepo.ExitRoofCustomizationMenu(); break;
                case "hurryConstructionPrompt": ClickerActionsRepo.CancelHurryConstruction(); break;
                case "closeAd":
                case "closeAd_2":
                case "closeAd_3":
                case "closeAd_4":
                case "closeAd_5":
                case "closeAd_6":
                case "closeAd_7":
                case "closeAd_8":
                case "closeAd_9": ClickerActionsRepo.CloseAd(); break;
                case "continueButton": ClickerActionsRepo.PressContinue(); break;
                case "foundCoinsChuteNotification": ClickerActionsRepo.CloseChuteNotification(); break;
                case "restockButton": ClickerActionsRepo.Restock(); break;
                case "freeBuxButton": ClickerActionsRepo.PressFreeBuxButton(); break;
                case "giftChute": ClickerActionsRepo.ClickOnChute(); break;
                case "backButton": ClickerActionsRepo.PressExitButton(); break;
                case "elevatorButton": ClickerActionsRepo.RideElevator(); break;
                case "questButton": ClickerActionsRepo.PressQuestButton(); break;
                case "completedQuestButton": ClickerActionsRepo.CompleteQuest(); break;
                case "watchAdPromptCoins": ClickerActionsRepo.WatchCoinsAds(); break;
                case "watchAdPromptBux": ClickerActionsRepo.WatchBuxAds(); break;
                case "findBitizens": ClickerActionsRepo.FindBitizens(); break;
                case "deliverBitizens": ClickerActionsRepo.DeliverBitizens(); break;
                case "newFloorMenu": ClickerActionsRepo.CloseNewFloorMenu(); break;
                case "buildNewFloorNotification": ClickerActionsRepo.CloseBuildNewFloorNotification(); break;
                case "gameIcon": ClickerActionsRepo.OpenTheGame(); break;
                case "adsLostReward": ClickerActionsRepo.CheckForLostAdsReward(); break;
                default: break;
            }
        }
    }
}
