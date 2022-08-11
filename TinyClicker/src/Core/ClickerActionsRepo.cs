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
    readonly ConfigManager configManager;
    readonly ImageToText imageToText;
    readonly ImageEditor imageEditor;
    public readonly InputSimulator inputSim;

    Dictionary<int, int> _floorPrices;
    public readonly MainWindow mainWindow;
    DateTime _timeForNewFloor;
    Rectangle _screenRect;
    bool _floorPricesCalculated = false;

    public ClickerActionsRepo(ScreenScanner screenScanner)
    {
        _screenScanner = screenScanner;
        configManager = _screenScanner.configManager;
        mainWindow = Application.Current.Windows.OfType<MainWindow>().First();
        inputSim = new InputSimulator(this);
        _timeForNewFloor = DateTime.Now;
        _screenRect = inputSim.GetWindowRectangle();
        imageEditor = new ImageEditor(_screenRect, this);
        imageToText = new ImageToText(imageEditor);
        _floorPrices = new Dictionary<int, int>();
    }

    #region Clicker Actions

    public void CancelHurryConstruction()
    {
        mainWindow.Log("Exiting the construction menu");

        inputSim.SendClick(100, 375); // Cancel action
        WaitSec(1);
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
        WaitMs(500);
        if (IsImageFound("watchAdPromptCoins") && configManager.curConfig.CurrentFloor >= _screenScanner.floorToStartWatchingAds)
        {
            TryWatchAds();
        }
        else if (_screenScanner.acceptBuxVideoOffers && configManager.curConfig.CurrentFloor >= _screenScanner.floorToStartWatchingAds)
        {
            if (IsImageFound("watchAdPromptBux"))
            {
                TryWatchAds();
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
            inputSim.SendClick(310, 41);
        }
        else
        {
            inputSim.SendClick(311, 22);
            inputSim.SendClick(22, 22);
            inputSim.SendClick(302, 52);
            inputSim.SendClick(310, 41);
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
        WaitMs(500);
    }

    public void PressContinue()
    {
        mainWindow.Log("Clicking continue");

        inputSim.SendClick(_screenScanner._matchedTemplates["continueButton"]);
        WaitMs(500);
        MoveUp();
        WaitSec(1);
    }

    public void Restock()
    {
        mainWindow.Log("Restocking");
        MoveDown();
        WaitMs(500);
        inputSim.SendClick(100, 480); // Stock all
        WaitMs(500);
        inputSim.SendClick(225, 375);
        WaitMs(500);

        if (IsImageFound("fullyStockedBonus"))
        {
            inputSim.SendClick(165, 375); // Close the bonus tooltip
            WaitSec(1);
            MoveUp();
            WaitSec(1);
            return;
        }
        else
        {
            MoveUp();
            WaitSec(1);
            return;
        }
    }

    public void PressFreeBuxButton()
    {
        mainWindow.Log("Pressing free bux icon");
        inputSim.SendClick(25, 130);
        WaitSec(1);
        inputSim.SendClick(230, 375);
        WaitSec(1);
    }
    
    public void RideElevator()
    {
        mainWindow.Log("Riding the elevator");
        inputSim.SendClick(21, 510);
        WaitSec(1);
        
        if (IsImageFound("backButton"))
        {
            PressExitButton();
        }
        else if (IsImageFound("continueButton", out Point location))
        {
            inputSim.SendClick(location.X, location.Y); // Click continue in case a new bitizen moved in
        }
        else 
        {
            int curFloor = configManager.curConfig.CurrentFloor;
            WaitMs(curFloor * 100); // Wait for the ride to finish
            if (IsImageFound("giftChute"))
            {
                return;
            }
            else
            {
                MoveUp();
            }
        }
    }

    public void PressQuestButton()
    {
        mainWindow.Log("Clicking on the quest button");
        inputSim.SendClick(_screenScanner._matchedTemplates["questButton"]);
        WaitMs(500);
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
            WaitMs(500);
            inputSim.SendClick(230, 380); // Confirm
        }
    }

    public void FindBitizens()
    {
        mainWindow.Log("Skipping the quest");
        inputSim.SendClick(95, 445); // Skip the quest
        WaitMs(500);
        inputSim.SendClick(225, 380); // Confirm skip
    }

    public void DeliverBitizens()
    {
        mainWindow.Log("Delivering bitizens");
        inputSim.SendClick(230, 440); // Continue
    }

    public void OpenTheGame()
    {
        WaitSec(1);
        inputSim.SendClick(_screenScanner._matchedTemplates["gameIcon"]);
        WaitSec(7);
    }

    public void CloseHiddenAd()
    {
        mainWindow.Log("Closing hidden ads");
        WaitMs(500);
        inputSim.SendClick(310, 10);
        inputSim.SendClick(310, 41);
        WaitMs(500);
        inputSim.SendClick(311, 22);
        CheckForLostAdsReward();
    }

    public void CheckForLostAdsReward()
    {
        WaitMs(500);
        if (IsImageFound("adsLostReward"))
        {
            inputSim.SendClick(240, 344); // Click "Keep watching"
            WaitSec(15);
        }
        else
        {
            inputSim.SendClick(240, 344);
        }
    }

    public void CheckForExitButton()
    {
        if (IsImageFound("backButton"))
        {
            PressExitButton();
        }
    }

    public void CloseNewFloorMenu()
    {
        mainWindow.Log("Exiting from new floor menu");
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

        WaitMs(500);
        inputSim.SendClick(_screenScanner._matchedTemplates["completedQuestButton"]);
    }

    public void CollectNewScience()
    {
        mainWindow.Log("Collecting new science");
        inputSim.SendClick(_screenScanner._matchedTemplates["newScienceButton"]);
        WaitMs(500);
        inputSim.SendClick(150, 110);
        WaitMs(300);
        PressExitButton();
        WaitMs(300);
        PressExitButton();
    }
    
    public void CheckForNewFloor(int currentFloor, Image gameWindow)
    {
        if (!_floorPricesCalculated)
        {
            _floorPrices = CalculateFloorPrices();
            _floorPricesCalculated = true;
        }
        int balance = imageToText.ParseBalance(gameWindow);
        if (balance != -1 && currentFloor >= 3)
        {
            if (currentFloor >= _screenScanner.floorToRebuildAt)
            {
                WaitMs(500);
                RebuildTower();
                return;
            }
            int targetPrice = _floorPrices[currentFloor + 1];
            if (balance > targetPrice && currentFloor < _screenScanner.floorToRebuildAt)
            {
                BuildNewFloor();
                // Allow consecutive building of new floors if there is enough coins, recursive call
                using var newGameWindow = inputSim.MakeScreenshot();
                CheckForNewFloor(configManager.curConfig.CurrentFloor, newGameWindow);
            }
        }
    }

    public void BuildNewFloor()
    {
        if (_timeForNewFloor <= DateTime.Now)
        {
            if (IsImageFound("backButton"))
            {
                PressExitButton();
                return;
            }
            
            mainWindow.Log("Building new floor");
            MoveUp();
            WaitSec(1);
            inputSim.SendClick(195, 390); // Click on a new floor
            WaitMs(500);
            
            if (IsImageFound("continueButton"))
            {
                inputSim.SendClick(105, 380); // Click "No thanks" on chute notification
            }
            else if (IsImageFound("buildNewFloorNotification"))
            {
                inputSim.SendClick(230, 320); // Confirm construction
                WaitMs(500);
                // Add a new floor if there is enough coins
                if (!IsImageFound("newFloorNoCoinsNotification"))
                {
                    configManager.AddOneFloor();
                    mainWindow.Log("Built a new floor");
                }
                else
                {
                    // Cooldown 30s in case building fails (to prevent repeated attempts)
                    _timeForNewFloor = DateTime.Now.AddSeconds(30);
                    mainWindow.Log("Not enough coins for a new floor");
                }
                MoveUp();
            }
        }
        else
        {
            mainWindow.Log("Too early to build a floor");
            WaitSec(1);
        }
    }

    public void RebuildTower()
    {
        mainWindow.Log("Rebuilding the tower");
        configManager.SaveStatRebuildTime();
        inputSim.SendClick(305, 570);
        WaitSec(1);
        inputSim.SendClick(165, 435);
        WaitMs(500);
        inputSim.SendClick(165, 440);
        WaitMs(500);
        inputSim.SendClick(230, 380);
        WaitMs(500);
        inputSim.SendClick(230, 380);
        //WaitSec(1);
        //inputSim.SendClick(170, 444); // Claim independence day bonus
        configManager.SetCurrentFloor(1);
    }

    public void PassTheTutorial()
    {
        mainWindow.Log("Passing the tutorial");
        WaitMs(1000);
        inputSim.SendClick(170, 435); // Continue
        WaitMs(500);
        MoveDown();
        WaitMs(1500);
        inputSim.SendClick(195, 260); // Build a new floor
        WaitMs(500);
        inputSim.SendClick(230, 380); // Confirm
        WaitMs(500);
        inputSim.SendClick(20, 60);   // Complete quest
        WaitMs(500);
        inputSim.SendClick(170, 435); // Collect bux
        WaitMs(500);
        inputSim.SendClick(170, 435); // Continue
        WaitMs(500);
        inputSim.SendClick(190, 300); // Click on a new floor
        WaitMs(500);
        inputSim.SendClick(240, 150); // Build a residential floor
        WaitMs(500);
        inputSim.SendClick(160, 375); // Continue
        WaitMs(500);
        inputSim.SendClick(20, 60);   // Complete quest
        WaitMs(500);
        inputSim.SendClick(170, 435); // Collect bux
        WaitMs(500);
        inputSim.SendClick(170, 435); // Continue
        WaitMs(1500);                 // Do not make this less than 1s, the elevator won't be there in less time 
        inputSim.SendClick(21, 510);  // Click on the elevator button
        WaitSec(4);
        
        // It's possible that the daily rent reward will interfere with the current tutorial completion, hence the check
        if (IsImageFound("freeBuxCollectButton", out Point location))
        {
            inputSim.SendClick(location.X, location.Y); // Collect daily rent
            WaitMs(500);
            inputSim.SendClick(21, 510);  // Click on elevator button again
            WaitSec(4);
        }
        
        inputSim.SendClick(230, 380); // Continue
        WaitMs(500);
        inputSim.SendClick(20, 60);   // Complete quest
        WaitMs(500);
        inputSim.SendClick(170, 435); // Collect bux
        WaitMs(500);
        inputSim.SendClick(170, 435); // Continue
        WaitMs(500);
        inputSim.SendClick(190, 200); // Build a new floor
        WaitMs(500);
        inputSim.SendClick(225, 380); // Confirm
        WaitMs(500);
        inputSim.SendClick(200, 200); // Open the new floor
        WaitMs(500);
        inputSim.SendClick(90, 340);  // Build random food floor
        WaitMs(500);
        inputSim.SendClick(170, 375); // Continue
        WaitMs(500);
        inputSim.SendClick(20, 60);   // Complete the quest
        WaitMs(500);
        inputSim.SendClick(170, 435); // Collect bux
        WaitMs(500);
        inputSim.SendClick(170, 435); // Continue
        WaitMs(500);
        inputSim.SendClick(200, 200); // Open food floor
        WaitMs(500);
        inputSim.SendClick(75, 210);  // Open the hire menu
        WaitMs(500);
        inputSim.SendClick(80, 100);  // Select our bitizen
        WaitMs(500);
        inputSim.SendClick(230, 380); // Hire him
        WaitMs(500);
        inputSim.SendClick(160, 380); // Continue on dream job assignement
        WaitMs(500);
        inputSim.SendClick(300, 560); // Exit the food store
        WaitMs(500);
        inputSim.SendClick(20, 60);   // Complete the quest
        WaitMs(500);
        inputSim.SendClick(170, 435); // Collect bux
        WaitMs(500);
        inputSim.SendClick(170, 435); // Continue
        WaitMs(500);
        inputSim.SendClick(200, 200); // Open the food store again
        WaitMs(500);
        inputSim.SendClick(200, 210); // Request restock of the first item in the store
        WaitSec(15);
        inputSim.SendClick(305, 190); // Press restock button
        WaitMs(500);
        inputSim.SendClick(20, 60);   // Complete the quest
        WaitMs(500);
        inputSim.SendClick(170, 435); // Collect bux
        WaitMs(500);
        inputSim.SendClick(170, 435); // Continue
        WaitMs(500);
        inputSim.SendClick(200, 200); // Open food store again
        WaitMs(500);
        inputSim.SendClick(170, 130); // Click upgrade
        WaitMs(500);
        inputSim.SendClick(230, 375); // Confirm
        WaitMs(500);
        inputSim.SendClick(165, 375); // Continue
        WaitMs(500);
        inputSim.SendClick(300, 560); // Exit the food store
        WaitMs(500);
        inputSim.SendClick(20, 60);   // Complete the quest
        WaitMs(500);
        inputSim.SendClick(170, 435); // Collect bux
        WaitMs(500);
        inputSim.SendClick(170, 435); // Collect more bux
        //WaitMs(500);
        //inputSim.SendClick(165, 375); // Continue
        configManager.SetCurrentFloor(3);
    }

    public void RestartGame()
    {
        mainWindow.Log("Restarting the app");
        inputSim.SendEscapeButton();
        WaitSec(1);
        inputSim.SendClick(230, 380);
    }

    public void TryWatchAds()
    {
        if (configManager.curConfig.CurrentFloor >= _screenScanner.floorToStartWatchingAds)
        {
            mainWindow.Log("Watching the advertisement");
            inputSim.SendClick(225, 375);
            WaitSec(20);
        }
        else
        {
            inputSim.SendClick(105, 380); // Decline the video offer
        }
    }

    public void MoveUp()
    {
        inputSim.SendClick(22, 10);
        WaitSec(1);
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

    public int PlayRaffle(int lastRaffleTime)
    {
        if (lastRaffleTime != DateTime.Now.Hour)
        {
            mainWindow.Log("Playing the raffle");
            WaitMs(500);
            inputSim.SendClick(300, 570);
            WaitMs(500);
            inputSim.SendClick(275, 440);
            WaitSec(2);
            inputSim.SendClick(165, 375);
            return DateTime.Now.Hour;
        }
        else
        {
            return lastRaffleTime;
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

    void WaitSec(int seconds)
    {
        int ms = seconds * 1000;
        Task.Delay(ms).Wait();
    }

    void WaitMs(int milliseconds)
    {
        Task.Delay(milliseconds).Wait();
    }

    /// <summary>
    /// Checks if the image is on the game screen.
    /// </summary>
    /// <param name="imageKey">Image name from the button_names.txt</param>
    /// <returns>true if the image is found within the screen</returns>  
    bool IsImageFound(string imageKey)
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

    /// <summary>
    /// Checks if the image is on the game screen.
    /// </summary>
    /// <param name="imageKey">Image name from the button_names.txt</param>
    /// <param name="location">OpenCvSharp.Point stuct with coordinates of the specified image, in case the image is found</param>
    /// <returns>true if the image is found within the screen</returns>
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

    /// <summary>
    /// Calculates the floor prices 1 through the desired floor to rebuild at +1
    /// </summary>
    /// <returns>Private Dictionary<int, int> where key is the floor number and value is the floor price</returns>
    private Dictionary<int, int> CalculateFloorPrices()
    {
        var dict = new Dictionary<int, int>();

        // Floors 1 through 9 cost 5000
        for (int i = 1; i <= 9; i++)
        {
            dict.Add(i, 5000);
        }

        // Calculate the prices for floors 10 through 50+
        for (int i = 10; i <= _screenScanner.floorToRebuildAt + 1; i++)
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
        images.Clear();
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
