using System;
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
    readonly ScreenScanner _screenScanner;
    readonly ScreenshotManager _screenshotManager;
    readonly ConfigManager _configManager;
    readonly ImageToText _imageToText;
    readonly ImageEditor _imageEditor;

    const string _ldPlayerProcName = "dnplayer";
    const string _blueStacksProcName = "HD-Player";

    internal Process _process;
    public int _processId;
    readonly IntPtr _childHandle;
    Dictionary<int, int> _floorPrices;
    readonly MainWindow _mainWindow;
    DateTime _timeForNewFloor;
    Rectangle _screenRect;

    public ClickerActionsRepo(ScreenScanner imageToAction)
    {
        _screenScanner = imageToAction;
        _configManager = new ConfigManager();
        _mainWindow = Application.Current.Windows.OfType<MainWindow>().First();
        _process = GetProcess();
        _processId = _process.Id;
        _childHandle = GetChildHandle();
        _timeForNewFloor = DateTime.Now;
        _screenRect = InputSimulator.GetWindowRectangle(_childHandle);
        _screenshotManager = new ScreenshotManager();
        _imageEditor = new ImageEditor(_screenRect);
        _imageToText = new ImageToText(_imageEditor);
        _floorPrices = new Dictionary<int, int>();
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
        SendClick(_screenScanner._matchedTemplates["freeBuxCollectButton"]);
    }

    public void ClickOnChute()
    {
        _mainWindow.Log("Clicking on the parachute");
        SendClick(_screenScanner._matchedTemplates["giftChute"]);
        Wait(1);
        if (IsImageFound("watchAdPromptCoins") && _configManager._curConfig.CurrentFloor >= _screenScanner._floorToStartWatchingAds)
        {
            WatchCoinsAds();
        }
        else if (_screenScanner._acceptBuxVideoOffers && _configManager._curConfig.CurrentFloor >= _screenScanner._floorToStartWatchingAds)
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

        if (_screenScanner._matchedTemplates.ContainsKey("closeAd_7") || _screenScanner._matchedTemplates.ContainsKey("closeAd_8"))
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

        SendClick(_screenScanner._matchedTemplates["continueButton"]);
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
        SendClick(_screenScanner._matchedTemplates["questButton"]);
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
        SendClick(_screenScanner._matchedTemplates["gameIcon"]);
        Wait(10);
    }

    public void CloseHiddenAd()
    {
        _mainWindow.Log("Closing hidden ads");
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
        SendClick(_screenScanner._matchedTemplates["completedQuestButton"]);
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
            if (balance > targetPrice && currentFloor < _screenScanner._floorToRebuildAt)
            {
                BuildNewFloor();
            }
            if (currentFloor >= _screenScanner._floorToRebuildAt)
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
            _mainWindow.Log("Building new floor");
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
        SendEscapeButton();
        Wait(1);
        SendClick(230, 380);
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

        var template = _screenScanner._templates[imageKey];
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

    public Process GetProcess()
    {
        string curProcName = "";
        if (_screenScanner._isBluestacks)
        {
            curProcName = _blueStacksProcName;
        }
        if (_screenScanner._isLDPlayer)
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
            if (_screenScanner._isBluestacks)
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
            if (_screenScanner._isBluestacks)
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
        if (_screenScanner._isBluestacks)
        {
            curProcName = _blueStacksProcName;
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
            throw new Exception("No emulator process found");
        }
    }

    public void SaveScreenshot()
    {
        if (_processId != -1)
        {
            if (!Directory.Exists($"./screenshots"))
            {
                Directory.CreateDirectory($"./screenshots");
            }

            IntPtr handle = Process.GetProcessById(_processId).MainWindowHandle;
            // Captures screenshot of a window and saves it to the screenshots folder
            _screenshotManager.CaptureWindowToFile(handle, $"./screenshots/window.png", ImageFormat.Png);
            _mainWindow.Log($"Made a screenshot. Screenshots can be found inside TinyClicker/screenshots folder");
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
        string statsPath = $"./Stats.txt";
        _configManager.SaveNewRebuildTime(dateTimeNow);
        string data = $"{dateTimeNow} - rebuilt the tower. Time elapsed since the last rebuild: {result}\n";
        File.AppendAllText(statsPath, data);
    }

    private Dictionary<int, int> CalculateFloorPrices()
    {
        var dict = new Dictionary<int, int>();

        // Floors 1 through 9 cost 5000
        for (int i = 1; i <= 9; i++)
        {
            dict.Add(i, 5000);
        }

        // Calculate the prices for floors 10 through 50+
        for (int i = 10; i <= _screenScanner._floorToRebuildAt + 1; i++)
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

    private byte[][] LoadData()
    {
        string path = $"./samples/samples.dat";
        var array = new byte[0][];
        using (var file = new FileStream(path, FileMode.Open, FileAccess.Read))
        {
            int countRead;
            int countItems;
            int sizeInt = sizeof(int);
            var sizeRow = new byte[sizeInt];
            while (true)
            {
                countRead = file.Read(sizeRow, 0, sizeInt);
                if (countRead != sizeInt)
                {
                    break;
                }
                countItems = BitConverter.ToInt32(sizeRow, 0);
                var data = new byte[countItems];
                countRead = file.Read(data, 0, countItems);
                if (countRead != countItems)
                {
                    break;
                }
                Array.Resize(ref array, array.Length + 1);
                array[array.Length - 1] = data;
            }
            file.Close();
        }
        return array;
    }

    public Dictionary<string, Image> GetSamples()
    {
        var dict = new Dictionary<string, Image>();
        string[] buttonNames = File.ReadAllLines($"./samples/button_names.txt");
        byte[][] buttonData = LoadData();

        for (int i = 0; i < buttonNames.Length; i++)
        {
            byte[] sample = buttonData[i];
            using (var ms = new MemoryStream(sample))
            {
                Image.FromStream(ms);
                dict.Add(buttonNames[i], Image.FromStream(ms));
            }
        }
        return dict;
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
        images = null;
        return mats;
    }

    private byte[] ImageToBytes(Image image)
    {
        using (var ms = new MemoryStream())
        {
            image.Save(ms, image.RawFormat);
            return ms.ToArray();
        }
    }

    private Image BytesToImage(byte[] bytes)
    {
        using (var ms = new MemoryStream(bytes))
        {
            return Image.FromStream(ms);
        }
    }

    #endregion
}
