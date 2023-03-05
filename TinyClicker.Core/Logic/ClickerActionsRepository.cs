using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using ImageMagick;
using TinyClicker.Core.Extensions;
using TinyClicker.Core.Helpers;
using TinyClicker.Core.Logging;
using Point = OpenCvSharp.Point;

namespace TinyClicker.Core.Logic;

public class ClickerActionsRepository
{
    private readonly ConfigManager _configManager;
    private readonly ImageToText _imageToText;
    private readonly ImageEditor _imageEditor;
    private readonly ILogger _logger;

    public ScreenScanner _screenScanner;
    public InputSimulator _inputSim;

    Dictionary<int, int> _floorPrices = new();
    DateTime _timeForNewFloor;
    bool _floorPricesCalculated = false;

    public ClickerActionsRepository(
        ConfigManager configManager,
        InputSimulator inputSimulator,
        ImageToText imageToText,
        ImageEditor imageEditor,
        ILogger logger)
    {
        _configManager = configManager;
        _logger = logger;
        _inputSim = inputSimulator;
        _imageToText = imageToText;
        _imageEditor = imageEditor;
    }

    public void Init(ScreenScanner screenScanner)
    {
        _screenScanner = screenScanner;
        _inputSim.Init(screenScanner);

        _timeForNewFloor = DateTime.Now;
    }

    #region Clicker Actions

    public void CancelHurryConstruction()
    {
        _logger.Log("Exiting the construction menu");

        _inputSim.SendClick(100, 375); // Cancel action
        WaitSec(1);
    }

    public void CollectFreeBux()
    {
        _logger.Log("Collecting free bux");
        _inputSim.SendClick(_screenScanner.FoundImages["freeBuxCollectButton"]);
    }

    public void ClickOnChute()
    {
        _logger.Log("Clicking on the parachute");
        _inputSim.SendClick(_screenScanner.FoundImages[Button.GiftChute.GetName()]);
        WaitMs(500);
        if (IsImageFound(GameWindow.WatchAdPromptCoins) && _configManager.CurrentConfig.CurrentFloor >= _screenScanner.FloorToStartWatchingAds)
        {
            TryWatchAds();
        }
        else if (_screenScanner.AcceptBuxVideoOffers && _configManager.CurrentConfig.CurrentFloor >= _screenScanner.FloorToStartWatchingAds)
        {
            if (IsImageFound(GameWindow.WatchAdPromptBux))
            {
                TryWatchAds();
            }
        }
        else
        {
            _inputSim.SendClick(155, 380); // Decline the video offer
        }
    }

    public void CloseAd()
    {
        _logger.Log("Closing the advertisement");

        _inputSim.SendClick(22, 22);
        _inputSim.SendClick(311, 22);
        _inputSim.SendClick(310, 10);
        _inputSim.SendClick(310, 41);
        _inputSim.SendClick(311, 22);
        _inputSim.SendClick(302, 52);
        _inputSim.SendClick(319, 15);
        _inputSim.SendClick(317, 15);

        CheckForLostAdsReward();
    }

    public void CloseChuteNotification()
    {
        _logger.Log("Closing the parachute notification");
        _inputSim.SendClick(165, 375); // Close the notification
    }

    public void ExitRoofCustomizationMenu()
    {
        _logger.Log("Exiting the roof customization menu");
        PressExitButton();
        WaitMs(500);
    }

    public void PressContinue()
    {
        _logger.Log("Clicking continue");

        _inputSim.SendClick(_screenScanner.FoundImages[Button.Continue.GetName()]);
        WaitMs(500);
        MoveUp();
        WaitSec(1);
    }

    public void Restock()
    {
        _logger.Log("Restocking");
        MoveDown();
        WaitMs(500);
        _inputSim.SendClick(100, 480); // Stock all
        WaitMs(500);
        _inputSim.SendClick(225, 375);
        WaitMs(500);

        if (IsImageFound(Button.FullyStockedBonus))
        {
            _inputSim.SendClick(165, 375); // Close the bonus tooltip
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
        _logger.Log("Pressing free bux icon");
        _inputSim.SendClick(25, 130);
        WaitSec(1);
        _inputSim.SendClick(230, 375);
        WaitSec(1);
    }

    public void RideElevator()
    {
        _logger.Log("Riding the elevator");
        _inputSim.SendClick(21, 510);
        WaitSec(1);

        if (IsImageFound(Button.BackButton))
        {
            PressExitButton();
        }
        else if (IsImageFound("continueButton", out Point location))
        {
            _inputSim.SendClick(location.X, location.Y); // Click continue in case a new bitizen moved in
        }
        else
        {
            int curFloor = _configManager.CurrentConfig.CurrentFloor;
            WaitMs(curFloor * 100); // Wait for the ride to finish
            if (IsImageFound(Button.GiftChute))
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
        _logger.Log("Clicking on the quest button");
        _inputSim.SendClick(_screenScanner.FoundImages[Button.QuestButton.GetName()]);
        WaitMs(500);
        if (IsImageFound(GameWindow.DeliverBitizens))
        {
            DeliverBitizens();
        }
        else if (IsImageFound(GameWindow.FindBitizens))
        {
            FindBitizens();
        }
        else
        {
            _inputSim.SendClick(90, 440); // Skip the quest
            WaitMs(500);
            _inputSim.SendClick(230, 380); // Confirm
        }
    }

    public void FindBitizens()
    {
        _logger.Log("Skipping the quest");
        _inputSim.SendClick(95, 445); // Skip the quest
        WaitMs(500);
        _inputSim.SendClick(225, 380); // Confirm skip
    }

    public void DeliverBitizens()
    {
        _logger.Log("Delivering bitizens");
        _inputSim.SendClick(230, 440); // Continue
    }

    public void OpenTheGame()
    {
        WaitSec(1);
        _inputSim.SendClick(_screenScanner.FoundImages["gameIcon"]);
        WaitSec(7);
    }

    public void CheckForLostAdsReward()
    {
        WaitMs(500);
        if (IsImageFound(GameWindow.AdsLostReward))
        {
            _inputSim.SendClick(240, 344); // Click "Keep watching"
            WaitSec(15);
        }
        else
        {
            _inputSim.SendClick(240, 344);
        }
    }

    public void CheckForExitButton()
    {
        if (IsImageFound(Button.BackButton))
        {
            PressExitButton();
        }
    }

    public void CloseNewFloorMenu()
    {
        _logger.Log("Exiting from new floor menu");
        PressExitButton();
    }

    public void CloseBuildNewFloorNotification()
    {
        _logger.Log("Closing the new floor notification");
        _inputSim.SendClick(105, 320); // Click no
    }

    public void CompleteQuest()
    {
        _logger.Log("Completing the quest");

        WaitMs(500);
        _inputSim.SendClick(_screenScanner.FoundImages["completedQuestButton"]);
    }

    public void CollectNewScience()
    {
        _logger.Log("Collecting new science");
        _inputSim.SendClick(_screenScanner.FoundImages[Button.NewScience.GetName()]);
        WaitMs(500);
        _inputSim.SendClick(150, 110);
        WaitMs(300);
        PressExitButton();
        WaitMs(300);
        PressExitButton();
    }

    public void CheckForNewFloor(int currentFloor, Image gameWindow)
    {
        if (!_configManager.CurrentConfig.BuildFloors)
        {
            return;
        }

        MoveUp();
        if (!_floorPricesCalculated)
        {
            _floorPrices = CalculateFloorPrices();
            _floorPricesCalculated = true;
        }

        int balance = _imageToText.ParseBalance(gameWindow);
        if (balance != -1 && currentFloor >= 3)
        {
            if (currentFloor >= _screenScanner.FloorToRebuildAt)
            {
                WaitMs(500);
                RebuildTower();
                return;
            }

            int targetPrice = _floorPrices[currentFloor + 1];
            if (balance > targetPrice && currentFloor < _screenScanner.FloorToRebuildAt)
            {
                BuildNewFloor();


                if (IsImageFound(GameWindow.NewFloorNoCoinsNotification))
                {
                    _timeForNewFloor = DateTime.Now.AddSeconds(10);
                    _logger.Log("Not enough coins for a new floor");
                    _inputSim.SendClick(230, 380); // continue
                    return;
                }

                if (IsImageFound(Button.BackButton))
                {
                    return;
                }
                else
                {
                    // Allow consecutive building of new floors if there is enough coins
                    using var newGameWindow = _inputSim.MakeScreenshot();

                    _logger.Log("Trying to build new floor");
                    CheckForNewFloor(_configManager.CurrentConfig.CurrentFloor, newGameWindow);
                }
            }
        }
    }

    public void BuildNewFloor()
    {
        if (_timeForNewFloor <= DateTime.Now)
        {
            if (IsImageFound(Button.Continue))
            {
                _inputSim.SendClick(230, 380); // Continue
                return;
            }

            if (IsImageFound(Button.BackButton))
            {
                PressExitButton();
                _logger.Log("Clicking Back button while building");
                return;
            }

            _logger.Log("Building new floor");
            MoveUp();

            _inputSim.SendClick(165, 345); // Click on a new floor
            WaitMs(500);

            if (IsImageFound(Button.Continue))
            {
                _inputSim.SendClick(160, 380); // Click "No thanks" on chute notification
                _logger.Log("Clicking Continue button while building");
            }
            else if (IsImageFound(GameWindow.BuildNewFloorNotification))
            {
                _inputSim.SendClick(230, 320); // Confirm construction
                WaitMs(500);

                // Add a new floor if there is enough coins
                if (!IsImageFound(GameWindow.NewFloorNoCoinsNotification))
                {
                    _configManager.AddOneFloor();
                    _logger.Log("Built a new floor");
                }
                else
                {
                    // Cooldown 10s in case building fails (to prevent repeated attempts)
                    _timeForNewFloor = DateTime.Now.AddSeconds(10);
                    _logger.Log("Not enough coins for a new floor");
                    //_inputSim.SendClick(230, 380); // continue
                }

                MoveUp();
            }
        }
        else
        {
            _logger.Log("Too early to build a floor");
            WaitSec(1);
            return;
        }
    }

    public void RebuildTower()
    {
        _logger.Log("Rebuilding the tower");
        _configManager.SaveStatRebuildTime();
        _inputSim.SendClick(305, 570);
        WaitSec(1);
        _inputSim.SendClick(165, 435);
        WaitMs(500);
        _inputSim.SendClick(165, 440);
        WaitMs(500);
        _inputSim.SendClick(230, 380);
        WaitMs(500);
        _inputSim.SendClick(230, 380);
        _configManager.SetCurrentFloor(1);
    }

    public void PassTheTutorial()
    {
        _logger.Log("Passing the tutorial");
        WaitMs(1000);
        _inputSim.SendClick(170, 435); // Continue
        WaitMs(500);
        MoveDown();
        WaitMs(1500);
        _inputSim.SendClick(195, 260); // Build a new floor
        WaitMs(500);
        _inputSim.SendClick(230, 380); // Confirm
        WaitMs(500);
        _inputSim.SendClick(20, 60);   // Complete quest
        WaitMs(500);
        _inputSim.SendClick(170, 435); // Collect bux
        WaitMs(500);
        _inputSim.SendClick(170, 435); // Continue
        WaitMs(500);
        _inputSim.SendClick(190, 300); // Click on a new floor
        WaitMs(500);
        _inputSim.SendClick(240, 150); // Build a residential floor
        WaitMs(500);
        _inputSim.SendClick(160, 375); // Continue
        WaitMs(500);
        _inputSim.SendClick(20, 60);   // Complete quest
        WaitMs(500);
        _inputSim.SendClick(170, 435); // Collect bux
        WaitMs(500);
        _inputSim.SendClick(170, 435); // Continue
        WaitMs(1500);                  // Not less than 1000ms, the elevator won't be there in less time 
        _inputSim.SendClick(21, 510);  // Click on the elevator button
        WaitSec(4);

        // Daily rent check (in case it's past midnight)
        if (IsImageFound("freeBuxCollectButton", out Point location))
        {
            _inputSim.SendClick(location.X, location.Y); // Collect daily rent
            WaitMs(500);
            _inputSim.SendClick(21, 510);  // Click on elevator button again
            WaitSec(4);
        }

        _inputSim.SendClick(230, 380); // Continue
        WaitMs(500);
        _inputSim.SendClick(20, 60);   // Complete quest
        WaitMs(500);
        _inputSim.SendClick(170, 435); // Collect bux
        WaitMs(500);
        _inputSim.SendClick(170, 435); // Continue
        WaitMs(500);
        _inputSim.SendClick(190, 200); // Build a new floor
        WaitMs(500);
        _inputSim.SendClick(225, 380); // Confirm
        WaitMs(500);
        _inputSim.SendClick(200, 200); // Open the new floor
        WaitMs(500);
        _inputSim.SendClick(90, 340);  // Build random food floor
        WaitMs(500);
        _inputSim.SendClick(170, 375); // Continue
        WaitMs(500);
        _inputSim.SendClick(20, 60);   // Complete the quest
        WaitMs(500);
        _inputSim.SendClick(170, 435); // Collect bux
        WaitMs(500);
        _inputSim.SendClick(170, 435); // Continue
        WaitMs(500);
        _inputSim.SendClick(200, 200); // Open food floor
        WaitMs(500);
        _inputSim.SendClick(75, 210);  // Open the hire menu
        WaitMs(500);
        _inputSim.SendClick(80, 100);  // Select our bitizen
        WaitMs(500);
        _inputSim.SendClick(230, 380); // Hire him
        WaitMs(500);
        _inputSim.SendClick(160, 380); // Continue on dream job assignement
        WaitMs(500);
        _inputSim.SendClick(300, 560); // Exit the food store
        WaitMs(500);
        _inputSim.SendClick(20, 60);   // Complete the quest
        WaitMs(500);
        _inputSim.SendClick(170, 435); // Collect bux
        WaitMs(500);
        _inputSim.SendClick(170, 435); // Continue
        WaitMs(500);
        _inputSim.SendClick(200, 200); // Open the food store again
        WaitMs(500);
        _inputSim.SendClick(200, 210); // Request restock of the first item in the store
        WaitSec(15);
        _inputSim.SendClick(305, 190); // Press restock button
        WaitMs(500);
        _inputSim.SendClick(20, 60);   // Complete the quest
        WaitMs(500);
        _inputSim.SendClick(170, 435); // Collect bux
        WaitMs(500);
        _inputSim.SendClick(170, 435); // Continue
        WaitMs(500);
        _inputSim.SendClick(200, 200); // Open food store again
        WaitMs(500);
        _inputSim.SendClick(170, 130); // Click upgrade
        WaitMs(500);
        _inputSim.SendClick(230, 375); // Confirm
        WaitMs(500);
        _inputSim.SendClick(165, 375); // Continue
        WaitMs(500);
        _inputSim.SendClick(300, 560); // Exit the food store
        WaitMs(500);
        _inputSim.SendClick(20, 60);   // Complete the quest
        WaitMs(500);
        _inputSim.SendClick(170, 435); // Collect bux
        WaitMs(500);
        _inputSim.SendClick(170, 435); // Collect more bux
    }

    public void RestartGame()
    {
        _logger.Log("Restarting the app");
        _inputSim.SendEscapeButton();
        WaitSec(1);
        _inputSim.SendClick(230, 380);
    }

    public void TryWatchAds()
    {
        if (_configManager.CurrentConfig.CurrentFloor >= _screenScanner.FloorToStartWatchingAds)
        {
            _logger.Log("Watching the advertisement");
            _inputSim.SendClick(225, 375);
            WaitSec(20);
        }
        else
        {
            _inputSim.SendClick(105, 380); // Decline the video offer
        }
    }

    public void MoveUp()
    {
        _inputSim.SendClick(22, 10);
        WaitSec(1);
    }

    public void MoveDown()
    {
        _inputSim.SendClick(230, 580);
    }

    public void PressExitButton()
    {
        _logger.Log("Pressing the exit button");
        _inputSim.SendClick(305, 565);
    }

    public void PlayRaffle()
    {
        var lastRaffleTime = _configManager.CurrentConfig.LastRaffleTime;
        var dateTimeNow = DateTime.Now;
        if (lastRaffleTime > dateTimeNow.AddHours(-1))
        {
            return;
        }

        if (IsImageFound(Button.Continue))
        {
            _inputSim.SendClick(160, 380); // Continue
        }

        _logger.Log("Playing the raffle");
        WaitMs(500);
        _inputSim.SendClick(300, 570); // Open menu
        WaitMs(500);
        _inputSim.SendClick(275, 440); // Open raffle
        WaitSec(2);
        _inputSim.SendClick(160, 345); // Enter raffle

        _configManager.CurrentConfig.LastRaffleTime = dateTimeNow;
    }

    #endregion

    #region Utility Methods

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
    public bool IsImageFound(Enum image, Dictionary<string, Mat>? templates = null, Image? screenshot = null)
    {
        Image gameWindow = screenshot == null ? _inputSim.MakeScreenshot() : screenshot;
        var windowBitmap = new Bitmap(gameWindow);
        gameWindow.Dispose();
        Mat reference = windowBitmap.ToMat();
        windowBitmap.Dispose();

        var template = templates == null ? _screenScanner.Templates[image.GetName()] : templates[image.GetName()];

        using (Mat res = new(reference.Rows - template.Rows + 1, reference.Cols - template.Cols + 1, MatType.CV_8S))
        {
            Mat gref = reference.CvtColor(ColorConversionCodes.BGR2GRAY);
            Mat gtpl = template.CvtColor(ColorConversionCodes.BGR2GRAY);

            Cv2.MatchTemplate(gref, gtpl, res, TemplateMatchModes.CCoeffNormed);
            Cv2.Threshold(res, res, 0.7, 1.0, ThresholdTypes.Tozero);

            double threshold = 0.7;
            Cv2.MinMaxLoc(res, out double minval, out double maxval, out Point minloc, out Point maxloc);

            return maxval >= threshold;
        }
    }

    /// <summary>
    /// Checks if the image is on the game screen and returns image location.
    /// </summary>
    /// <param name="imageKey">Image name from the button_names.txt</param>
    /// <param name="location">OpenCvSharp.Point stuct with coordinates of the specified image, in case the image is found</param>
    /// <returns>true if the image is found within the screen</returns>
    public bool IsImageFound(string imageKey, out Point location)
    {
        Image gameWindow = _inputSim.MakeScreenshot();
        var windowBitmap = new Bitmap(gameWindow);
        gameWindow.Dispose();
        Mat reference = windowBitmap.ToMat();
        windowBitmap.Dispose();

        var template = _screenScanner.Templates[imageKey];
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
        for (int i = 10; i <= _screenScanner.FloorToRebuildAt + 1; i++)
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

    private Dictionary<string, Image> GetSamples()
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

    public Dictionary<string, Mat> MakeTemplates(Image screenshot)
    {
        var images = GetSamples();
        var percentage = _imageEditor.GetScreenDiffPercentageForTemplates(screenshot);

        var mats = new Dictionary<string, Mat>();

        foreach (var image in images)
        {
            // Resize images before making templates
            using (var imageOld = new MagickImage(_imageEditor.ImageToBytes(image.Value), MagickFormat.Png))
            {
                imageOld.Resize(percentage.x, percentage.y);
                var imageBitmap = new Bitmap(_imageEditor.BytesToImage(imageOld.ToByteArray()));
                var template = imageBitmap.ToMat();

                imageBitmap.Dispose();
                mats.Add(image.Key, template);
            }
        }

        images.Clear();
        return mats;
    }

    #endregion
}
