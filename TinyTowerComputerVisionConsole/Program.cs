using System;
using System.Collections.Generic;
using System.Windows;
using System.Drawing;
using System.Drawing.Imaging;
using System.Diagnostics;
using System.Threading;
using System.IO;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using Point = OpenCvSharp.Point;
using Size = OpenCvSharp.Size;

namespace TinyTowerComputerVisionConsole
{
    internal class Program
    {
        public const string processName = "dnplayer";
        static IntPtr clickableChildHandle = FindClickableChildHandles(processName);
        static bool suspended = false;

        public static void Main()
        {
            Console.WriteLine("Commands:" +
                "\nstart - enable clicker" +
                "\nl - display all processes" +
                "\nq - quit application");

            string input = Console.ReadLine();
            switch (input)
            {
                case "start":
                    ClickerStart();
                    break;

                case "l":
                    PrintAllProcesses();
                    Main();
                    break;

                case "q":
                    Environment.Exit(0);
                    break;

                default:
                    Main();
                    break;
            }
        }

        public static void ClickerStart()
        {
            var images = FindImages();
            int processId = GetProcessId(processName);

            MatchImages(processId, images); 
        }

        public static void PerformAction(string key, int location)
        {
            suspended = true;
            switch (key)
            {
                case "elevatorButton":
                    Click(location);
                    Thread.Sleep(10000);
                    Click(100, 375);
                    Thread.Sleep(500);
                    Click(245, 415);
                    Thread.Sleep(500);
                    Click(220, 380);
                    Thread.Sleep(500);
                    break;

                default:
                    Console.WriteLine("Action is not implemented yet");
                    break;
            }

            suspended = false;
            ClickerStart();
        }

        static void MatchImages(int processId, Dictionary<string, Image> images)
        {
            while (processId != -1 && !suspended)
            {
                Thread.Sleep(1000); // Object detection performed twice a second
                Image gameWindow = MakeScreenshot(processId);

                var windowBitmap = new Bitmap(gameWindow);
                gameWindow.Dispose();

                Mat reference = BitmapConverter.ToMat(windowBitmap);
                windowBitmap.Dispose();

                foreach (var image in images)
                {
                    var imageBitmap = new Bitmap(image.Value);
                    Mat template = BitmapConverter.ToMat(imageBitmap);
                    imageBitmap.Dispose();

                    using (Mat res = new Mat(reference.Rows - template.Rows + 1, reference.Cols - template.Cols + 1, MatType.CV_32FC1))
                    {
                        //Convert input images to gray
                        Mat gref = reference.CvtColor(ColorConversionCodes.BGR2GRAY);
                        Mat gtpl = template.CvtColor(ColorConversionCodes.BGR2GRAY);

                        Cv2.MatchTemplate(gref, gtpl, res, TemplateMatchModes.CCoeffNormed);
                        Cv2.Threshold(res, res, 0.8, 1.0, ThresholdTypes.Tozero);
                        GC.Collect();

                        while (true && !suspended)
                        {
                            double minval, maxval, threshold = 0.8;
                            Point minloc, maxloc;
                            Cv2.MinMaxLoc(res, out minval, out maxval, out minloc, out maxloc);

                            if (maxval >= threshold)
                            {
                                if (image.Key == "elevatorButton")
                                {
                                    PerformAction(image.Key, MakeParam(maxloc.X, maxloc.Y));
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
            }
            GC.Collect(); // Never remove this!!!
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
                Console.WriteLine("No clickable child found - clicker function not possible");
                return IntPtr.Zero;
            } 
        }

        public static Image MakeScreenshot(int processId)
        {
            if (processId != -1)
            {
                IntPtr handle = Process.GetProcessById(processId).MainWindowHandle;

                ScreenCapture sc = new ScreenCapture();

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

            return dict;
        }
    }
}
