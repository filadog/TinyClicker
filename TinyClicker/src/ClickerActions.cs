﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using Point = OpenCvSharp.Point;
using System.Threading.Tasks;

namespace TinyClickerUI
{
    internal static class ClickerActions
    {
        public const string processName = "dnplayer";
        static readonly IntPtr _clickableChildHandle = GetClickableChildHandles(processName);
        public static int processId = GetProcess().Id;
        static readonly Dictionary<int, int> _floorPrices = CalculateFloorPrices();
        static readonly MainWindow _window = TinyClicker.window;
        

        #region Clicker Actions

        public static void ExitRoofCustomizationMenu()
        {
            _window.Log("Exiting the roof customization menu");
            PressExitButton();
            Wait(1);
        }

        public static void CancelHurryConstruction()
        {
            _window.Log("Exiting the construction menu");

            Click(100, 375); // Cancel action
            Wait(1);
        }

        public static void CloseAd()
        {
            _window.Log("Closing the advertisement");

            if(TinyClicker.matchedTemplates.ContainsKey("closeAd_7") || TinyClicker.matchedTemplates.ContainsKey("closeAd_8"))
            {
                Click(22, 22);
                Click(311, 22);
                Click(302, 52);
            }
            else
            {
                Click(311, 22);
                Click(22, 22);
                Click(302, 52);
            }

            CheckForLostAdsReward();
        }

        public static void PressContinue()
        {
            _window.Log("Clicking continue");

            Click(TinyClicker.matchedTemplates["continueButton"]);
            Wait(1);
            MoveUp();
        }

        public static void CloseChuteNotification()
        {
            _window.Log("Closing the parachute notification");
            Click(165, 375); // Close the notification
        }

        public static void Restock()
        {
            _window.Log("Restocking");
            MoveDown();
            Wait(1);
            Click(100, 480); // Stock all
            Wait(1);
            Click(225, 375);
            Wait(1);

            if (IsImageFound("fullyStockedBonus"))
            {
                Click(165, 375); // Close the bonus tooltip
                Wait(1);
                MoveUp();
                Wait(1);
                return;
            }
            else
            {
                MoveUp();
                Wait(1);
                return;
            }
        }

        public static void PressFreeBuxButton()
        {
            _window.Log("Pressing free bux icon");
            Click(TinyClicker.matchedTemplates["freeBuxButton"]);
            Wait(1);
            Click(230, 375);
            Wait(1);
        }

        public static void CollectFreeBux()
        {
            _window.Log("Collecting free bux");
            Click(TinyClicker.matchedTemplates["freeBuxCollectButton"]);
        }

        public static void ClickOnChute()
        {
            _window.Log("Clicking on the parachute");
            Click(TinyClicker.matchedTemplates["giftChute"]);
            Wait(1);
            if (IsImageFound("watchAdPromptCoins"))
            {
                WatchCoinsAds();
            }
            else if (TinyClicker.acceptBuxVideoOffers)
            {
                if (IsImageFound("watchAdPromptBux"))
                {
                    WatchBuxAds();
                }
            }
            else
            {
                Click(105, 380); // Decline the video offer
            }
        }

        public static void RideElevator()
        {
            _window.Log("Riding the elevator");
            Click(45, 535);
            Wait(4);
            if (IsImageFound("giftChute"))
            {
                return;
            }
            else
            {
                MoveUp();
            }
        }

        public static void PressQuestButton()
        {
            _window.Log("Clicking on the quest button");
            Click(TinyClicker.matchedTemplates["questButton"]);
            Wait(1);
            if (IsImageFound("deliverBitizens"))
            {
                DeliverBitizens();
            }
            else if (IsImageFound("findBitizens"))
            {
                FindBitizens();
            }
            else
            {
                Click(90, 440); // Skip the quest
                Wait(1);
                Click(230, 380); // Confirm
            }
        }

        public static void FindBitizens()
        {
            _window.Log("Skipping the quest");
            Click(95, 445); // Skip the quest
            Wait(1);
            Click(225, 380); // Confirm skip
        }

        public static void DeliverBitizens()
        {
            _window.Log("Delivering bitizens");
            Click(230, 440); // Continue
        }

        public static void OpenTheGame()
        {
            Wait(1);
            Click(TinyClicker.matchedTemplates["gameIcon"]);
            Wait(10);
        }

        public static void CloseHiddenAd()
        {
            _window.Log("Closing the hidden ad");
            Wait(1);
            Click(310, 10);
            Wait(1);
            Click(311, 22);
            CheckForLostAdsReward();
        }

        public static void CheckForLostAdsReward()
        {
            Wait(1);
            if (IsImageFound("adsLostReward"))
            {
                Click(240, 344); // Click "Keep watching"
                Wait(15);
            }
            else
            {
                Click(240, 344);
            }
        }

        public static void CloseNewFloorMenu()
        {
            _window.Log("Exiting");
            PressExitButton();
        }

        public static void CloseBuildNewFloorNotification()
        {
            _window.Log("Closing the new floor notification");
            Click(105, 320); // Click no
        }

        public static void CompleteQuest()
        {
            _window.Log("Completing the quest");
            
            Wait(1);
            Click(TinyClicker.matchedTemplates["completedQuestButton"]);
        }

        public static void WatchCoinsAds()
        {
            _window.Log("Watching the advertisement");
            Click(225, 375);
            Wait(20);
        }
        public static void WatchBuxAds()
        {
            _window.Log("Watching the advertisement");
            Click(225, 375);
            Wait(20);
        }

        public static void CheckBuildableFloor(int currentFloor, Image gameWindow)
        {
            TinyClicker.currentFloor = ConfigManager.GetConfig().FloorsNumber;
            int balance = TextProcessor.ParseBalance(gameWindow);

            if (balance != 0 && balance != -1 && currentFloor >= 3)
            {
                int targetPrice = _floorPrices[currentFloor + 1];
                if (balance > targetPrice && currentFloor < TinyClicker.floorToRebuildAt)
                {
                    BuyFloor();
                }
                if (currentFloor >= TinyClicker.floorToRebuildAt)
                {
                    Wait(1);
                    RebuildTower();
                }
            }
        }

        public static void BuyFloor()
        {
            _window.Log("Building a new floor");
            MoveUp();
            Wait(2);
            Click(195, 390);
            Wait(1);

            if (IsImageFound("buildNewFloorNotification"))
            {
                Click(230, 320);
                Wait(1);
                if (!IsImageFound("newFloorNoCoinsNotification"))
                {
                    ConfigManager.AddOneFloor();
                    _window.Log("Built a new floor");
                }
            }
        }

        public static void RebuildTower()
        {
            _window.Log("Rebuilding the tower");
            SaveStatRebuildTime();
            Click(305, 570);
            Wait(1);
            Click(165, 435);
            Wait(1);
            Click(165, 440);
            Wait(1);
            Click(230, 380);
            Wait(1);
            Click(230, 380);
            Wait(3);
            ConfigManager.ChangeCurrentFloor(1);
            TinyClicker.currentFloor = 1;
        }

        public static void PassTheTutorial()
        {
            _window.Log("Passing the tutorial");
            Wait(3);
            Click(170, 435); // Continue
            Wait(3);
            MoveDown();
            Wait(1);
            Click(195, 260); // Build a new floor
            Wait(1);
            Click(230, 380); // Confirm
            Wait(1);
            Click(20, 60); // Complete quest
            Wait(1);
            Click(170, 435); // Collect bux
            Wait(1);
            Click(170, 435); // Continue
            Wait(1);
            Click(190, 300); // Click on new floor
            Wait(1);
            Click(240, 150); // Build a residential floor
            Wait(1);
            Click(160, 375); // Continue
            Wait(1);
            Click(20, 60); // Complete quest
            Wait(1);
            Click(170, 435); // Collect bux
            Wait(1);
            Click(170, 435); // Continue
            Wait(1);
            Click(30, 535); // Ride elevator
            Wait(5);
            Click(230, 380); // Continue
            Wait(1);
            Click(20, 60); // Complete quest
            Wait(1);
            Click(170, 435); // Collect bux
            Wait(1);
            Click(170, 435); // Continue
            Wait(1);
            Click(190, 200); // Build new floor
            Wait(1);
            Click(225, 380); // Confirm
            Wait(1);
            Click(200, 200); // Open the new floor
            Wait(1);
            Click(90, 340); // Build random food
            Wait(1);
            Click(170, 375); // Continue
            Wait(1);
            Click(20, 60); // Complete quest
            Wait(1);
            Click(170, 435); // Collect bux
            Wait(1);
            Click(170, 435); // Continue
            Wait(1);
            Click(200, 200); // Open food floor
            Wait(1);
            Click(75, 210); // Hire
            Wait(1);
            Click(80, 100); // Select our bitizen
            Wait(1);
            Click(230, 380); // Hire him
            Wait(1);
            Click(160, 380); // Continue on dream job assignement
            Wait(1);
            Click(300, 560); // Exit the food store
            Wait(1);
            Click(20, 60); // Complete quest
            Wait(1);
            Click(170, 435); // Collect bux
            Wait(1);
            Click(170, 435); // Continue
            Wait(1);
            Click(200, 200); // Open food store again
            Wait(1);
            Click(200, 210); // Restock first item in the store
            Wait(15);
            Click(305, 190); // Restock
            Wait(1);
            Click(20, 60); // Complete quest
            Wait(1);
            Click(170, 435); // Collect bux
            Wait(1);
            Click(170, 435); // Continue
            Wait(1);
            Click(200, 200); // Open food store again
            Wait(1);
            Click(170, 130); // Click upgrade
            Wait(1);
            Click(230, 375); // Confirm
            Wait(1);
            Click(165, 375); // Continue
            Wait(1);
            Click(300, 560); // Exit the food store
            Wait(1);
            Click(20, 60); // Complete quest
            Wait(1);
            Click(170, 435); // Collect bux
            Wait(1);
            Click(170, 435); // Colect more bux
            Wait(1);
            Click(165, 375); // Continue
            ConfigManager.ChangeCurrentFloor(3);
            TinyClicker.currentFloor = 3;
        }

        public static void RestartGame()
        {
            _window.Log("Restarting the app");
            IntPtr mainHandle = GetProcess().MainWindowHandle;

            InputSimulator.SendMessage(mainHandle, InputSimulator.WM_LBUTTONDOWN, 1, GenerateCoordinates(98, 17));
            InputSimulator.SendMessage(mainHandle, InputSimulator.WM_LBUTTONUP, 0, GenerateCoordinates(98, 17));

            Wait(1);
            InputSimulator.SendMessage(mainHandle, InputSimulator.WM_LBUTTONDOWN, 1, GenerateCoordinates(355, 547));
            InputSimulator.SendMessage(mainHandle, InputSimulator.WM_LBUTTONUP, 0, GenerateCoordinates(355, 547));

            Wait(1);
        }

        public static void MoveUp()
        {
            Click(160, 8);
            Wait(1);
        }

        public static void MoveDown()
        {
            Click(230, 580);
        }

        public static void PressExitButton()
        {
            _window.Log("Pressing the exit button");
            Click(305, 565);
        }

        public static int PlayRaffle(int currentHour)
        {
            if (currentHour != DateTime.Now.Hour)
            {
                _window.Log("Playing the raffle");
                Wait(1);
                Click(300, 570);
                Wait(1);
                Click(275, 440);
                Wait(2);
                Click(165, 375);

                return DateTime.Now.Hour;
            }
            else
            {
                return currentHour;
            }
        }

        #endregion

        #region Utility Methods

        static void Wait(int seconds)
        {
            int milliseconds = seconds * 1000;
            Task.Delay(milliseconds).Wait();
        }

        static bool IsImageFound(string imageKey) // Returns true if the image is found 
        {
            Image gameWindow = MakeScreenshot();
            var windowBitmap = new Bitmap(gameWindow);
            gameWindow.Dispose();

            Mat reference = BitmapConverter.ToMat(windowBitmap);
            windowBitmap.Dispose();

            var template = TinyClicker.templates[imageKey];
            using (Mat res = new(reference.Rows - template.Rows + 1, reference.Cols - template.Cols + 1, MatType.CV_8S))
            {
                Mat gref = reference.CvtColor(ColorConversionCodes.BGR2GRAY);
                Mat gtpl = template.CvtColor(ColorConversionCodes.BGR2GRAY);

                Cv2.MatchTemplate(gref, gtpl, res, TemplateMatchModes.CCoeffNormed);
                Cv2.Threshold(res, res, 0.7, 1.0, ThresholdTypes.Tozero);

                double threshold = 0.7;
                Cv2.MinMaxLoc(res, out double minval, out double maxval, out Point minloc, out Point maxloc);
                //GC.Collect();

                if (maxval >= threshold)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public static void MatchSingleTemplate(KeyValuePair<string, Mat> template, Mat reference)
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
                    Cv2.MinMaxLoc(res, out _, out double maxval, out _, out Point maxloc);
                    res.Dispose();

                    if (maxval >= threshold)
                    {
                        TinyClicker.matchedTemplates.Add(template.Key, GenerateCoordinates(maxloc.X, maxloc.Y));
                        break;
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }

        public static Process? GetProcess()
        {
            Process[] processlist = Process.GetProcesses();
            foreach (Process process in processlist)
            {
                if (!string.IsNullOrEmpty(process.MainWindowTitle) && process.ProcessName == processName)
                {
                    return process;
                }
            }
            return null;
        }

        public static void Click(int location)
        {
            if (_clickableChildHandle != IntPtr.Zero)
            {
                InputSimulator.SendMessage(_clickableChildHandle, InputSimulator.WM_LBUTTONDOWN, 1, location);
                InputSimulator.SendMessage(_clickableChildHandle, InputSimulator.WM_LBUTTONUP, 0, location);
            }
        }

        public static void Click(int x, int y)
        {
            if (_clickableChildHandle != IntPtr.Zero)
            {
                InputSimulator.SendMessage(_clickableChildHandle, InputSimulator.WM_LBUTTONDOWN, 1, GenerateCoordinates(x, y));
                InputSimulator.SendMessage(_clickableChildHandle, InputSimulator.WM_LBUTTONUP, 0, GenerateCoordinates(x, y));
            }   
        }

        public static void PressEscape()
        {
            if (_clickableChildHandle != IntPtr.Zero)
            {
                InputSimulator.SendMessage(_clickableChildHandle, InputSimulator.WM_KEYDOWN, InputSimulator.VK_ESCAPE, 0);
            }
        }

        public static int GenerateCoordinates(int x, int y) => (y << 16) | (x & 0xFFFF); // Generates coordinates within the game screen

        public static IntPtr GetClickableChildHandles(string processName)
        {
            if (WindowHandleInfo.GetChildren(processName) != null)
            {
                List<IntPtr> childProcesses = WindowHandleInfo.GetChildren(processName);
                return childProcesses[0];
            }
            else
            {
                _window.Log("LDPlayer process not found - TinyClicker function is not possible. Launch LDPlayer and restart the app.");
                return IntPtr.Zero;
            }
        }

        public static Image? MakeScreenshot()
        {
            if (processId != -1)
            {
                IntPtr handle = Process.GetProcessById(processId).MainWindowHandle;

                ScreenshotManager sc = new();

                Image img = sc.CaptureWindow(handle);
                return img;
            }
            else
            {
                //Console.WriteLine("Error. No process with LDPlayer found. Try launching LDPlayer and restart the app");
                _window.Log("Error. No process with LDPlayer found. Try launching LDPlayer and restart the app");
                return null;
            }
        }

        public static void SaveScreenshot()
        {
            if (processId != -1)
            {
                if (!Directory.Exists(Environment.CurrentDirectory + @"\screenshots"))
                {
                    Directory.CreateDirectory(Environment.CurrentDirectory + @"\screenshots");
                }

                IntPtr handle = Process.GetProcessById(processId).MainWindowHandle;
                ScreenshotManager sc = new();

                // Captures screenshot of a window and saves it to the screenshots folder
                sc.CaptureWindowToFile(handle, Environment.CurrentDirectory + @"\screenshots\window.png", ImageFormat.Png);

                _window.Log(@"Made a screenshot. Screenshots can be found inside TinyClicker\screenshots folder");
            }
        }

        public static void SaveStatRebuildTime()
        {
            DateTime dateTimeNow = DateTime.Now;
            DateTime lastRebuild = ConfigManager.GetConfig().LastRebuildTime;
            double totalHours = 0d;

            if (lastRebuild != DateTime.MinValue)
            {
                TimeSpan diff = dateTimeNow - lastRebuild;
                totalHours = diff.TotalHours;
            }

            string statsPath = Environment.CurrentDirectory + @"\Stats.txt";

            ConfigManager.SaveNewRebuildTime(dateTimeNow);
            string data = $"\n{dateTimeNow} - rebuilt the tower.\nHours since the last rebuild: {totalHours:0.00}\n";
            File.AppendAllText(statsPath, data);
        }
        
        public static Dictionary<int, int> CalculateFloorPrices()
        {
            var dict = new Dictionary<int, int>();

            // Floors 1 through 9 cost 5000
            for (int i = 1; i <= 9; i++)
            {
                dict.Add(i, 5000);
            }

            // Calculate the prices for floors 10 through 50+
            for (int i = 10; i <= TinyClicker.floorToRebuildAt + 1; i++)
            {
                float floorCost = 1000 * 1 * (0.5f * (i * i) + 8 * i - 117);

                // Round up the result to match the game prices
                if (i % 2 != 0)
                {
                    floorCost += 500;
                }

                dict.Add(i, (int)floorCost);
            }
            return dict;
        }

        public static Dictionary<string, Image> FindImages()
        {
            var dict = new Dictionary<string, Image>();
            string path = Path.Combine(Environment.CurrentDirectory, @"samples\");

            try
            {
                // Order is important
                dict.Add("freeBuxCollectButton", Image.FromFile(path + "free_bux_collect_button.png"));
                dict.Add("watchAdPromptBux", Image.FromFile(path + "watch_ad_prompt_bux.png"));
                dict.Add("watchAdPromptCoins", Image.FromFile(path + "watch_ad_prompt_coins.png"));
                dict.Add("continueButton", Image.FromFile(path + "continue_button.png"));
                dict.Add("newFloorNoCoinsNotification", Image.FromFile(path + "new_floor_no_coins_notification.png"));
                dict.Add("backButton", Image.FromFile(path + "back_button.png"));
                dict.Add("findBitizens", Image.FromFile(path + "find_bitizens.png"));
                dict.Add("newFloorMenu", Image.FromFile(path + "new_floor_menu.png"));
                dict.Add("buildNewFloorNotification", Image.FromFile(path + "build_new_floor_notification.png"));
                dict.Add("deliverBitizens", Image.FromFile(path + "deliver_bitizens.png"));
                dict.Add("freeBuxButton", Image.FromFile(path + "free_bux_button.png"));
                //dict.Add("freeBuxVidoffersButton", Image.FromFile(path + "free_bux_vidoffers_button.png"));
                dict.Add("questButton", Image.FromFile(path + "quest_button.png"));
                dict.Add("completedQuestButton", Image.FromFile(path + "completed_quest_button.png"));
                dict.Add("gameIcon", Image.FromFile(path + "game_icon.png"));
                dict.Add("restockButton", Image.FromFile(path + "restock_button.png"));
                //dict.Add("foundCoinsChuteNotification", Image.FromFile(path + "found_coins_chute_notification.png"));
                dict.Add("closeAd", Image.FromFile(path + "close_ad_button.png"));
                dict.Add("closeAd_2", Image.FromFile(path + "close_ad_button_2.png"));
                dict.Add("closeAd_3", Image.FromFile(path + "close_ad_button_3.png"));
                dict.Add("closeAd_4", Image.FromFile(path + "close_ad_button_4.png"));
                dict.Add("closeAd_5", Image.FromFile(path + "close_ad_button_5.png"));
                dict.Add("closeAd_6", Image.FromFile(path + "close_ad_button_6.png"));
                dict.Add("closeAd_7", Image.FromFile(path + "close_ad_button_7.png"));
                dict.Add("closeAd_8", Image.FromFile(path + "close_ad_button_8.png"));
                dict.Add("closeAd_9", Image.FromFile(path + "close_ad_button_9.png"));
                dict.Add("fullyStockedBonus", Image.FromFile(path + "fully_stocked_bonus.png"));
                dict.Add("hurryConstructionPrompt", Image.FromFile(path + "hurry_construction_prompt.png"));
                dict.Add("roofCustomizationWindow", Image.FromFile(path + "roof_customization_window.png"));
                dict.Add("giftChute", Image.FromFile(path + "gift_chute.png"));
                dict.Add("elevatorButton", Image.FromFile(path + "elevator_button.png"));
                dict.Add("adsLostReward", Image.FromFile(path + "ads_lost_reward.png"));

                return dict;
            }
            catch (Exception ex)
            {
                string msg = "Cannot import all sample images, some are missing or renamed. \nMissing image path: " + ex.Message;
                _window.Log(msg);

                return dict;
            }
        }

        public static Dictionary<string, Mat> MakeTemplates(Dictionary<string, Image> images)
        {
            Dictionary<string, Mat> mats = new();
            foreach (var image in images)
            {
                var imageBitmap = new Bitmap(image.Value);
                Mat template = BitmapConverter.ToMat(imageBitmap);
                imageBitmap.Dispose();
                mats.Add(image.Key, template);
            }
            images.Clear();
            return mats;
        }

        #endregion
    }
}
