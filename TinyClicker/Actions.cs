using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using Point = OpenCvSharp.Point;

namespace TinyClicker
{
    internal static class Actions
    {
        static bool verbose = true;
        public const string processName = "dnplayer";
        static IntPtr clickableChildHandle = FindClickableChildHandles(processName);
        public static int processId = GetProcessId();

        static Dictionary<int, int> floors = Floors();

        #region Clicker Actions

        public static void ExitRoofCustomizationMenu()
        {
            if (verbose) Console.WriteLine("Exiting from the roof customization menu");

            PressExitButton();
            Wait(1);
        }

        public static void CancelHurryConstruction()
        {
            if (verbose) Console.WriteLine("Exiting from the construction menu");
            Click(100, 375); // Cancel action
            Wait(1);
        }

        public static void CloseAd()
        {
            if (verbose) Console.WriteLine("Closing advertisement");

            var matchedImages = Clicker.matchedImages;
            if(matchedImages.ContainsKey("closeAd_8") || matchedImages.ContainsKey("closeAd_9"))
            {
                Click(22, 22);
            }
            else
            {
                Click(311, 22);
            }
            Wait(1);
        }

        public static void PressContinue()
        {
            if (verbose) Console.WriteLine("Clicking continue");
            Click(Clicker.matchedImages["continueButton"]);
            Wait(1);
            MoveUp();
        }

        public static void CloseChuteNotification()
        {
            if (verbose) Console.WriteLine("Closing the parachute notification");
            Click(165, 375); // Close the notification
        }

        public static void Restock()
        {
            if (verbose) Console.WriteLine("Restocking");
            MoveDown();
            Wait(1);
            Click(100, 480); // Stock all
            Wait(1);
            Click(225, 375);
            Wait(1);

            if (MatchImage("fullyStockedBonus"))
            {
                Click(165, 375); // Close the bonus tooltip
                Wait(1);
                MoveUp(); // Go up
                Wait(1);
                return;
            }
            else
            {
                MoveUp(); // Go up
                Wait(1);
                return;
            }
        }

        public static void PressFreeBuxButton()
        {
            if (verbose) Console.WriteLine("Pressing free bux icon");
            Click(Clicker.matchedImages["freeBuxButton"]);
            Wait(1);
            Click(230, 375);
            Wait(1);
        }

        public static void CollectFreeBux()
        {
            if (verbose) Console.WriteLine("Collecting free bux");
            Click(Clicker.matchedImages["freeBuxCollectButton"]);
        }

        public static void ClickOnChute()
        {
            if (verbose) Console.WriteLine("Clicking on the parachute");
            Click(Clicker.matchedImages["giftChute"]);
            Wait(1);
            if (MatchImage("watchAdPromptBux") || MatchImage("watchAdPromptCoins"))
            {
                WatchAd();
            }
        }

        public static void RideElevator()
        {
            if (verbose) Console.WriteLine("Riding the elevator");
            Click(45, 535);
            Wait(4);
            if (MatchImage("giftChute"))
            {
                return;
            }
            else
            {
                MoveUp();
                Wait(1);
            }
        }

        public static void PressQuestButton()
        {
            if (verbose) Console.WriteLine("Clicking on the quest button");
            Wait(1);
            Click(Clicker.matchedImages["questButton"]);
            Wait(1);
            if (MatchImage("deliverBitizens"))
            {
                DeliverBitizens();
            }
            else if (MatchImage("findBitizens"))
            {
                FindBitizens();
            }
        }

        public static void FindBitizens()
        {
            if (verbose) Console.WriteLine("Skipping the quest");
            Click(95, 445); // Skip the quest
            Wait(1);
            Click(225, 380); // Confirm skip
        }

        public static void DeliverBitizens()
        {
            if (verbose) Console.WriteLine("Delivering bitizens");
            Click(230, 440); // Continue
        }

        public static void CloseNewFloorMenu()
        {
            PressExitButton();
        }

        public static void CloseBuildNewFloorNotification()
        {
            Click(105, 320); // Click no
        }

        public static void CompleteQuest()
        {
            Wait(1);
            if (verbose) Console.WriteLine("Completing the quest");
            Click(Clicker.matchedImages["completedQuestButton"]);
        }

        public static void WatchAd()
        {
            if (verbose) Console.WriteLine("Watching the advertisement");
            Click(225, 375);
            Wait(31);
        }

        public static void CheckBuildableFloor(int currentFloor, int balance)
        {
            int targetPrice = floors[currentFloor + 1];
            if (targetPrice < balance)
            {
                BuyFloor();
            }
        }

        public static void BuyFloor()
        {
            MoveUp();
            Wait(1);
            Click(195, 390);
            Wait(1);

            if (MatchImage("buildNewFloorNotification"))
            {
                Click(230, 320);
                ConfigManager.AddNewFloor();
            }
        }

        public static void MoveUp()
        {
            if (verbose) Console.WriteLine("Moving up");
            Click(160, 8);
        }

        public static void MoveDown()
        {
            Click(230, 580);
        }

        public static void PressExitButton()
        {
            if (verbose) Console.WriteLine("Pressing exit button");
            Click(305, 565);
        }

        public static void PlayRaffle()
        {
            while (true)
            {
                Console.WriteLine("Played raffle");
                Click(305, 575);
                Thread.Sleep(500);
                Click(275, 440);
                Thread.Sleep(5000);
                Click(165, 375);
                Thread.Sleep(2000);
                Click(165, 375);
                Thread.Sleep(216000000); // wait 1 hour
            }
        }

        #endregion

        #region Utility Methods

        public static void PrintInfo()
        {
            Console.WriteLine(
                "TinyClicker build v0.365"+
                "\nCurrent config: Vip = {0}, Elevator Speed = {1} FPS, Number of floors = {2}"+
                "\n\nCommands:" +
                "\ns - Enable clicker" +
                "\nl - Display all processes" +
                "\nq - Quit the application" +
                "\nss - Capture and save a screenshot" +
                "\ncc - Create new config", Clicker.currentConfig.VipPackage, Clicker.currentConfig.ElevatorSpeed, Clicker.currentConfig.FloorsNumber);
        }

        static void Wait(int seconds)
        {
            int milliseconds = seconds * 1000;
            Thread.Sleep(milliseconds);
        }

        static bool MatchImage(string imageKey)
        {
            // Returns true if image is found 

            Image gameWindow = Actions.MakeScreenshot();
            var windowBitmap = new Bitmap(gameWindow);
            gameWindow.Dispose();

            Mat reference = BitmapConverter.ToMat(windowBitmap);
            windowBitmap.Dispose();

            var template = Clicker.templates[imageKey];
            using (Mat res = new Mat(reference.Rows - template.Rows + 1, reference.Cols - template.Cols + 1, MatType.CV_32FC1))
            {
                Mat gref = reference.CvtColor(ColorConversionCodes.BGR2GRAY);
                Mat gtpl = template.CvtColor(ColorConversionCodes.BGR2GRAY);

                Cv2.MatchTemplate(gref, gtpl, res, TemplateMatchModes.CCoeffNormed);
                Cv2.Threshold(res, res, 0.7, 1.0, ThresholdTypes.Tozero);

                double minval, maxval, threshold = 0.7; // default 0.5
                Point minloc, maxloc;
                Cv2.MinMaxLoc(res, out minval, out maxval, out minloc, out maxloc);
                GC.Collect();

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

        public static int GetProcessId()
        {
            Process[] processlist = Process.GetProcesses();
            foreach (Process process in processlist)
            {
                if (!String.IsNullOrEmpty(process.MainWindowTitle) && process.ProcessName == processName)
                {
                    //Console.WriteLine("Process:   {0}", process.ProcessName);
                    //Console.WriteLine("    ID   : {0}", process.Id);
                    //Console.WriteLine("    Title: {0} \n", process.MainWindowTitle);
                    return process.Id;
                }
            }
            return -1;
        }

        public static void Click(int location)
        {
            if (clickableChildHandle != IntPtr.Zero)
            {
                MouseSim.SendMessage(clickableChildHandle, MouseSim.WM_LBUTTONDOWN, 1, location);
                MouseSim.SendMessage(clickableChildHandle, MouseSim.WM_LBUTTONUP, 0, location);
            }
        }

        public static void Click(int x, int y)
        {
            if (clickableChildHandle != IntPtr.Zero)
            {
                MouseSim.SendMessage(clickableChildHandle, MouseSim.WM_LBUTTONDOWN, 1, MakeParam(x, y));
                MouseSim.SendMessage(clickableChildHandle, MouseSim.WM_LBUTTONUP, 0, MakeParam(x, y));
            }   
        }

        public static int MakeParam(int x, int y) => (y << 16) | (x & 0xFFFF);

        public static IntPtr FindClickableChildHandles(string processName)
        {
            if (WindowHandleInfo.GetChildren(processName) != null)
            {
                List<IntPtr> childProcesses = WindowHandleInfo.GetChildren(processName);
                return childProcesses[0];
            }
            else
            {
                Console.WriteLine("LDPlayer process not found - clicker function is not possible. Launch LDPlayer and restart the app.");
                Console.ReadLine();
                return IntPtr.Zero;
            }
        }

        public static Image MakeScreenshot()
        {
            if (processId != -1)
            {
                IntPtr handle = Process.GetProcessById(processId).MainWindowHandle;

                ScreenToImage sc = new ScreenToImage();

                Image img = sc.CaptureWindow(handle);
                return img;

                // Captures screenshot of a window and saves it to screenshots folder

                //sc.CaptureWindowToFile(handle, Environment.CurrentDirectory + "\\screenshots\\mainWindow.png", ImageFormat.Png);
                ////sc.CaptureScreenToFile(Environment.CurrentDirectory + "\\screenshots\\temp.png", ImageFormat.Png);
                //Console.WriteLine("Made a screenchot you bastard");
                //Thread.Sleep(500);
            }
            else
            {
                Console.WriteLine("Error. No process with LDPlayer found. Try launching LDPlayer and restart the app");
                return null;
            }
        }

        public static void SaveScreenshot()
        {
            if (processId != -1)
            {
                IntPtr handle = Process.GetProcessById(processId).MainWindowHandle;
                ScreenToImage sc = new ScreenToImage();

                // Captures screenshot of a window and saves it to screenshots folder

                sc.CaptureWindowToFile(handle, Environment.CurrentDirectory + "\\screenshots\\window.png", ImageFormat.Png);
                Console.WriteLine("Made a screenshot. Screenshots can be found inside TinyClicker\\screenshots directory");
            }
        }

        public static void PrintAllProcesses()
        {
            Process[] processlist = Process.GetProcesses();
            foreach (Process process in processlist)
            {
                if (!String.IsNullOrEmpty(process.MainWindowTitle))
                {
                    Console.WriteLine("Process:   {0}", process.ProcessName);
                    Console.WriteLine("    ID   : {0}", process.Id);
                    Console.WriteLine("    Title: {0} \n", process.MainWindowTitle);
                }
            }
        }

        public static Dictionary<int, int> Floors()
        {
            // Prices for each floor
            var dict = new Dictionary<int, int>();
            dict.Add(2, 5000);
            dict.Add(3, 5000);
            dict.Add(4, 5000);
            dict.Add(5, 5000);
            dict.Add(6, 5000);
            dict.Add(7, 5000);
            dict.Add(8, 5000);
            dict.Add(9, 5000);
            dict.Add(10, 13000);
            dict.Add(11, 32000);
            dict.Add(12, 51000);
            dict.Add(13, 72000);
            dict.Add(14, 93000);
            dict.Add(15, 116000);
            dict.Add(16, 139000);
            dict.Add(17, 164000);
            dict.Add(18, 189000);
            dict.Add(19, 216000);
            dict.Add(20, 243000);
            dict.Add(21, 272000);
            dict.Add(22, 301000);
            dict.Add(23, 332000);
            dict.Add(24, 363000);
            dict.Add(25, 396000);
            dict.Add(26, 429000);
            dict.Add(27, 464000);
            dict.Add(28, 499000);
            dict.Add(29, 536000);
            dict.Add(30, 573000);
            dict.Add(31, 612000);
            dict.Add(32, 651000);
            dict.Add(33, 692000);
            dict.Add(34, 733000);
            dict.Add(35, 776000);
            dict.Add(36, 819000);
            dict.Add(37, 864000);
            dict.Add(38, 909000);
            dict.Add(39, 956000);
            dict.Add(40, 1003000);
            dict.Add(41, 1052000);
            dict.Add(42, 1101000);
            dict.Add(43, 1152000);
            dict.Add(44, 1203000);
            dict.Add(45, 1256000);
            dict.Add(46, 1309000);
            dict.Add(47, 1364000);
            dict.Add(48, 1419000);
            dict.Add(49, 1476000);
            dict.Add(50, 1533000);
            return dict;
        }

        public static Dictionary<string, Image> FindImages()
        {
            var dict = new Dictionary<string, Image>();
            string path = Path.Combine(Environment.CurrentDirectory, "samples\\");

            try
            {
                // Order is important

                dict.Add("freeBuxCollectButton", Image.FromFile(path + "free_bux_collect_button.png"));
                //dict.Add("menuButton", Image.FromFile(samplesPath + "menu_button.png"));
                dict.Add("watchAdPromptBux", Image.FromFile(path + "watch_ad_prompt_bux.png"));
                dict.Add("deliverBitizens", Image.FromFile(path + "deliver_bitizens.png"));
                dict.Add("continueButton", Image.FromFile(path + "continue_button.png"));
                dict.Add("findBitizens", Image.FromFile(path + "find_bitizens.png"));
                dict.Add("newFloorMenu", Image.FromFile(path + "new_floor_menu.png"));
                dict.Add("buildNewFloorNotification", Image.FromFile(path + "build_new_floor_notification.png"));
                dict.Add("backButton", Image.FromFile(path + "back_button.png"));
                dict.Add("elevatorButton", Image.FromFile(path + "elevator_button.png"));
                dict.Add("vipButton", Image.FromFile(path + "vip_button.png"));
                dict.Add("freeBuxButton", Image.FromFile(path + "free_bux_button.png"));
                dict.Add("freeBuxVidoffersButton", Image.FromFile(path + "free_bux_vidoffers_button.png"));
                dict.Add("questButton", Image.FromFile(path + "quest_button.png"));
                dict.Add("completedQuestButton", Image.FromFile(path + "completed_quest_button.png"));
                

                dict.Add("raffleIconMenu", Image.FromFile(path + "raffle_icon_menu.png"));
                //dict.Add("enterRaffleButton", Image.FromFile(path + "enter_raffle_button.png"));
                
                //dict.Add("rushAllButton", Image.FromFile(samplesPath + "rush_all_button.png"));
                //dict.Add("stockAllButton", Image.FromFile(samplesPath + "stock_all_button.png"));
                dict.Add("moveIn", Image.FromFile(path + "move_in.png"));
                dict.Add("restockButton", Image.FromFile(path + "restock_button.png"));
                
                dict.Add("foundCoinsChuteNotification", Image.FromFile(path + "found_coins_chute_notification.png"));
                dict.Add("watchAdPromptCoins", Image.FromFile(path + "watch_ad_prompt_coins.png"));
                dict.Add("closeAd", Image.FromFile(path + "close_ad_button.png"));
                dict.Add("closeAd_2", Image.FromFile(path + "close_ad_button_2.png"));
                dict.Add("closeAd_3", Image.FromFile(path + "close_ad_button_3.png"));
                dict.Add("closeAd_4", Image.FromFile(path + "close_ad_button_4.png"));
                dict.Add("closeAd_5", Image.FromFile(path + "close_ad_button_5.png"));
                dict.Add("closeAd_6", Image.FromFile(path + "close_ad_button_6.png"));
                dict.Add("closeAd_7", Image.FromFile(path + "close_ad_button_7.png"));
                dict.Add("closeAd_8", Image.FromFile(path + "close_ad_button_8.png"));
                dict.Add("fullyStockedBonus", Image.FromFile(path + "fully_stocked_bonus.png"));
                dict.Add("hurryConstructionPrompt", Image.FromFile(path + "hurry_construction_prompt.png"));
                dict.Add("roofCustomizationWindow", Image.FromFile(path + "roof_customization_window.png"));
                dict.Add("giftChute", Image.FromFile(path + "gift_chute.png"));

                return dict;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Cannot import all sample images, some are missing or renamed. Clicker will continue nonetheless.\nMissing image path: " + ex.Message);
                return dict;
            }
        }

        public static Dictionary<string, Mat> MakeTemplates(Dictionary<string, Image> images)
        {
            Dictionary<string, Mat> mats = new Dictionary<string, Mat>();
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
