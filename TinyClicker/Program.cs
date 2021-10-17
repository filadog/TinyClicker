﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Diagnostics;
using System.Threading;
using System.IO;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using Point = OpenCvSharp.Point;
using Size = OpenCvSharp.Size;

namespace TinyClicker
{
    internal class Program
    {
        public const string processName = "dnplayer";
        static IntPtr clickableChildHandle = FindClickableChildHandles(processName);
        static bool suspended = false;
        static int processId = GetProcessId(processName);

        // Matched images are stored in fields below. Cleaned every update cycle (~1000 ms)
        // string - image key, int - location within the target screen

        static Dictionary<string, int> matchedImages = new Dictionary<string, int>();
        static Dictionary<string, Image> images = FindImages();
        static Dictionary<string, Mat> templates = MakeTemplates(images);

        public static void Main()
        {
            Startup();
        }

        public static void Startup()
        {
            PrintInfo();
            string input = Console.ReadLine();
            switch (input)
            {
                case "s":
                    ClickerStart();
                    break;

                case "l":
                    PrintAllProcesses();
                    Main();
                    break;

                case "q":
                    Environment.Exit(0);
                    break;

                case "ss":
                    SaveScreenshot(GetProcessId(processName));
                    break;

                default:
                    Main();
                    break;
            }
        }

        public static void ClickerStart()
        {
            //PlayRaffle();
            //Thread.Sleep(10000);

            while (processId != -1 && !suspended)
            {
                MatchImages();
                // Perform all actions here

                foreach (var image in matchedImages)
                {
                    Console.WriteLine("Found {0}", image.Key);
                }
                PerformActions();

                Thread.Sleep(1000); // Object detection performed ~once a second
                matchedImages.Clear();
            }
        }

        static void MatchImages()
        {
            Image gameWindow = MakeScreenshot(processId);
            var windowBitmap = new Bitmap(gameWindow);
            gameWindow.Dispose();

            Mat reference = BitmapConverter.ToMat(windowBitmap);
            windowBitmap.Dispose();

            foreach (var template in templates)
            {
                //var imageBitmap = new Bitmap(image.Value);
                //Mat template = BitmapConverter.ToMat(imageBitmap);
                //imageBitmap.Dispose();
                //MatType.CV_32FC1
                using (Mat res = new Mat(reference.Rows - template.Value.Rows + 1, reference.Cols - template.Value.Cols + 1, MatType.CV_32FC1))
                {
                    //Convert input images to gray
                    Mat gref = reference.CvtColor(ColorConversionCodes.BGR2GRAY);
                    Mat gtpl = template.Value.CvtColor(ColorConversionCodes.BGR2GRAY);

                    Cv2.MatchTemplate(gref, gtpl, res, TemplateMatchModes.CCoeffNormed);
                    Cv2.Threshold(res, res, 0.7, 1.0, ThresholdTypes.Tozero);
                    GC.Collect();

                    while (!suspended)
                    {
                        double minval, maxval, threshold = 0.5; // default 0.5
                        Point minloc, maxloc;
                        Cv2.MinMaxLoc(res, out minval, out maxval, out minloc, out maxloc);

                        if (maxval >= threshold)
                        {
                            matchedImages.Add(template.Key, MakeParam(maxloc.X, maxloc.Y));
                            //Console.WriteLine(image.Key + " is found");
                            break;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
            GC.Collect(); // Never remove this!!!
        }

        public static void PerformActions()
        {
            //switch (MatchedImages.Keys)
            //{
            //    default:
            //        break;
            //}
            if (matchedImages.ContainsKey("freeBuxButton"))
            {
                // Perform free bux collection
                Click(matchedImages["freeBuxButton"]);
                Thread.Sleep(500);
                Click(230, 375);
                Thread.Sleep(500);
            }
            else if (matchedImages.ContainsKey("giftChute"))
            {
                Click(matchedImages["giftChute"]);
                Thread.Sleep(1000);
                Click(210, 375);
                Thread.Sleep(1000);
            }
            else if (matchedImages.ContainsKey("elevatorButton"))
            {
                // Perform lift ride
                Click(45, 535);
                //Click(MatchedImages["elevatorButton"]);
                Thread.Sleep(12000);
                Click(240, 575); // Click at the bottom pane to return to the ground level
                Thread.Sleep(1000);
            }
            
            
            //suelse ifspended = true;
            //switch (key)
            //{
            //    case "moveIn":

            //        Console.WriteLine("Move in is found");
            //        Click(100, 375);
            //        Thread.Sleep(500);
            //        Click(245, 415);
            //        Thread.Sleep(500);
            //        Click(220, 380);
            //        Thread.Sleep(500);
            //        break;

            //    case "freeBuxCollectButton":

            //        Click(location);
            //        Thread.Sleep(500);
            //        break;

            //    case "elevatorButton":

            //        Console.WriteLine("Elevator is found");
            //        Click(location);
            //        Thread.Sleep(13000);
            //        break;
                
            //    case "freeBuxButton":

            //        Click(location);
            //        Thread.Sleep(500);
            //        Click(230, 375);
            //        Thread.Sleep(500);
            //        break;

            //    default:
            //        Console.WriteLine("Action is not implemented yet : {0}", key);
            //        break;
            //}

            //suspended = false;
            //ClickerStart();
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

        public static Dictionary <string, Mat> MakeTemplates(Dictionary<string, Image> images)
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
            return mats;
        }

        public static void Click(int location)
        {
            MouseClickSimulator.SendMessage(clickableChildHandle, MouseClickSimulator.WM_LBUTTONDOWN, 1, location);
            MouseClickSimulator.SendMessage(clickableChildHandle, MouseClickSimulator.WM_LBUTTONUP, 0, location);
        }

        public static void Click(int x, int y)
        {
            MouseClickSimulator.SendMessage(clickableChildHandle, MouseClickSimulator.WM_LBUTTONDOWN, 1, MakeParam(x, y));
            MouseClickSimulator.SendMessage(clickableChildHandle, MouseClickSimulator.WM_LBUTTONUP, 0, MakeParam(x, y));
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

        public static Image MakeScreenshot(int processId)
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

        public static void SaveScreenshot(int processId)
        {
            IntPtr handle = Process.GetProcessById(processId).MainWindowHandle;
            ScreenToImage sc = new ScreenToImage();

            // Captures screenshot of a window and saves it to screenshots folder

            sc.CaptureWindowToFile(handle, Environment.CurrentDirectory + "\\screenshots\\mainWindow.png", ImageFormat.Png);
            Console.WriteLine("Made a screenchot you bastard");
            Main();
        }

        static void PrintAllProcesses()
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

        static int GetProcessId(string processName)
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

        static Dictionary<string, Image> FindImages()
        {
            var dict = new Dictionary<string, Image>();
            string samplesPath = Path.Combine(Environment.CurrentDirectory, "samples\\");

            dict.Add("menuButton", Image.FromFile(samplesPath + "menu_button.png"));
            dict.Add("backButton", Image.FromFile(samplesPath + "back_button.png"));
            dict.Add("questButton", Image.FromFile(samplesPath + "quest_button.png"));
            dict.Add("elevatorButton", Image.FromFile(samplesPath + "elevator_button.png"));
            dict.Add("vipButton", Image.FromFile(samplesPath + "vip_button.png"));
            dict.Add("freeBuxButton", Image.FromFile(samplesPath + "free_bux_button.png"));
            dict.Add("freeBuxCollectButton", Image.FromFile(samplesPath + "free_bux_collect_button.png"));
            dict.Add("freeBuxVidoffersButton", Image.FromFile(samplesPath + "free_bux_vidoffers_button.png"));
            dict.Add("raffleIconMenu", Image.FromFile(samplesPath + "raffle_icon_menu.png"));
            dict.Add("enterRaffleButton", Image.FromFile(samplesPath + "enter_raffle_button.png"));
            dict.Add("rushAllButton", Image.FromFile(samplesPath + "rush_all_button.png"));
            dict.Add("stockAllButton", Image.FromFile(samplesPath + "stock_all_button.png"));
            dict.Add("giftChute", Image.FromFile(samplesPath + "gift_chute.png"));
            dict.Add("moveIn", Image.FromFile(samplesPath + "move_in.png"));

            return dict;
        }

        static void PrintInfo()
        {
            Console.WriteLine("TinyClicker build v0.02" +
                "\nCommands:" +
                "\ns - Enable clicker" +
                "\nl - Display all processes" +
                "\nq - Quit the application" +
                "\nss - Capture and save a screenshot");
        }
    }
}