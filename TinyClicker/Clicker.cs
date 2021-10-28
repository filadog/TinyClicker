using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using OpenCvSharp;
using OpenCvSharp.Extensions;

namespace TinyClicker
{
    internal static class Clicker
    {
        public static Config currentConfig = ConfigManager.GetConfig();
        public static int balance = currentConfig.Coins;
        public static int currentFloor = currentConfig.FloorsNumber;
        public static float elevatorSpeed = currentConfig.ElevatorSpeed;
        public static bool vipPackage = currentConfig.VipPackage;

        public static Dictionary<string, int> matchedImages = new Dictionary<string, int>();
        public static Dictionary<string, Image> images = Actions.FindImages();
        public static Dictionary<string, Mat> templates = Actions.MakeTemplates(images);
        
        public static void StartClicker()
        {
            int processId = Actions.processId;
            int foundNothing = 0;
            int currentHour = DateTime.Now.Hour - 1;
            int currentMinute = DateTime.Now.Minute - 1;

            while (processId != -1)
            {
                currentFloor = ConfigManager.GetConfig().FloorsNumber;
                string dateTimeNow = DateTime.Now.ToString("HH:mm:ss");
                Image gameWindow = Actions.MakeScreenshot();
                MatchImages(gameWindow);

                // Check buildable floor every minute
                if (currentMinute != DateTime.Now.Minute && currentFloor != 1)
                {
                    currentMinute = DateTime.Now.Minute;
                    Actions.CheckBuildableFloor(currentFloor, gameWindow);
                }
                gameWindow.Dispose();

                foreach (var image in matchedImages)
                {
                    Console.WriteLine(dateTimeNow + " Found {0}", image.Key);
                    foundNothing = 0;
                }

                if (matchedImages.Count == 0)
                {
                    foundNothing++;
                    Console.WriteLine(dateTimeNow + " Found nothing x{0}", foundNothing);
                    if (foundNothing >= 27)
                    {
                        Actions.CloseHiddenAd(); // Close the hidden ad after 27 attempts
                    }
                    if (foundNothing >= 30) Actions.RestartApp();
                }

                if (currentFloor == 1) Actions.PassTheTutorial();

                if (currentFloor != 50)
                {
                    currentHour = Actions.PlayRaffle(currentHour);
                    PerformActions();
                } 
                Thread.Sleep(1000); // Object detection performed ~once a second
                matchedImages.Clear();
                GC.Collect();
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
                    Actions.MatchImage(template, reference);
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
            string key = matchedImages.Keys.FirstOrDefault();
            switch (key)
            {
                case "roofCustomizationWindow": Actions.ExitRoofCustomizationMenu(); break;
                case "hurryConstructionPrompt": Actions.CancelHurryConstruction(); break;
                case "closeAd":
                case "closeAd_2":
                case "closeAd_3":
                case "closeAd_4":
                case "closeAd_5":
                case "closeAd_6":
                case "closeAd_7":
                case "closeAd_8":
                case "closeAd_9": Actions.CloseAd(); break;
                case "continueButton": Actions.PressContinue(); break;
                case "foundCoinsChuteNotification": Actions.CloseChuteNotification(); break;
                case "restockButton": Actions.Restock(); break;
                case "freeBuxCollectButton": Actions.CollectFreeBux(); break;
                case "freeBuxButton": Actions.PressFreeBuxButton(); break;
                case "giftChute": Actions.ClickOnChute(); break;
                case "backButton": Actions.PressExitButton(); break;
                case "elevatorButton": Actions.RideElevator(); break;
                case "questButton": Actions.PressQuestButton(); break;
                case "completedQuestButton": Actions.CompleteQuest(); break;
                case "watchAdPromptBux": Actions.WatchAd(); break;
                case "findBitizens": Actions.FindBitizens(); break;
                case "deliverBitizens": Actions.DeliverBitizens(); break;
                case "newFloorMenu": Actions.CloseNewFloorMenu(); break;
                case "buildNewFloorNotification": Actions.CloseBuildNewFloorNotification(); break;
                case "gameIcon": Actions.OpenTheGame(); break;
                default: break;
            }
        }
    }
}
