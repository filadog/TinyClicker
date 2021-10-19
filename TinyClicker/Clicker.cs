using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using Point = OpenCvSharp.Point;

namespace TinyClicker
{
    internal static class Clicker
    {
        static bool suspended = false;

        static Dictionary<string, int> matchedImages = new Dictionary<string, int>();
        static Dictionary<string, Image> images = Actions.FindImages();
        static Dictionary<string, Mat> templates = Actions.MakeTemplates(images);

        public static void StartClicker()
        {
            //PlayRaffle();
            //Thread.Sleep(10000);
            int processId = Actions.processId;

            while (processId != -1 && !suspended)
            {
                MatchImages();
                string dateTimeNow = DateTime.Now.ToString("HH:mm:ss");
                foreach (var image in matchedImages)
                {
                    Console.WriteLine(dateTimeNow + " Found {0}", image.Key);
                }
                if (matchedImages.Count == 0)
                {
                    Console.WriteLine(dateTimeNow + " Nothing was found");
                }
                PerformActions();
                Thread.Sleep(1000); // Object detection performed ~once a second
                matchedImages.Clear();
            }
        }

        static void MatchImages()
        {
            Image gameWindow = Actions.MakeScreenshot();
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
                using (Mat res = new(reference.Rows - template.Value.Rows + 1, reference.Cols - template.Value.Cols + 1, MatType.CV_32FC1))
                {
                    //Convert input images to gray
                    Mat gref = reference.CvtColor(ColorConversionCodes.BGR2GRAY);
                    Mat gtpl = template.Value.CvtColor(ColorConversionCodes.BGR2GRAY);

                    Cv2.MatchTemplate(gref, gtpl, res, TemplateMatchModes.CCoeffNormed);
                    Cv2.Threshold(res, res, 0.7, 1.0, ThresholdTypes.Tozero);
                    GC.Collect();

                    while (!suspended)
                    {
                        double minval, maxval, threshold = 0.78; // default 0.5
                        Point minloc, maxloc;
                        Cv2.MinMaxLoc(res, out minval, out maxval, out minloc, out maxloc);

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
            GC.Collect(); // Never remove this!!!
        }

        static bool MatchImage(string imageKey)
        {
            // Returns true if image is found 

            Image gameWindow = Actions.MakeScreenshot();
            var windowBitmap = new Bitmap(gameWindow);
            gameWindow.Dispose();

            Mat reference = BitmapConverter.ToMat(windowBitmap);
            windowBitmap.Dispose();

            var template = templates[imageKey];
            using (Mat res = new Mat(reference.Rows - template.Rows + 1, reference.Cols - template.Cols + 1, MatType.CV_32FC1))
            {
                //Convert input images to gray
                Mat gref = reference.CvtColor(ColorConversionCodes.BGR2GRAY);
                Mat gtpl = template.CvtColor(ColorConversionCodes.BGR2GRAY);

                Cv2.MatchTemplate(gref, gtpl, res, TemplateMatchModes.CCoeffNormed);
                Cv2.Threshold(res, res, 0.7, 1.0, ThresholdTypes.Tozero);

                double minval, maxval, threshold = 0.5; // default 0.5
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

        static void PerformActions()
        {
            // TODO: Refactor this abomination

            foreach (var key in matchedImages.Keys)
            {
                switch (key)
                {
                    case "roofCustomizationWindow":
                        Actions.ExitRoofCustomizationMenu();
                        //Click(305, 565); // Exit the menu
                        //Thread.Sleep(1000);
                        break;

                    default:
                        break;
                }
            }


            //if (matchedImages.ContainsKey("roofCustomizationWindow"))
            //{
            //    Click(305, 565); // Exit the menu
            //    Thread.Sleep(1000);
            //}
            if (matchedImages.ContainsKey("hurryConstructionPrompt"))
            {
                Actions.Click(100, 375); // Cancel
                Thread.Sleep(1000);
            }
            if (matchedImages.ContainsKey("closeAdButton"))
            {
                Actions.Click(311, 22); // Close ad (x)
                Thread.Sleep(2000);
                return;
            }
            if (matchedImages.ContainsKey("closeAdButton_2"))
            {
                Actions.Click(311, 22); // Close ad (x)
                Thread.Sleep(2000);
                return;
            }
            if (matchedImages.ContainsKey("closeAdButton_3"))
            {
                Actions.Click(311, 22); // Close ad (x)
                Thread.Sleep(2000);
                return;
            }
            if (matchedImages.ContainsKey("closeAdButton_4"))
            {
                Actions.Click(311, 22);
                Thread.Sleep(2000);
                return;
            }
            if (matchedImages.ContainsKey("closeAdButton_5"))
            {
                Actions.Click(311, 22);
                Thread.Sleep(2000);
                return;
            }
            if (matchedImages.ContainsKey("closeAdButton_6"))
            {
                Actions.Click(311, 22);
                Thread.Sleep(2000);
                return;
            }
            if (matchedImages.ContainsKey("closeAdButton_7")) // Probably incorrect click place
            {
                Actions.Click(311, 22);
                Thread.Sleep(2000);
                return;
            }
            if (matchedImages.ContainsKey("closeAdButton_8")) // Probably incorrect click place
            {
                Actions.Click(311, 22);
                Thread.Sleep(2000);
                return;
            }

            if (matchedImages.ContainsKey("continueButton"))
            {
                Actions.Click(matchedImages["continueButton"]);
                Thread.Sleep(1000);
                Actions.Click(160, 8); // Go up
                return;
            }
            if (matchedImages.ContainsKey("foundCoinsChuteNotification"))
            {
                Console.WriteLine("Closing the notification");
                Actions.Click(165, 375); // Close the notification
                return;
            }
            if (matchedImages.ContainsKey("restockButton"))
            {
                Actions.Click(230, 580); // Click to go down
                Thread.Sleep(1500);
                Actions.Click(100, 480); // Click STOCK ALL
                Thread.Sleep(1000);
                Actions.Click(225, 375); // Yes
                Thread.Sleep(1500);
                if (MatchImage("fullyStockedBonus"))
                {
                    Actions.Click(165, 375); // Close the bonus tooltip
                    Thread.Sleep(1000);
                    Actions.Click(160, 8); // Go up
                    Thread.Sleep(800);
                    return;
                }
                else
                {
                    Actions.Click(160, 8); // Go up
                    Thread.Sleep(800);
                    return;
                }

                // Add a check for bonus of fully stocked floors
            }

            if (matchedImages.ContainsKey("freeBuxButton"))
            {
                // Perform free bux collection
                Actions.Click(matchedImages["freeBuxButton"]);
                Thread.Sleep(500);
                Actions.Click(230, 375);
                Thread.Sleep(500);
                return;
            }
            if (matchedImages.ContainsKey("giftChute"))
            {
                Actions.Click(matchedImages["giftChute"]);
                Thread.Sleep(1500);

                if (MatchImage("watchAdPromptBux"))
                {
                    Actions.Click(220, 380); // Continue
                    Thread.Sleep(40500);
                    Console.WriteLine("Watched the ad. Looking for a close button");
                    MatchImages();
                    if (matchedImages.ContainsKey("closeAdButton"))
                    {
                        Actions.Click(311, 22);
                        Thread.Sleep(2000);
                        return;
                    }
                    if (matchedImages.ContainsKey("closeAdButton_2"))
                    {
                        Actions.Click(311, 22);
                        Thread.Sleep(2000);
                        return;
                    }
                    if (matchedImages.ContainsKey("closeAdButton_3"))
                    {
                        Actions.Click(311, 22);
                        Thread.Sleep(2000);
                        return;
                    }
                    if (matchedImages.ContainsKey("closeAdButton_4"))
                    {
                        Actions.Click(311, 22);
                        Thread.Sleep(2000);
                        return;
                    }
                    if (matchedImages.ContainsKey("closeAdButton_5"))
                    {
                        Actions.Click(311, 22);
                        Thread.Sleep(2000);
                        return;
                    }
                    if (matchedImages.ContainsKey("closeAdButton_6"))
                    {
                        Actions.Click(311, 22);
                        Thread.Sleep(2000);
                        return;
                    }
                    if (matchedImages.ContainsKey("closeAdButton_7")) // Probably incorrect click place
                    {
                        Actions.Click(311, 22);
                        Thread.Sleep(2000);
                        return;
                    }
                    if (matchedImages.ContainsKey("closeAdButton_8")) // Probably incorrect click place
                    {
                        Actions.Click(311, 22);
                        Thread.Sleep(2000);
                        return;
                    }
                    else
                    {
                        Actions.Click(311, 22); // Close ad (x)
                        Thread.Sleep(2000);
                        return;
                    }
                }
                if (MatchImage("watchAdPromptCoins"))
                {
                    Actions.Click(220, 380); // Continue
                    Thread.Sleep(40500);
                    Console.WriteLine("Watched the ad. Looking for a close button");
                    MatchImages();
                    if (matchedImages.ContainsKey("closeAdButton"))
                    {
                        Actions.Click(311, 22);
                        Thread.Sleep(2000);
                        return;
                    }
                    if (matchedImages.ContainsKey("closeAdButton_2"))
                    {
                        Actions.Click(311, 22);
                        Thread.Sleep(2000);
                        return;
                    }
                    if (matchedImages.ContainsKey("closeAdButton_3"))
                    {
                        Actions.Click(311, 22);
                        Thread.Sleep(2000);
                        return;
                    }
                    if (matchedImages.ContainsKey("closeAdButton_4"))
                    {
                        Actions.Click(311, 22);
                        Thread.Sleep(2000);
                        return;
                    }
                    if (matchedImages.ContainsKey("closeAdButton_5"))
                    {
                        Actions.Click(311, 22);
                        Thread.Sleep(2000);
                        return;
                    }
                    if (matchedImages.ContainsKey("closeAdButton_6"))
                    {
                        Actions.Click(311, 22);
                        Thread.Sleep(2000);
                        return;
                    }
                    if (matchedImages.ContainsKey("closeAdButton_7")) // Probably incorrect click place
                    {
                        Actions.Click(311, 22);
                        Thread.Sleep(2000);
                        return;
                    }
                    if (matchedImages.ContainsKey("closeAdButton_8")) // Probably incorrect click place
                    {
                        Actions.Click(311, 22);
                        Thread.Sleep(2000);
                        return;
                    }
                    else
                    {
                        Actions.Click(311, 22); // Close ad (x)
                        Thread.Sleep(2000);
                        return;
                    }
                }
                return;
            }
            if (matchedImages.ContainsKey("elevatorButton"))
            {
                // Perform lift ride
                Actions.Click(45, 535);
                //Click(MatchedImages["elevatorButton"]);
                Thread.Sleep(2000); // Default 12000 (when floors > 80)
                if (MatchImage("giftChute"))
                {
                    return;
                }
                Actions.Click(160, 8); // Click to go up
                Thread.Sleep(1000);
                return;
            }
        }
    }
}
