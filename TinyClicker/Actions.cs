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
        public const string processName = "dnplayer";
        static IntPtr clickableChildHandle = FindClickableChildHandles(processName);
        public static int processId = GetProcessId();


        public static void ExitRoofCustomizationMenu()
        {
            PressExitButton();
            Wait(1);
        }

        static void PressExitButton()
        {
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

        #region Utility Methods

        public static void PrintInfo()
        {
            Console.WriteLine(
                "TinyClicker build v0.063" +
                "\nCommands:" +
                "\ns - Enable clicker" +
                "\nl - Display all processes" +
                "\nq - Quit the application" +
                "\nss - Capture and save a screenshot");
        }

        static void Wait(int seconds)
        {
            int milliseconds = seconds * 1000;
            Thread.Sleep(milliseconds);
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
            MouseSim.SendMessage(clickableChildHandle, MouseSim.WM_LBUTTONDOWN, 1, location);
            MouseSim.SendMessage(clickableChildHandle, MouseSim.WM_LBUTTONUP, 0, location);
        }

        public static void Click(int x, int y)
        {
            MouseSim.SendMessage(clickableChildHandle, MouseSim.WM_LBUTTONDOWN, 1, MakeParam(x, y));
            MouseSim.SendMessage(clickableChildHandle, MouseSim.WM_LBUTTONUP, 0, MakeParam(x, y));
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
                Console.WriteLine("No clickable child found - clicker function is not possible");
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

            IntPtr handle = Process.GetProcessById(processId).MainWindowHandle;
            ScreenToImage sc = new ScreenToImage();

            // Captures screenshot of a window and saves it to screenshots folder

            sc.CaptureWindowToFile(handle, Environment.CurrentDirectory + "\\screenshots\\mainWindow.png", ImageFormat.Png);
            Console.WriteLine("Made a screenchot you bastard");
        }

        public static void PrintAllProcesses()
        {
            Process[] processlist = Process.GetProcesses();
            foreach (Process process in processlist)
            {
                // If the process appears on the Taskbar (if has a title)
                // print the information of the process
                if (!String.IsNullOrEmpty(process.MainWindowTitle))
                {
                    Console.WriteLine("Process:   {0}", process.ProcessName);
                    Console.WriteLine("    ID   : {0}", process.Id);
                    Console.WriteLine("    Title: {0} \n", process.MainWindowTitle);
                }
            }
        }

        public static Dictionary<string, Image> FindImages()
        {
            var dict = new Dictionary<string, Image>();
            string samplesPath = Path.Combine(Environment.CurrentDirectory, "samples\\");

            //dict.Add("menuButton", Image.FromFile(samplesPath + "menu_button.png"));
            dict.Add("backButton", Image.FromFile(samplesPath + "back_button.png"));
            //dict.Add("questButton", Image.FromFile(samplesPath + "quest_button.png"));
            dict.Add("elevatorButton", Image.FromFile(samplesPath + "elevator_button.png"));
            dict.Add("vipButton", Image.FromFile(samplesPath + "vip_button.png"));
            dict.Add("freeBuxButton", Image.FromFile(samplesPath + "free_bux_button.png"));
            dict.Add("freeBuxCollectButton", Image.FromFile(samplesPath + "free_bux_collect_button.png"));
            dict.Add("freeBuxVidoffersButton", Image.FromFile(samplesPath + "free_bux_vidoffers_button.png"));
            dict.Add("raffleIconMenu", Image.FromFile(samplesPath + "raffle_icon_menu.png"));
            dict.Add("enterRaffleButton", Image.FromFile(samplesPath + "enter_raffle_button.png"));
            //dict.Add("rushAllButton", Image.FromFile(samplesPath + "rush_all_button.png"));
            //dict.Add("stockAllButton", Image.FromFile(samplesPath + "stock_all_button.png"));
            dict.Add("giftChute", Image.FromFile(samplesPath + "gift_chute.png"));
            dict.Add("moveIn", Image.FromFile(samplesPath + "move_in.png"));
            dict.Add("restockButton", Image.FromFile(samplesPath + "restock_button.png"));

            dict.Add("deliverBitizens", Image.FromFile(samplesPath + "deliver_bitizens.png"));
            dict.Add("findBitizens", Image.FromFile(samplesPath + "find_bitizens.png"));

            dict.Add("foundCoinsChuteNotification", Image.FromFile(samplesPath + "found_coins_chute_notification.png"));
            dict.Add("watchAdPromptBux", Image.FromFile(samplesPath + "watch_ad_prompt_bux.png"));
            dict.Add("watchAdPromptCoins", Image.FromFile(samplesPath + "watch_ad_prompt_coins.png"));
            dict.Add("closeAdButton", Image.FromFile(samplesPath + "close_ad_button.png"));
            dict.Add("closeAdButton_2", Image.FromFile(samplesPath + "close_ad_button_2.png"));
            dict.Add("closeAdButton_3", Image.FromFile(samplesPath + "close_ad_button_3.png"));
            dict.Add("closeAdButton_4", Image.FromFile(samplesPath + "close_ad_button_4.png"));
            dict.Add("closeAdButton_5", Image.FromFile(samplesPath + "close_ad_button_5.png"));
            dict.Add("closeAdButton_6", Image.FromFile(samplesPath + "close_ad_button_6.png"));
            dict.Add("closeAdButton_7", Image.FromFile(samplesPath + "close_ad_button_7.png"));
            dict.Add("closeAdButton_8", Image.FromFile(samplesPath + "close_ad_button_8.png"));
            dict.Add("fullyStockedBonus", Image.FromFile(samplesPath + "fully_stocked_bonus.png"));
            dict.Add("continueButton", Image.FromFile(samplesPath + "continue_button.png"));
            dict.Add("hurryConstructionPrompt", Image.FromFile(samplesPath + "hurry_construction_prompt.png"));
            dict.Add("roofCustomizationWindow", Image.FromFile(samplesPath + "roof_customization_window.png"));

            return dict;
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
                //template.Dispose();
            }
            images.Clear();
            return mats;
        }

        #endregion
    }
}
