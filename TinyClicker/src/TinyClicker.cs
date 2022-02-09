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

namespace TinyClickerUI
{
    public static class TinyClicker
    {
        #region Fields

        public static bool stopped;
        public static Config currentConfig = ConfigManager.GetConfig();
        public static int balance = currentConfig.Coins;
        public static int currentFloor = currentConfig.FloorsNumber;
        public static float elevatorSpeed = currentConfig.ElevatorSpeed;
        public static bool vipPackage = currentConfig.VipPackage;
        public static int floorToRebuildAt = 50;
        public static bool acceptBuxVideoOffers = false; // Should be true by default

        public static Dictionary<string, int> matchedTemplates = new Dictionary<string, int>();
        public static Dictionary<string, Image> images = ClickerActions.FindImages();
        public static Dictionary<string, Mat> templates = ClickerActions.MakeTemplates(images);

        public static MainWindow window = Application.Current.Windows.OfType<MainWindow>().First();

        #endregion

        public static void StartInBackground(BackgroundWorker worker)
        {
            worker.DoWork += (s, e) =>
            {
                RunClickerLoop(worker);
            };
            worker.RunWorkerAsync();
        }

        // Main loop of the clicker
        public static void RunClickerLoop(BackgroundWorker worker)
        {
            int processId = ClickerActions.processId;
            int foundNothing = 0;
            int curHour = DateTime.Now.Hour - 1;
            int curSecond = DateTime.Now.Second - 1;
            int sameItemCounter = 0;
            string lastItemName = "";

            while (processId != -1 && !worker.CancellationPending)
            {
                currentFloor = ConfigManager.GetConfig().FloorsNumber;
                string dateTimeNow = DateTime.Now.ToString("HH:mm:ss");
                Image gameWindow = ClickerActions.MakeScreenshot();

                // Update the static list of found images via template matching
                if (gameWindow != null)
                {
                    MatchTemplates(gameWindow);
                }

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
                            ClickerActions.PressEscape();
                            sameItemCounter = 0;
                        }
                    }
                    lastItemName = image.Key;
                }

                // Print if nothing was found and restart the app if nothing was found for too long
                if (matchedTemplates.Count == 0)
                {
                    foundNothing++;
                    string msg = dateTimeNow + " Found nothing x" + foundNothing;
                    window.Log(msg);

                    // Try to close ads with improper close button location after 20 attempts
                    if (foundNothing >= 20)
                        ClickerActions.CloseHiddenAd(); 
                    if (foundNothing >= 23) 
                        ClickerActions.RestartGame();
                }

                // Commence the turorial at the first floor
                if (currentFloor == 1)
                    ClickerActions.PassTheTutorial();

                // Play the hourly raffle at the beginning of every hour and perform all actions
                if (currentFloor != floorToRebuildAt)
                {
                    PerformActions();
                    curHour = ClickerActions.PlayRaffle(curHour);
                }

                // Check for buildable floor every iteration
                if (gameWindow != null)
                {
                    if (curSecond != DateTime.Now.Second && currentFloor != 1)
                    {
                        curSecond = DateTime.Now.Second;
                        ClickerActions.CheckBuildableFloor(currentFloor, gameWindow);
                    }
                    gameWindow.Dispose();
                }
                
                GC.Collect();
                matchedTemplates.Clear();
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
                        ClickerActions.MatchSingleTemplate(template, reference);
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
                    case "freeBuxCollectButton": ClickerActions.CollectFreeBux(); break;
                    case "roofCustomizationWindow": ClickerActions.ExitRoofCustomizationMenu(); break;
                    case "hurryConstructionPrompt": ClickerActions.CancelHurryConstruction(); break;
                    case "closeAd":
                    case "closeAd_2":
                    case "closeAd_3":
                    case "closeAd_4":
                    case "closeAd_5":
                    case "closeAd_6":
                    case "closeAd_7":
                    case "closeAd_8":
                    case "closeAd_9": ClickerActions.CloseAd(); break;
                    case "continueButton": ClickerActions.PressContinue(); break;
                    case "foundCoinsChuteNotification": ClickerActions.CloseChuteNotification(); break;
                    case "restockButton": ClickerActions.Restock(); break;
                    case "freeBuxButton": ClickerActions.PressFreeBuxButton(); break;
                    case "giftChute": ClickerActions.ClickOnChute(); break;
                    case "backButton": ClickerActions.PressExitButton(); break;
                    case "elevatorButton": ClickerActions.RideElevator(); break;
                    case "questButton": ClickerActions.PressQuestButton(); break;
                    case "completedQuestButton": ClickerActions.CompleteQuest(); break;
                    case "watchAdPromptCoins": ClickerActions.WatchCoinsAds(); break;
                    case "watchAdPromptBux": ClickerActions.WatchBuxAds(); break;
                    case "findBitizens": ClickerActions.FindBitizens(); break;
                    case "deliverBitizens": ClickerActions.DeliverBitizens(); break;
                    case "newFloorMenu": ClickerActions.CloseNewFloorMenu(); break;
                    case "buildNewFloorNotification": ClickerActions.CloseBuildNewFloorNotification(); break;
                    case "gameIcon": ClickerActions.OpenTheGame(); break;
                    case "adsLostReward": ClickerActions.CheckForLostAdsReward(); break;
                    default: break;
                }
            }
        }
    }
}
