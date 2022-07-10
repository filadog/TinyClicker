using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Linq;
using System.Threading.Tasks;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using Point = OpenCvSharp.Point;
using ImageMagick;

namespace TinyClicker;

public class ClickerActionsRepo
{
    public readonly ScreenScanner _screenScanner;
    readonly ConfigManager _configManager;
    readonly ImageToText _imageToText;
    readonly ImageEditor _imageEditor;
    public readonly InputSimulator inputSim;

    Dictionary<int, int> _floorPrices;
    public readonly MainWindow mainWindow;
    DateTime _timeForNewFloor;
    Rectangle _screenRect;
    bool _floorPricesCalculated = false;

    public ClickerActionsRepo(ScreenScanner screenScanner)
    {
        _screenScanner = screenScanner;
        _configManager = _screenScanner.configManager;
        mainWindow = Application.Current.Windows.OfType<MainWindow>().First();
        inputSim = new InputSimulator(this);
        _timeForNewFloor = DateTime.Now;
        _screenRect = inputSim.GetWindowRectangle();
        _imageEditor = new ImageEditor(_screenRect, this);
        _imageToText = new ImageToText(_imageEditor);
        _floorPrices = new Dictionary<int, int>();
    }

    #region Clicker Actions

    // Methods below execute depending on what's found on the game screen

    public void CancelHurryConstruction()
    {
        mainWindow.Log("Exiting the construction menu");

        inputSim.SendClick(100, 375); // Cancel action
        Wait(1);
    }

    public void CollectFreeBux()
    {
        mainWindow.Log("Collecting free bux");
        inputSim.SendClick(_screenScanner._matchedTemplates["freeBuxCollectButton"]);
    }

    public void ClickOnChute()
    {
        mainWindow.Log("Clicking on the parachute");
        inputSim.SendClick(_screenScanner._matchedTemplates["giftChute"]);
        Wait(1);
        if (IsImageFound("watchAdPromptCoins") && _configManager.curConfig.CurrentFloor >= _screenScanner._floorToStartWatchingAds)
        {
            WatchCoinsAds();
        }
        else if (_screenScanner._acceptBuxVideoOffers && _configManager.curConfig.CurrentFloor >= _screenScanner._floorToStartWatchingAds)
        {
            if (IsImageFound("watchAdPromptBux"))
            {
                WatchBuxAds();
            }
        }
        else
        {
            inputSim.SendClick(105, 380); // Decline the video offer
        }
    }

    public void CloseAd()
    {
        mainWindow.Log("Closing the advertisement");

        if (_screenScanner._matchedTemplates.ContainsKey("closeAd_7") || _screenScanner._matchedTemplates.ContainsKey("closeAd_8"))
        {
            inputSim.SendClick(22, 22);
            inputSim.SendClick(311, 22);
            inputSim.SendClick(302, 52);
        }
        else
        {
            inputSim.SendClick(311, 22);
            inputSim.SendClick(22, 22);
            inputSim.SendClick(302, 52);
        }
        CheckForLostAdsReward();
    }

    public void CloseChuteNotification()
    {
        mainWindow.Log("Closing the parachute notification");
        inputSim.SendClick(165, 375); // Close the notification
    }

    public void ExitRoofCustomizationMenu()
    {
        mainWindow.Log("Exiting the roof customization menu");
        PressExitButton();
        Wait(1);
    }

    public void PressContinue()
    {
        mainWindow.Log("Clicking continue");

        inputSim.SendClick(_screenScanner._matchedTemplates["continueButton"]);
        Wait(1);
        MoveUp();
    }

    public void Restock()
    {
        mainWindow.Log("Restocking");
        MoveDown();
        Wait(1);
        inputSim.SendClick(100, 480); // Stock all
        Wait(1);
        inputSim.SendClick(225, 375);
        Wait(1);

        if (IsImageFound("fullyStockedBonus"))
        {
            inputSim.SendClick(165, 375); // Close the bonus tooltip
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
        mainWindow.Log("Pressing free bux icon");
        inputSim.SendClick(25, 130);
        Wait(1);
        inputSim.SendClick(230, 375);
        Wait(1);
    }
    
    public void RideElevator()
    {
        mainWindow.Log("Riding the elevator");
        inputSim.SendClick(45, 535);
        int curFloor = _configManager.curConfig.CurrentFloor;
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
        mainWindow.Log("Clicking on the quest button");
        inputSim.SendClick(_screenScanner._matchedTemplates["questButton"]);
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
            inputSim.SendClick(90, 440); // Skip the quest
            Wait(1);
            inputSim.SendClick(230, 380); // Confirm
        }
    }

    public void FindBitizens()
    {
        mainWindow.Log("Skipping the quest");
        inputSim.SendClick(95, 445); // Skip the quest
        Wait(1);
        inputSim.SendClick(225, 380); // Confirm skip
    }

    public void DeliverBitizens()
    {
        mainWindow.Log("Delivering bitizens");
        inputSim.SendClick(230, 440); // Continue
    }

    public void OpenTheGame()
    {
        Wait(1);
        inputSim.SendClick(_screenScanner._matchedTemplates["gameIcon"]);
        Wait(10);
    }

    public void CloseHiddenAd()
    {
        mainWindow.Log("Closing hidden ads");
        Wait(1);
        inputSim.SendClick(310, 10);
        Wait(1);
        inputSim.SendClick(311, 22);
        CheckForLostAdsReward();
    }

    public void CheckForLostAdsReward()
    {
        Wait(1);
        if (IsImageFound("adsLostReward"))
        {
            inputSim.SendClick(240, 344); // Click "Keep watching"
            Wait(15);
        }
        else
        {
            inputSim.SendClick(240, 344);
        }
    }

    public void CloseNewFloorMenu()
    {
        mainWindow.Log("Exiting");
        PressExitButton();
    }

    public void CloseBuildNewFloorNotification()
    {
        mainWindow.Log("Closing the new floor notification");
        inputSim.SendClick(105, 320); // Click no
    }

    public void CompleteQuest()
    {
        mainWindow.Log("Completing the quest");
        
        Wait(1);
        inputSim.SendClick(_screenScanner._matchedTemplates["completedQuestButton"]);
    }

    public void WatchCoinsAds()
    {
        mainWindow.Log("Watching the advertisement");
        inputSim.SendClick(225, 375);
        Wait(20);
    }

    public void WatchBuxAds()
    {
        mainWindow.Log("Watching the advertisement");
        inputSim.SendClick(225, 375);
        Wait(20);
    }

    public void CheckForNewFloor(int currentFloor, Image gameWindow)
    {
        if (!_floorPricesCalculated)
        {
            _floorPrices = CalculateFloorPrices();
            _floorPricesCalculated = true;
        }
        int balance = _imageToText.ParseBalance(gameWindow);
        if (balance != 0 && balance != -1 && currentFloor >= 3)
        {
            if (currentFloor >= _screenScanner._floorToRebuildAt)
            {
                Wait(1);
                RebuildTower();
                return;
            }
            int targetPrice = _floorPrices[currentFloor + 1];
            if (balance > targetPrice && currentFloor < _screenScanner._floorToRebuildAt)
            {
                if (balance.ToString().Length < 9)
                {
                    BuildNewFloor();
                }
            }
        }
    }

    public void BuildNewFloor()
    {
        if (_timeForNewFloor <= DateTime.Now)
        {
            mainWindow.Log("Building new floor");
            MoveUp();
            Wait(2);
            inputSim.SendClick(195, 390); // Click on a new floor
            Wait(1);

            if (IsImageFound("buildNewFloorNotification"))
            {
                inputSim.SendClick(230, 320); // Confirm construction
                Wait(1);
                // Add a new floor if there is enough coins
                if (!IsImageFound("newFloorNoCoinsNotification"))
                {
                    _configManager.AddOneFloor();
                    mainWindow.Log("Built a new floor");
                }
                else
                {
                    // Cooldown 30s in case building fails (to prevent repeated attempts)
                    _timeForNewFloor = DateTime.Now.AddSeconds(30);
                    mainWindow.Log("Not enough coins for a new floor");
                }
            }
            MoveUp();
        }
        else
        {
            mainWindow.Log("Too early to build a floor");
            Wait(1);
        }
    }

    public void RebuildTower()
    {
        mainWindow.Log("Rebuilding the tower");
        _configManager.SaveStatRebuildTime();
        inputSim.SendClick(305, 570);
        Wait(1);
        inputSim.SendClick(165, 435);
        Wait(1);
        inputSim.SendClick(165, 440);
        Wait(1);
        inputSim.SendClick(230, 380);
        Wait(1);
        inputSim.SendClick(230, 380);
        Wait(3);
        _configManager.SetCurrentFloor(1);
    }

    public void PassTheTutorial()
    {
        mainWindow.Log("Passing the tutorial");
        Wait(3);
        inputSim.SendClick(170, 435); // Continue
        Wait(3);
        MoveDown();
        Wait(1);
        inputSim.SendClick(195, 260); // Build a new floor
        Wait(1);
        inputSim.SendClick(230, 380); // Confirm
        Wait(1);
        inputSim.SendClick(20, 60);   // Complete quest
        Wait(1);
        inputSim.SendClick(170, 435); // Collect bux
        Wait(1);
        inputSim.SendClick(170, 435); // Continue
        Wait(1);
        inputSim.SendClick(190, 300); // Click on a new floor
        Wait(1);
        inputSim.SendClick(240, 150); // Build a residential floor
        Wait(1);
        inputSim.SendClick(160, 375); // Continue
        Wait(1);
        inputSim.SendClick(20, 60);   // Complete quest
        Wait(1);
        inputSim.SendClick(170, 435); // Collect bux
        Wait(1);
        inputSim.SendClick(170, 435); // Continue
        Wait(1);
        inputSim.SendClick(30, 535);  // Ride elevator
        Wait(5);
        inputSim.SendClick(230, 380); // Continue
        Wait(1);
        inputSim.SendClick(20, 60);   // Complete quest
        Wait(1);
        inputSim.SendClick(170, 435); // Collect bux
        Wait(1);
        inputSim.SendClick(170, 435); // Continue
        Wait(1);
        inputSim.SendClick(190, 200); // Build a new floor
        Wait(1);
        inputSim.SendClick(225, 380); // Confirm
        Wait(1);
        inputSim.SendClick(200, 200); // Open the new floor
        Wait(1);
        inputSim.SendClick(90, 340);  // Build random food floor
        Wait(1);
        inputSim.SendClick(170, 375); // Continue
        Wait(1);
        inputSim.SendClick(20, 60);   // Complete the quest
        Wait(1);
        inputSim.SendClick(170, 435); // Collect bux
        Wait(1);
        inputSim.SendClick(170, 435); // Continue
        Wait(1);
        inputSim.SendClick(200, 200); // Open food floor
        Wait(1);
        inputSim.SendClick(75, 210);  // Open the hire menu
        Wait(1);
        inputSim.SendClick(80, 100);  // Select our bitizen
        Wait(1);
        inputSim.SendClick(230, 380); // Hire him
        Wait(1);
        inputSim.SendClick(160, 380); // Continue on dream job assignement
        Wait(1);
        inputSim.SendClick(300, 560); // Exit the food store
        Wait(1);
        inputSim.SendClick(20, 60);   // Complete the quest
        Wait(1);
        inputSim.SendClick(170, 435); // Collect bux
        Wait(1);
        inputSim.SendClick(170, 435); // Continue
        Wait(1);
        inputSim.SendClick(200, 200); // Open the food store again
        Wait(1);
        inputSim.SendClick(200, 210); // Request restock of the first item in the store
        Wait(15);
        inputSim.SendClick(305, 190); // Press restock button
        Wait(1);
        inputSim.SendClick(20, 60);   // Complete the quest
        Wait(1);
        inputSim.SendClick(170, 435); // Collect bux
        Wait(1);
        inputSim.SendClick(170, 435); // Continue
        Wait(1);
        inputSim.SendClick(200, 200); // Open food store again
        Wait(1);
        inputSim.SendClick(170, 130); // Click upgrade
        Wait(1);
        inputSim.SendClick(230, 375); // Confirm
        Wait(1);
        inputSim.SendClick(165, 375); // Continue
        Wait(1);
        inputSim.SendClick(300, 560); // Exit the food store
        Wait(1);
        inputSim.SendClick(20, 60);   // Complete the quest
        Wait(1);
        inputSim.SendClick(170, 435); // Collect bux
        Wait(1);
        inputSim.SendClick(170, 435); // Collect more bux
        Wait(1);
        inputSim.SendClick(165, 375); // Continue
        _configManager.SetCurrentFloor(3);
    }

    public void RestartGame()
    {
        mainWindow.Log("Restarting the app");
        inputSim.SendEscapeButton();
        Wait(1);
        inputSim.SendClick(230, 380);
    }

    public void MoveUp()
    {
        inputSim.SendClick(160, 8);
        Wait(1);
    }

    public void MoveDown()
    {
        inputSim.SendClick(230, 580);
    }

    public void PressExitButton()
    {
        mainWindow.Log("Pressing the exit button");
        inputSim.SendClick(305, 565);
    }

    public int PlayRaffle(int currentHour)
    {
        if (currentHour != DateTime.Now.Hour)
        {
            mainWindow.Log("Playing the raffle");
            Wait(1);
            inputSim.SendClick(300, 570);
            Wait(1);
            inputSim.SendClick(275, 440);
            Wait(2);
            inputSim.SendClick(165, 375);
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
        var _screenRect = inputSim.GetWindowRectangle();
        int rectX = Math.Abs(_screenRect.Width - _screenRect.Left);
        int rectY = Math.Abs(_screenRect.Height - _screenRect.Top);
        float x1 = ((float)rectX * 100 / 333);
        float y1 = ((float)rectY * 100 / 592);
        var _screenHeightPercentage = new Percentage(y1);
        var _screenWidthPercentage = new Percentage(x1);
        return (_screenHeightPercentage, _screenWidthPercentage);
    }

    void Wait(int seconds)
    {
        int ms = seconds * 1000;
        Task.Delay(ms).Wait();
    }

    bool IsImageFound(string imageKey) // Returns true if the image is found 
    {
        Image gameWindow = inputSim.MakeScreenshot();
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

    public bool IsImageFound(string imageKey, out Point location)
    {
        Image gameWindow = inputSim.MakeScreenshot();
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
                location = maxloc;
                return true;
            }
            else
            {
                location = maxloc;
                return false;
            }
        }
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

    private byte[][] LoadButtonData()
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
        byte[][] buttonData = LoadButtonData();

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
