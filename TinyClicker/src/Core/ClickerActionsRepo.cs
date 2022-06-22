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
using ImageMagick;
using System.Windows;
using System.Linq;
using System.Windows.Controls.Primitives;

namespace TinyClicker;

public class ClickerActionsRepo
{
    readonly TinyClickerApp _clickerApp;
    readonly ScreenshotManager _screenshotManager;
    readonly ConfigManager _configManager;
    readonly ImageToText _imageToText;
    readonly ImageEditor _imageEditor;

    const string _ldPlayerProcName = "dnplayer";
    const string _blueStacksProcName = "HD-Player";

    //static IntPtr _clickableChildHandle = new IntPtr(136866);
    internal Process _process;
    public int _processId;
    readonly IntPtr _childHandle;
    Dictionary<int, int> _floorPrices;
    readonly MainWindow _mainWindow;
    DateTime _timeForNewFloor;
    Rectangle _screenRect;

    public ClickerActionsRepo(TinyClickerApp clickerApp, ConfigManager configManager)
    {
        _clickerApp = clickerApp;
        _configManager = configManager;
        _mainWindow = Application.Current.Windows.OfType<MainWindow>().First();
        _process = GetProcess();
        _processId = _process.Id;
        _childHandle = GetChildHandle();
        _timeForNewFloor = DateTime.Now;
        _screenRect = InputSimulator.GetWindowRectangle(_childHandle);
        _screenshotManager = new ScreenshotManager();
        _imageEditor = new ImageEditor(_screenRect);
        _imageToText = new ImageToText(_imageEditor);
    }

    #region Clicker Actions

    public void CancelHurryConstruction()
    {
        _mainWindow.Log("Exiting the construction menu");

        SendClick(100, 375); // Cancel action
        Wait(1);
    }

    public void CollectFreeBux()
    {
        _mainWindow.Log("Collecting free bux");
        SendClick(_clickerApp._matchedTemplates["freeBuxCollectButton"]);
    }

    public void ClickOnChute()
    {
        _mainWindow.Log("Clicking on the parachute");
        SendClick(_clickerApp._matchedTemplates["giftChute"]);
        Wait(1);
        if (IsImageFound("watchAdPromptCoins") && _configManager._curConfig.CurrentFloor >= _clickerApp._floorToStartWatchingAds)
        {
            WatchCoinsAds();
        }
        else if (_clickerApp._acceptBuxVideoOffers && _configManager._curConfig.CurrentFloor >= _clickerApp._floorToStartWatchingAds)
        {
            if (IsImageFound("watchAdPromptBux"))
            {
                WatchBuxAds();
            }
        }
        else
        {
            SendClick(105, 380); // Decline the video offer
        }
    }

    public void CloseAd()
    {
        _mainWindow.Log("Closing the advertisement");

        if (_clickerApp._matchedTemplates.ContainsKey("closeAd_7") || _clickerApp._matchedTemplates.ContainsKey("closeAd_8"))
        {
            SendClick(22, 22);
            SendClick(311, 22);
            SendClick(302, 52);
        }
        else
        {
            SendClick(311, 22);
            SendClick(22, 22);
            SendClick(302, 52);
        }

        CheckForLostAdsReward();
    }

    public void CloseChuteNotification()
    {
        _mainWindow.Log("Closing the parachute notification");
        SendClick(165, 375); // Close the notification
    }

    public void ExitRoofCustomizationMenu()
    {
        _mainWindow.Log("Exiting the roof customization menu");
        PressExitButton();
        Wait(1);
    }

    public void PressContinue()
    {
        _mainWindow.Log("Clicking continue");

        SendClick(_clickerApp._matchedTemplates["continueButton"]);
        Wait(1);
        MoveUp();
    }

    public void Restock()
    {
        _mainWindow.Log("Restocking");
        MoveDown();
        Wait(1);
        SendClick(100, 480); // Stock all
        Wait(1);
        SendClick(225, 375);
        Wait(1);

        if (IsImageFound("fullyStockedBonus"))
        {
            SendClick(165, 375); // Close the bonus tooltip
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

    public void PressFreeBuxButton()
    {
        _mainWindow.Log("Pressing free bux icon");
        SendClick(25, 130);
        Wait(1);
        SendClick(230, 375);
        Wait(1);
    }
    
    public void RideElevator()
    {
        _mainWindow.Log("Riding the elevator");
        SendClick(45, 535);
        int curFloor = _configManager._curConfig.CurrentFloor;
        if (curFloor >= 43)
            Wait(5);
        else if (curFloor >= 33)
            Wait(4);
        else if (curFloor >= 23)
            Wait(3);
        else if (curFloor >= 13)
            Wait(2);
        else
            Wait(1);

        if (IsImageFound("giftChute"))
        {
            return;
        }
        else
        {
            MoveUp();
        }
    }

    public void PressQuestButton()
    {
        _mainWindow.Log("Clicking on the quest button");
        SendClick(_clickerApp._matchedTemplates["questButton"]);
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
            SendClick(90, 440); // Skip the quest
            Wait(1);
            SendClick(230, 380); // Confirm
        }
    }

    public void FindBitizens()
    {
        _mainWindow.Log("Skipping the quest");
        SendClick(95, 445); // Skip the quest
        Wait(1);
        SendClick(225, 380); // Confirm skip
    }

    public void DeliverBitizens()
    {
        _mainWindow.Log("Delivering bitizens");
        SendClick(230, 440); // Continue
    }

    public void OpenTheGame()
    {
        Wait(1);
        SendClick(_clickerApp._matchedTemplates["gameIcon"]);
        Wait(10);
    }

    public void CloseHiddenAd()
    {
        _mainWindow.Log("Closing the hidden ad");
        Wait(1);
        SendClick(310, 10);
        Wait(1);
        SendClick(311, 22);
        CheckForLostAdsReward();
    }

    public void CheckForLostAdsReward()
    {
        Wait(1);
        if (IsImageFound("adsLostReward"))
        {
            SendClick(240, 344); // Click "Keep watching"
            Wait(15);
        }
        else
        {
            SendClick(240, 344);
        }
    }

    public void CloseNewFloorMenu()
    {
        _mainWindow.Log("Exiting");
        PressExitButton();
    }

    public void CloseBuildNewFloorNotification()
    {
        _mainWindow.Log("Closing the new floor notification");
        SendClick(105, 320); // Click no
    }

    public void CompleteQuest()
    {
        _mainWindow.Log("Completing the quest");
        
        Wait(1);
        SendClick(_clickerApp._matchedTemplates["completedQuestButton"]);
    }

    public void WatchCoinsAds()
    {
        _mainWindow.Log("Watching the advertisement");
        SendClick(225, 375);
        Wait(20);
    }

    public void WatchBuxAds()
    {
        _mainWindow.Log("Watching the advertisement");
        SendClick(225, 375);
        Wait(20);
    }

    public void CheckForNewFloor(int currentFloor, Image gameWindow)
    {
        //_clickerApp._currentFloor = ConfigManager.GetConfig().CurrentFloor;
        _floorPrices = CalculateFloorPrices();
        int balance = _imageToText.ParseBalance(gameWindow);

        if (balance != 0 && balance != -1 && currentFloor >= 3)
        {
            int targetPrice = _floorPrices[currentFloor + 1];
            if (balance > targetPrice && currentFloor < _clickerApp._floorToRebuildAt)
            {
                BuildNewFloor();
            }
            if (currentFloor >= _clickerApp._floorToRebuildAt)
            {
                Wait(1);
                RebuildTower();
            }
        }
    }

    public void BuildNewFloor()
    {
        if (_timeForNewFloor <= DateTime.Now)
        {
            _mainWindow.Log("Building a new floor");
            MoveUp();
            Wait(2);
            SendClick(195, 390); // Click on a new floor
            Wait(1);

            if (IsImageFound("buildNewFloorNotification"))
            {
                SendClick(230, 320); // Confirm construction
                Wait(1);
                // Add a new floor if there is enough coins
                if (!IsImageFound("newFloorNoCoinsNotification"))
                {
                    _configManager.AddOneFloor();
                    _mainWindow.Log("Built a new floor");
                }
                else
                {
                    // Cooldown 30s in case building fails (to prevent repeated attempts)
                    _timeForNewFloor = DateTime.Now.AddSeconds(30);
                    _mainWindow.Log("Not enough coins for a new floor");
                }
            }
            MoveUp();
        }
        else
        {
            _mainWindow.Log("Too early to rebuild the floor");
            Wait(1);
        }
    }

    public void RebuildTower()
    {
        _mainWindow.Log("Rebuilding the tower");
        SaveStatRebuildTime();
        SendClick(305, 570);
        Wait(1);
        SendClick(165, 435);
        Wait(1);
        SendClick(165, 440);
        Wait(1);
        SendClick(230, 380);
        Wait(1);
        SendClick(230, 380);
        Wait(3);
        _configManager.ChangeCurrentFloor(1);
    }

    public void PassTheTutorial()
    {
        _mainWindow.Log("Passing the tutorial");
        Wait(3);
        SendClick(170, 435); // Continue
        Wait(3);
        MoveDown();
        Wait(1);
        SendClick(195, 260); // Build a new floor
        Wait(1);
        SendClick(230, 380); // Confirm
        Wait(1);
        SendClick(20, 60); // Complete quest
        Wait(1);
        SendClick(170, 435); // Collect bux
        Wait(1);
        SendClick(170, 435); // Continue
        Wait(1);
        SendClick(190, 300); // Click on new floor
        Wait(1);
        SendClick(240, 150); // Build a residential floor
        Wait(1);
        SendClick(160, 375); // Continue
        Wait(1);
        SendClick(20, 60); // Complete quest
        Wait(1);
        SendClick(170, 435); // Collect bux
        Wait(1);
        SendClick(170, 435); // Continue
        Wait(1);
        SendClick(30, 535); // Ride elevator
        Wait(5);
        SendClick(230, 380); // Continue
        Wait(1);
        SendClick(20, 60); // Complete quest
        Wait(1);
        SendClick(170, 435); // Collect bux
        Wait(1);
        SendClick(170, 435); // Continue
        Wait(1);
        SendClick(190, 200); // Build new floor
        Wait(1);
        SendClick(225, 380); // Confirm
        Wait(1);
        SendClick(200, 200); // Open the new floor
        Wait(1);
        SendClick(90, 340); // Build random food
        Wait(1);
        SendClick(170, 375); // Continue
        Wait(1);
        SendClick(20, 60); // Complete quest
        Wait(1);
        SendClick(170, 435); // Collect bux
        Wait(1);
        SendClick(170, 435); // Continue
        Wait(1);
        SendClick(200, 200); // Open food floor
        Wait(1);
        SendClick(75, 210); // Hire
        Wait(1);
        SendClick(80, 100); // Select our bitizen
        Wait(1);
        SendClick(230, 380); // Hire him
        Wait(1);
        SendClick(160, 380); // Continue on dream job assignement
        Wait(1);
        SendClick(300, 560); // Exit the food store
        Wait(1);
        SendClick(20, 60); // Complete quest
        Wait(1);
        SendClick(170, 435); // Collect bux
        Wait(1);
        SendClick(170, 435); // Continue
        Wait(1);
        SendClick(200, 200); // Open food store again
        Wait(1);
        SendClick(200, 210); // Restock first item in the store
        Wait(15);
        SendClick(305, 190); // Restock
        Wait(1);
        SendClick(20, 60); // Complete quest
        Wait(1);
        SendClick(170, 435); // Collect bux
        Wait(1);
        SendClick(170, 435); // Continue
        Wait(1);
        SendClick(200, 200); // Open food store again
        Wait(1);
        SendClick(170, 130); // Click upgrade
        Wait(1);
        SendClick(230, 375); // Confirm
        Wait(1);
        SendClick(165, 375); // Continue
        Wait(1);
        SendClick(300, 560); // Exit the food store
        Wait(1);
        SendClick(20, 60); // Complete quest
        Wait(1);
        SendClick(170, 435); // Collect bux
        Wait(1);
        SendClick(170, 435); // Colect more bux
        Wait(1);
        SendClick(165, 375); // Continue
        _configManager.ChangeCurrentFloor(3);
    }

    public void RestartGame()
    {
        _mainWindow.Log("Restarting the app");
        //IntPtr mainHandle = GetProcess().MainWindowHandle;
        SendEscapeButton();
        Wait(1);
        SendClick(230, 380);

        //InputSimulator.SendMessage(mainHandle, (int)InputSimulator.KeyCodes.WM_LBUTTONDOWN, 1, MakeLParam(98, 17));
        //InputSimulator.SendMessage(mainHandle, (int)InputSimulator.KeyCodes.WM_LBUTTONUP, 0, MakeLParam(98, 17));
        //Wait(1);
        //InputSimulator.SendMessage(mainHandle, (int)InputSimulator.KeyCodes.WM_LBUTTONDOWN, 1, MakeLParam(355, 547));
        //InputSimulator.SendMessage(mainHandle, (int)InputSimulator.KeyCodes.WM_LBUTTONUP, 0, MakeLParam(355, 547));
        //Wait(1);
    }

    public void MoveUp()
    {
        SendClick(160, 8);
        Wait(1);
    }

    public void MoveDown()
    {
        SendClick(230, 580);
    }

    public void PressExitButton()
    {
        _mainWindow.Log("Pressing the exit button");
        SendClick(305, 565);
    }

    public int PlayRaffle(int currentHour)
    {
        if (currentHour != DateTime.Now.Hour)
        {
            _mainWindow.Log("Playing the raffle");
            Wait(1);
            SendClick(300, 570);
            Wait(1);
            SendClick(275, 440);
            Wait(2);
            SendClick(165, 375);

            return DateTime.Now.Hour;
        }
        else
        {
            return currentHour;
        }
    }

    #endregion

    #region Utility Methods

    (Percentage x, Percentage y) GetScreenDiffPercentage()
    {
        var _screenRect = InputSimulator.GetWindowRectangle(_childHandle);
        int rectX = Math.Abs(_screenRect.Width - _screenRect.Left);
        int rectY = Math.Abs(_screenRect.Height - _screenRect.Top);
        float x1 = ((float)rectX * 100 / 333);
        float y1 = ((float)rectY * 100 / 592);
        var _screenHeightPercentage = new Percentage(y1);
        var _screenWidthPercentage = new Percentage(x1);
        return (_screenHeightPercentage, _screenWidthPercentage);
    }

    int GetRelativeCoords(int x, int y)
    {
        int rectX = Math.Abs(_screenRect.Width - _screenRect.Left);
        int rectY = Math.Abs(_screenRect.Height - _screenRect.Top);
        float x1 = ((float)x * 100 / 333) / 100;
        float y1 = ((float)y * 100 / 592) / 100;
        
        int x2 = (int)(rectX * x1);
        int y2 = (int)(rectY * y1);
        return MakeLParam(x2, y2);
    }

    void Wait(int seconds)
    {
        int milliseconds = seconds * 1000;
        Task.Delay(milliseconds).Wait();
    }

    bool IsImageFound(string imageKey) // Returns true if the image is found 
    {
        Image gameWindow = MakeScreenshot();
        var windowBitmap = new Bitmap(gameWindow);
        gameWindow.Dispose();
        Mat reference = BitmapConverter.ToMat(windowBitmap);
        windowBitmap.Dispose();

        var template = _clickerApp._templates[imageKey];
        using (Mat res = new(reference.Rows - template.Rows + 1, reference.Cols - template.Cols + 1, MatType.CV_8S))
        {
            Mat gref = reference.CvtColor(ColorConversionCodes.BGR2GRAY);
            Mat gtpl = template.CvtColor(ColorConversionCodes.BGR2GRAY);

            Cv2.MatchTemplate(gref, gtpl, res, TemplateMatchModes.CCoeffNormed);
            Cv2.Threshold(res, res, 0.7, 1.0, ThresholdTypes.Tozero);

            double threshold = 0.7;
            Cv2.MinMaxLoc(res, out double minval, out double maxval, out Point minloc, out Point maxloc);

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

    public void MatchSingleTemplate(KeyValuePair<string, Mat> template, Mat reference)
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
                    _clickerApp._matchedTemplates.Add(template.Key, MakeLParam(maxloc.X, maxloc.Y));
                    break;
                }
                else
                {
                    break;
                }
            }
        }
    }

    public Process GetProcess()
    {
        string curProcName = "";
        if (_clickerApp._isBluestacks)
        {
            curProcName = _blueStacksProcName;
        }
        if (_clickerApp._isLDPlayer)
        {
            curProcName = _ldPlayerProcName;
        }

        Process[] processlist = Process.GetProcesses();
        foreach (Process process in processlist)
        {
            if (!string.IsNullOrEmpty(process.MainWindowTitle) && process.ProcessName == curProcName)
            {
                return process;
            }
        }
        _mainWindow.Log("Emulator process not found");
        throw new Exception("Emulator process not found");
    }

    public void SendClick(int location)
    {
        if (_childHandle != IntPtr.Zero)
        {
            if (_clickerApp._isBluestacks)
            {
                // Bluestacks input 
                InputSimulator.SendMessage(_process.MainWindowHandle, (int)InputSimulator.KeyCodes.WM_SETFOCUS, 0, 0);
                InputSimulator.PostMessageA(_childHandle, (int)InputSimulator.KeyCodes.WM_LBUTTONDOWN, 0x0001, location);
                InputSimulator.PostMessageA(_childHandle, (int)InputSimulator.KeyCodes.WM_LBUTTONUP, 0x0001, location);
            }
            else
            {
                // LDPlayer input 
                InputSimulator.SendMessage(_childHandle, (int)InputSimulator.KeyCodes.WM_LBUTTONDOWN, 1, location);
                Task.Delay(1).Wait();
                InputSimulator.SendMessage(_childHandle, (int)InputSimulator.KeyCodes.WM_LBUTTONUP, 0, location);
            }
        }
    }
    public void SendClick(int x, int y)
    {
        SendClick(GetRelativeCoords(x, y));
    }

    public void SendEscapeButton()
    {
        if (_childHandle != IntPtr.Zero)
        {
            if (_clickerApp._isBluestacks)
            {
                // Bluestacks input 
                InputSimulator.SendMessage(_process.MainWindowHandle, (int)InputSimulator.KeyCodes.WM_SETFOCUS, 0, 0);
                InputSimulator.PostMessageA(_childHandle, (int)InputSimulator.KeyCodes.WM_KEYDOWN, (int)InputSimulator.KeyCodes.VK_ESCAPE, 0);
            }
            else
            {
                // LDPlayer input 
                InputSimulator.SendMessage(_childHandle, (int)InputSimulator.KeyCodes.WM_KEYDOWN, (int)InputSimulator.KeyCodes.VK_ESCAPE, 0);
            }
        }
    }

    public int MakeLParam(int x, int y) => (y << 16) | (x & 0xFFFF); // Generate coordinates within the game screen

    public IntPtr GetChildHandle()
    {
        string curProcName = "";
        if (_clickerApp._isBluestacks)
        {
            curProcName = _blueStacksProcName;

            if (WindowHandleInfo.GetChildrenHandles(curProcName) != null)
            {
                List<IntPtr> childProcesses = WindowHandleInfo.GetChildrenHandles(curProcName);
                if (childProcesses != null && childProcesses.Count >= 1)
                {
                    return childProcesses[0];
                }
            }
        }
        else
        {
            curProcName = _ldPlayerProcName;
        }

        if (WindowHandleInfo.GetChildrenHandles(curProcName) != null)
        {
            List<IntPtr> childProcesses = WindowHandleInfo.GetChildrenHandles(curProcName);
            if (childProcesses != null)
            {
                return childProcesses[0];
            }
        }
        _mainWindow.Log("Emulator process not found - TinyClicker function is not possible. Launch emulator and restart the app.");
        throw new Exception("Emulator child handle not found");
    }

    public Image MakeScreenshot()
    {
        if (_processId != -1)
        {
            IntPtr handle = Process.GetProcessById(_processId).MainWindowHandle;
            Image img = _screenshotManager.CaptureWindow(handle);
            return img;
        }
        else
        {
            throw new Exception("No process with LDPlayer found. Try restarting the LDPlayer and then restart the Tiny Clicker");
        }
    }

    public void SaveScreenshot()
    {
        if (_processId != -1)
        {
            if (!Directory.Exists(Environment.CurrentDirectory + @"\screenshots"))
            {
                Directory.CreateDirectory(Environment.CurrentDirectory + @"\screenshots");
            }

            IntPtr handle = Process.GetProcessById(_processId).MainWindowHandle;
            // Captures screenshot of a window and saves it to the screenshots folder
            _screenshotManager.CaptureWindowToFile(handle, Environment.CurrentDirectory + @"\screenshots\window.png", ImageFormat.Png);
            _mainWindow.Log(@"Made a screenshot. Screenshots can be found inside TinyClicker\screenshots folder");
        }
    }

    public void SaveStatRebuildTime()
    {
        DateTime dateTimeNow = DateTime.Now;
        DateTime lastRebuild = _configManager._curConfig.LastRebuildTime;
        string result = "";
        if (lastRebuild != DateTime.MinValue)
        {
            TimeSpan diff = dateTimeNow - lastRebuild;
            string formatted = diff.ToString(@"hh\:mm\:ss");
            if (diff.Days >= 1)
            {
                result = string.Format("{0} days ", diff.Days) + formatted;
            }
            else
            {
                result = formatted;
            }
        }
        string statsPath = Environment.CurrentDirectory + @"\Stats.txt";
        _configManager.SaveNewRebuildTime(dateTimeNow);
        string data = $"{dateTimeNow} - rebuilt the tower. Time elapsed since the last rebuild: {result}\n";
        File.AppendAllText(statsPath, data);
    }
    
    public Dictionary<int, int> CalculateFloorPrices()
    {
        var dict = new Dictionary<int, int>();

        // Floors 1 through 9 cost 5000
        for (int i = 1; i <= 9; i++)
        {
            dict.Add(i, 5000);
        }

        // Calculate the prices for floors 10 through 50+
        for (int i = 10; i <= _clickerApp._floorToRebuildAt + 1; i++)
        {
            float floorCost = 1000 * 1 * (0.5f * (i * i) + 8 * i - 117);

            // Round up the result to match with the in-game prices
            if (i % 2 != 0)
            {
                floorCost += 500;
            }

            dict.Add(i, (int)floorCost);
        }
        return dict;
    }

    public Dictionary<string, Image> FindImages()
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
            _mainWindow.Log(msg);

            return dict;
        }
    }

    public Dictionary<string, Mat> MakeTemplates(Dictionary<string, Image> images)
    {
        var percentages = GetScreenDiffPercentage();

        

        Dictionary<string, Mat> mats = new();
        foreach (var image in images)
        {
            // Resize images before making templates
            using (var imageOld = new MagickImage(ImageToBytes(image.Value), MagickFormat.Png))
            {
                imageOld.Resize(percentages.y);
                var imageBitmap = new Bitmap(BytesToImage(imageOld.ToByteArray()));
                Mat template = BitmapConverter.ToMat(imageBitmap);
                imageBitmap.Dispose();
                mats.Add(image.Key, template);
            }
        }
        images.Clear();
        return mats;
    }

    byte[] ImageToBytes(Image image)
    {
        using (var ms = new MemoryStream())
        {
            image.Save(ms, image.RawFormat);
            return ms.ToArray();
        }
    }

    Image BytesToImage(byte[] bytes)
    {
        using (var ms = new MemoryStream(bytes))
        {
            return Image.FromStream(ms);
        }
    }

    #endregion
}