using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using Point = OpenCvSharp.Point;

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
            //PlayRaffle();
            int processId = Actions.processId;
            int foundNothing = 0;
            int currentHour = DateTime.Now.Hour - 1;
            int currentMinute = DateTime.Now.Minute - 1;

            while (processId != -1)
            {
                string dateTimeNow = DateTime.Now.ToString("HH:mm:ss");
                Image gameWindow = Actions.MakeScreenshot();
                MatchImages(gameWindow);

                // Check the balance every minute
                if (currentMinute != DateTime.Now.Minute)
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
                    if (foundNothing >= 30) // Restart the game after 30 attempts
                    {
                        Console.WriteLine("Restarting the app");
                        Actions.RestartApp();
                    }
                }
                if (currentFloor == 1) Actions.PassTheTutorial();
                currentHour = Actions.PlayRaffle(currentHour);
                PerformActions();
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
                Thread.Sleep(12); // Smoothing the CPU peak load
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
                        double minval, maxval, threshold = 0.78; // default 0.78
                        Point minloc, maxloc;
                        Cv2.MinMaxLoc(res, out minval, out maxval, out minloc, out maxloc);
                        res.Dispose();
                        
                        if (maxval >= threshold)
                        {
                            if (!matchedImages.ContainsKey(template.Key))
                            {
                                matchedImages.Add(template.Key, Actions.MakeParam(maxloc.X, maxloc.Y));
                                //Console.WriteLine(image.Key + " is found");
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
                case "closeAd_8": Actions.CloseAd(); break;
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
