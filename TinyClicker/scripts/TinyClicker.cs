using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Threading;
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

        public static Dictionary<string, int> matchedImages = new Dictionary<string, int>();
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

            while (processId != -1 && !worker.CancellationPending)
            {
                currentFloor = ConfigManager.GetConfig().FloorsNumber;
                string dateTimeNow = DateTime.Now.ToString("HH:mm:ss");
                Image gameWindow = ClickerActions.MakeScreenshot();

                // Update the list of found images via template matching
                MatchImages(gameWindow);

                // Cancel the execution of the next loop iteration if cancelling is requested
                if (worker.CancellationPending)
                {
                    window.Print("Stopped!");
                    break;
                }

                // Print the name of the found object
                foreach (var image in matchedImages)
                {
                    string msg = dateTimeNow + " Found " + image.Key;
                    window.Print(msg);
                    foundNothing = 0;
                }

                // Print if nothing is found and restart the app if nothing is found for too long
                if (matchedImages.Count == 0)
                {
                    foundNothing++;
                    string msg = dateTimeNow + " Found nothing x" + foundNothing;
                    window.Print(msg);

                    // Close the hidden ad after 27 attempts
                    if (foundNothing >= 27)
                    {
                        
                        ClickerActions.CloseHiddenAd(); 
                    }
                    if (foundNothing >= 30) ClickerActions.RestartApp();
                }

                if (currentFloor == 1) ClickerActions.PassTheTutorial();

                // Play the hourly raffle at the beginning of every hour and perform all actions
                if (currentFloor != floorToRebuildAt)
                {
                    curHour = ClickerActions.PlayRaffle(curHour);
                    PerformActions();
                }

                // Check buildable floor
                if (curSecond != DateTime.Now.Second && currentFloor != 1)
                {
                    curSecond = DateTime.Now.Second;
                    ClickerActions.CheckBuildableFloor(currentFloor, gameWindow);
                }
                gameWindow.Dispose();
                
                GC.Collect();
                matchedImages.Clear();
                Thread.Sleep(1000);
            }
        }

        static void MatchImages(Image gameWindow)
        {
            var windowBitmap = new Bitmap(gameWindow);
            Mat reference = BitmapConverter.ToMat(windowBitmap);
            windowBitmap.Dispose();

            foreach (var template in templates)
            {
                if (matchedImages.Count == 0)
                {
                    ClickerActions.MatchImage(template, reference);
                }
                else
                {
                    break;
                }
            }
            GC.Collect();
        }

        static void PerformActions()
        {
            string key;
            if (matchedImages.Keys.Count > 0)
            {
                key = matchedImages.Keys.First();
                switch (key)
                {
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
                    case "freeBuxCollectButton": ClickerActions.CollectFreeBux(); break;
                    case "freeBuxButton": ClickerActions.PressFreeBuxButton(); break;
                    case "giftChute": ClickerActions.ClickOnChute(); break;
                    case "backButton": ClickerActions.PressExitButton(); break;
                    case "elevatorButton": ClickerActions.RideElevator(); break;
                    case "questButton": ClickerActions.PressQuestButton(); break;
                    case "completedQuestButton": ClickerActions.CompleteQuest(); break;
                    case "watchAdPromptBux": ClickerActions.WatchAd(); break;
                    case "findBitizens": ClickerActions.FindBitizens(); break;
                    case "deliverBitizens": ClickerActions.DeliverBitizens(); break;
                    case "newFloorMenu": ClickerActions.CloseNewFloorMenu(); break;
                    case "buildNewFloorNotification": ClickerActions.CloseBuildNewFloorNotification(); break;
                    case "gameIcon": ClickerActions.OpenTheGame(); break;
                    default: break;
                }
            }
        }
    }
}
