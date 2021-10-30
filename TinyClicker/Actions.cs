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
        public static bool verbose = true;
        public const string processName = "dnplayer";
        static IntPtr clickableChildHandle = FindClickableChildHandles(processName);
        public static int processId = GetProcess().Id;

        static Dictionary<int, int> floors = Floors();

        #region Clicker Actions

        public static void ExitRoofCustomizationMenu()
        {
            if (verbose) Console.WriteLine("Exiting the roof customization menu");

            PressExitButton();
            Wait(1);
        }

        public static void CancelHurryConstruction()
        {
            if (verbose) Console.WriteLine("Exiting the construction menu");
            Click(100, 375); // Cancel action
            Wait(1);
        }

        public static void CloseAd()
        {
            if (verbose) Console.WriteLine("Closing the advertisement");

            var matchedImages = Clicker.matchedImages;
            if(matchedImages.ContainsKey("closeAd_7") || matchedImages.ContainsKey("closeAd_8"))
            {
                Click(22, 22);
                Click(311, 22);
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
            }
        }

        public static void PressQuestButton()
        {
            if (verbose) Console.WriteLine("Clicking on the quest button");
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
            else
            {
                Click(90, 440); // Skip the quest
                Wait(1);
                Click(230, 380); // Confirm
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

        public static void OpenTheGame()
        {
            Wait(1);
            Click(Clicker.matchedImages["gameIcon"]);
            Wait(10);
        }

        public static void CloseHiddenAd()
        {
            if (verbose) Console.WriteLine("Closing the hidden ad");
            Wait(1);
            Click(310, 10);
            Wait(1);
            Click(311, 22);
        }

        public static void CloseNewFloorMenu()
        {
            if (verbose) Console.WriteLine("Exiting");
            PressExitButton();
        }

        public static void CloseBuildNewFloorNotification()
        {
            if (verbose) Console.WriteLine("Closing the new floor notification");
            Click(105, 320); // Click no
        }

        public static void CompleteQuest()
        {
            if (verbose) Console.WriteLine("Completing the quest");
            Wait(1);
            Click(Clicker.matchedImages["completedQuestButton"]);
        }

        public static void WatchAd()
        {
            if (verbose) Console.WriteLine("Watching the advertisement");
            Click(225, 375);
            Wait(31);
        }

        public static void CheckBuildableFloor(int currentFloor, Image gameWindow)
        {
            Clicker.currentFloor = ConfigManager.GetConfig().FloorsNumber;
            int balance = TextRecognition.ParseBalance(gameWindow);

            if (balance != -1)
            {
                int newBalance = balance;
                if (currentFloor != 50)
                {
                    if (currentFloor > 37)
                    {
                        string temp = balance.ToString();
                        if (temp.Length > 3)
                        {
                            string s = temp.Remove(4);
                            string toMillions = s + "000";
                            newBalance = Convert.ToInt32(toMillions);
                        }
                    }

                    if (verbose) Console.WriteLine("Current balance: {0}", newBalance);
                    int targetPrice = floors[currentFloor + 1];
                    if (verbose) Console.WriteLine("Current goal: {0}", targetPrice);

                    if (targetPrice < newBalance && newBalance < 1900000) // Helps with incorrect balance detection
                    {
                        BuyFloor();
                    }
                }
            }
            if (currentFloor == 50)
            {
                RebuildTower();
            }
        }

        public static void BuyFloor()
        {
            if (verbose) Console.WriteLine("Building a new floor");
            MoveUp();
            Wait(2);
            Click(195, 390);
            Wait(1);

            if (MatchImage("buildNewFloorNotification"))
            {
                Click(230, 320);
                Wait(1);
                if (!MatchImage("newFloorNoCoinsNotification"))
                {
                    ConfigManager.AddNewFloor();
                    Console.WriteLine("Built a new floor");
                }
            }
        }

        public static void RebuildTower()
        {
            if (verbose) Console.WriteLine("Rebuilding the tower");
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
            Wait(5);
            ConfigManager.SaveNewFloor(1);
        }

        public static void PassTheTutorial()
        {
            if (verbose) Console.WriteLine("Passing the tutorial");
            Wait(5);
            Click(170, 435); // Continue
            Wait(3);
            MoveDown();
            Wait(1);
            Click(195, 260); // Build the new floor
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
            Click(240, 150); // Build residential
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
            ConfigManager.SaveNewFloor(3);
        }

        public static void RestartApp()
        {
            if (verbose) Console.WriteLine("Restarting the app");
            IntPtr mainHandle = GetProcess().MainWindowHandle;
            InputSim.SendMessage(mainHandle, InputSim.WM_LBUTTONDOWN, 1, MakeParam(98, 17));
            InputSim.SendMessage(mainHandle, InputSim.WM_LBUTTONUP, 0, MakeParam(98, 17));

            Wait(5);
        }

        public static void MoveUp()
        {
            if (verbose) Console.WriteLine("Moving up");
            Click(160, 8);
            Wait(1);
        }

        public static void MoveDown()
        {
            if (verbose) Console.WriteLine("Moving down");
            Click(230, 580);
        }

        public static void PressExitButton()
        {
            if (verbose) Console.WriteLine("Pressing the exit button");
            Click(305, 565);
        }

        public static int PlayRaffle(int currentHour)
        {
            if (currentHour != DateTime.Now.Hour)
            {
                if (verbose) Console.WriteLine("Playing the raffle");
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

        public static void PrintInfo()
        {
            Console.WriteLine(
                "TinyClicker build v0.459"+
                "\nCurrent config: Vip = {0}, Elevator Speed = {1} FPS, Number of floors = {2}"+
                "\n\nCommands:" +
                "\ns - Start TinyClicker" +
                "\nq - Quit" +
                "\nss - Capture and save a screenshot" +
                "\ncc - Create a new Config", 
                Clicker.currentConfig.VipPackage, 
                Clicker.currentConfig.ElevatorSpeed, 
                Clicker.currentConfig.FloorsNumber);
        }

        static void Wait(int seconds)
        {
            int milliseconds = seconds * 1000;
            Thread.Sleep(milliseconds);
        }

        static bool MatchImage(string imageKey)
        {
            // Returns true if image is found 

            Image gameWindow = MakeScreenshot();
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

                double minval, maxval, threshold = 0.7;
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

        public static void MatchImage(KeyValuePair<string, Mat> template, Mat reference)
        {
            Thread.Sleep(15); // Smooth the CPU load between templates
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
                    double minval, maxval, threshold = 0.78;
                    Point minloc, maxloc;
                    Cv2.MinMaxLoc(res, out minval, out maxval, out minloc, out maxloc);
                    res.Dispose();

                    if (maxval >= threshold)
                    {
                        if (!Clicker.matchedImages.ContainsKey(template.Key))
                        {
                            Clicker.matchedImages.Add(template.Key, MakeParam(maxloc.X, maxloc.Y));
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

        public static Process GetProcess()
        {
            Process[] processlist = Process.GetProcesses();
            foreach (Process process in processlist)
            {
                if (!string.IsNullOrEmpty(process.MainWindowTitle) && process.ProcessName == processName)
                {
                    //Console.WriteLine("Process: {0}", process.ProcessName);
                    //Console.WriteLine("ID: {0}", process.Id);
                    //Console.WriteLine("Title: {0} \n", process.MainWindowTitle);
                    return process;
                }
            }
            return null;
        }

        public static void Click(int location)
        {
            if (clickableChildHandle != IntPtr.Zero)
            {
                InputSim.SendMessage(clickableChildHandle, InputSim.WM_LBUTTONDOWN, 1, location);
                InputSim.SendMessage(clickableChildHandle, InputSim.WM_LBUTTONUP, 0, location);
            }
        }

        public static void Click(int x, int y)
        {
            if (clickableChildHandle != IntPtr.Zero)
            {
                InputSim.SendMessage(clickableChildHandle, InputSim.WM_LBUTTONDOWN, 1, MakeParam(x, y));
                InputSim.SendMessage(clickableChildHandle, InputSim.WM_LBUTTONUP, 0, MakeParam(x, y));
            }   
        }

        public static void PressEscape()
        {
            if (clickableChildHandle != IntPtr.Zero)
            {
                InputSim.SendMessage(clickableChildHandle, InputSim.WM_KEYDOWN, InputSim.VK_ESCAPE, 0);
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
                Console.WriteLine("LDPlayer process not found - TinyClicker function is not possible. Launch LDPlayer and restart the app.");
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
                if (!Directory.Exists(Environment.CurrentDirectory + @"\screenshots"))
                {
                    Directory.CreateDirectory(Environment.CurrentDirectory + @"\screenshots");
                }

                IntPtr handle = Process.GetProcessById(processId).MainWindowHandle;
                ScreenToImage sc = new ScreenToImage();

                // Captures screenshot of a window and saves it to screenshots folder

                sc.CaptureWindowToFile(handle, Environment.CurrentDirectory + @"\screenshots\window.png", ImageFormat.Png);
                Console.WriteLine(@"Made a screenshot. Screenshots can be found inside TinyClicker\screenshots folder");
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

            if (!Directory.Exists(statsPath))
            {
                Directory.CreateDirectory(statsPath);
            }

            ConfigManager.SaveNewRebuildTime(dateTimeNow);
            string data = "\n" + dateTimeNow.ToString() + $" - rebuilt the tower.\nHours since the last rebuild: {totalHours:0.00}\n";
            File.AppendAllText(statsPath, data);
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
                dict.Add("watchAdPromptBux", Image.FromFile(path + "watch_ad_prompt_bux.png"));
                dict.Add("continueButton", Image.FromFile(path + "continue_button.png"));
                dict.Add("newFloorNoCoinsNotification", Image.FromFile(path + "new_floor_no_coins_notification.png"));
                dict.Add("backButton", Image.FromFile(path + "back_button.png"));
                dict.Add("findBitizens", Image.FromFile(path + "find_bitizens.png"));
                dict.Add("newFloorMenu", Image.FromFile(path + "new_floor_menu.png"));
                dict.Add("buildNewFloorNotification", Image.FromFile(path + "build_new_floor_notification.png"));
                dict.Add("deliverBitizens", Image.FromFile(path + "deliver_bitizens.png"));
                dict.Add("freeBuxButton", Image.FromFile(path + "free_bux_button.png"));
                dict.Add("freeBuxVidoffersButton", Image.FromFile(path + "free_bux_vidoffers_button.png"));
                dict.Add("questButton", Image.FromFile(path + "quest_button.png"));
                dict.Add("completedQuestButton", Image.FromFile(path + "completed_quest_button.png"));
                dict.Add("gameIcon", Image.FromFile(path + "game_icon.png"));
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
                dict.Add("closeAd_9", Image.FromFile(path + "close_ad_button_9.png"));
                dict.Add("fullyStockedBonus", Image.FromFile(path + "fully_stocked_bonus.png"));
                dict.Add("hurryConstructionPrompt", Image.FromFile(path + "hurry_construction_prompt.png"));
                dict.Add("roofCustomizationWindow", Image.FromFile(path + "roof_customization_window.png"));
                dict.Add("giftChute", Image.FromFile(path + "gift_chute.png"));
                dict.Add("elevatorButton", Image.FromFile(path + "elevator_button.png"));

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
