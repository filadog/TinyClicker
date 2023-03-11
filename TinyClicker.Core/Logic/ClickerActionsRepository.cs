using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using ImageMagick;
using TinyClicker.Core.Logging;
using TinyClicker.Core.Services;
using System.Linq;

namespace TinyClicker.Core.Logic;

public class ClickerActionsRepository
{
    private readonly IConfigService _configService;
    private readonly IWindowsApiService _windowsApiService;
    private readonly IImageService _imageService;
    private readonly ILogger _logger;

    public ClickerActionsRepository(
        IConfigService configService,
        IWindowsApiService windowsApiService,
        IImageService imageService,
        ILogger logger)
    {
        _configService = configService;
        _windowsApiService = windowsApiService;
        _imageService = imageService;
        _logger = logger;
    }

    public Dictionary<int, int> FloorPrices { get; private set; }
    private DateTime TimeForNewFloor { get; set; } = DateTime.Now;
    private Dictionary<string, Mat> Templates { get; set; } = new();

    #region Clicker Actions

    public void CancelHurryConstruction()
    {
        _logger.Log("Exiting the construction menu");
        ClickAndWaitSec(100, 375, 1); // Cancel action
    }

    public void CollectFreeBux(int location)
    {
        _logger.Log("Collecting free bux");
        _windowsApiService.SendClick(location);
    }

    public void ClickOnChute(int location)
    {
        _logger.Log("Clicking on the parachute");
        _windowsApiService.SendClick(location);
        WaitMs(500);
        if (IsImageFound(GameWindow.WatchAdPromptCoins) && _configService.Config.CurrentFloor >= _configService.Config.WatchAdsFromFloor)
        {
            TryWatchAds();
        }
        else if (_configService.Config.WatchBuxAds && _configService.Config.CurrentFloor >= _configService.Config.WatchAdsFromFloor)
        {
            if (IsImageFound(GameWindow.WatchAdPromptBux))
            {
                TryWatchAds();
            }
        }
        else
        {
            _windowsApiService.SendClick(155, 380); // Decline the video offer
        }
    }

    public void CloseAd()
    {
        _logger.Log("Closing the advertisement");

        _windowsApiService.SendClick(22, 22);
        _windowsApiService.SendClick(311, 22);
        _windowsApiService.SendClick(310, 10);
        _windowsApiService.SendClick(310, 41);
        _windowsApiService.SendClick(311, 22);
        _windowsApiService.SendClick(302, 52);
        _windowsApiService.SendClick(319, 15);
        _windowsApiService.SendClick(317, 15);

        CheckForLostAdsReward();
    }

    public void CloseChuteNotification()
    {
        _logger.Log("Closing the parachute notification");
        _windowsApiService.SendClick(165, 375); // Close the notification
    }

    public void ExitRoofCustomizationMenu()
    {
        _logger.Log("Exiting the roof customization menu");
        PressExitButton();
        WaitMs(500);
    }

    public void PressContinue(int location)
    {
        _logger.Log("Clicking continue");

        _windowsApiService.SendClick(location);
        WaitMs(500);
        MoveUp();
        WaitSec(1);
    }

    public void Restock()
    {
        _logger.Log("Restocking");
        MoveDown();
        WaitMs(500);
        ClickAndWaitMs(100, 480, 500); // Stock all
        ClickAndWaitMs(225, 375, 500); 

        if (IsImageFound(Button.FullyStockedBonus))
        {
            ClickAndWaitSec(165, 375, 1); // Close the bonus tooltip
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
        ClickAndWaitSec(25, 130, 1);
        ClickAndWaitSec(230, 375, 1);
    }

    public void RideElevator()
    {
        _logger.Log("Riding the elevator");
        ClickAndWaitSec(21, 510, 1); // Move up

        if (IsImageFound(Button.BackButton))
        {
            PressExitButton();
        }
        else if (IsImageFound(Button.Continue, out OpenCvSharp.Point location))
        {
            // Click continue in case a new bitizen moved in
            _windowsApiService.SendClick(location.X, location.Y); 
        }
        else
        {
            int curFloor = _configService.Config.CurrentFloor;
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

    public void PressQuestButton(int location)
    {
        _logger.Log("Clicking on the quest button");
        _windowsApiService.SendClick(location);
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
            ClickAndWaitMs(90, 440, 500); // Skip the quest
            ClickAndWaitMs(230, 380, 0);  // Confirm
        }
    }

    public void FindBitizens()
    {
        _logger.Log("Skipping the quest");
        ClickAndWaitMs(95, 445, 500); // Skip the quest
        ClickAndWaitMs(225, 380, 0);  // Confirm skip
    }

    public void DeliverBitizens()
    {
        _logger.Log("Delivering bitizens");
        _windowsApiService.SendClick(230, 440); // Continue
    }

    public void OpenTheGame(int location)
    {
        WaitSec(1);
        _windowsApiService.SendClick(location);
        WaitSec(7);
    }

    public void CheckForLostAdsReward()
    {
        WaitMs(500);
        if (IsImageFound(GameWindow.AdsLostReward))
        {
            _windowsApiService.SendClick(240, 344); // Click "Keep watching"
            WaitSec(15);
        }
        else
        {
            _windowsApiService.SendClick(240, 344);
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
        _windowsApiService.SendClick(105, 320); // Click no
    }

    public void CompleteQuest(int location)
    {
        _logger.Log("Completing the quest");

        WaitMs(500);
        _windowsApiService.SendClick(location);
    }

    public void CollectNewScience(int location)
    {
        _logger.Log("Collecting new science");
        _windowsApiService.SendClick(location);
        WaitMs(500);
        ClickAndWaitMs(150, 110, 300);
        PressExitButton();
        WaitMs(300);
        PressExitButton();
    }

    public void CheckForNewFloor(int currentFloor, Image gameWindow)
    {
        if (!_configService.Config.BuildFloors)
        {
            return;
        }

        if (FloorPrices == null)
        {
            FloorPrices = CalculateFloorPrices();
        }

        var balance = _imageService.GetBalanceFromWindow(gameWindow);
        if (balance != -1 && currentFloor >= 3)
        {
            if (currentFloor >= _configService.Config.RebuildAtFloor)
            {
                WaitMs(500);
                RebuildTower();
                return;
            }

            if (balance > FloorPrices[currentFloor + 1] && currentFloor < _configService.Config.RebuildAtFloor)
            {
                MoveUp();

                if (IsImageFound(Button.ElevatorButton))
                {
                    RideElevator();
                }

                BuildNewFloor();

                if (IsImageFound(GameWindow.NewFloorNoCoinsNotification))
                {
                    TimeForNewFloor = DateTime.Now.AddSeconds(10);
                    _logger.Log("Not enough coins for a new floor");
                    _windowsApiService.SendClick(230, 380); // continue
                    return;
                }

                if (IsImageFound(Button.BackButton))
                {
                    return;
                }
                else
                {
                    // Allow consecutive building of new floors if there is enough coins
                    using var newGameWindow = _windowsApiService.MakeScreenshot();
                    CheckForNewFloor(_configService.Config.CurrentFloor, newGameWindow);
                }
            }
        }
    }

    public void BuildNewFloor()
    {
        if (TimeForNewFloor <= DateTime.Now)
        {
            if (IsImageFound(Button.Continue))
            {
                _windowsApiService.SendClick(230, 380); // Continue
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
            ClickAndWaitMs(300, 360, 500); // Click on a new floor

            if (IsImageFound(Button.Continue))
            {
                _windowsApiService.SendClick(160, 380); // Click "No thanks" on chute notification
                _logger.Log("Clicking Continue button while building");
            }
            else if (IsImageFound(GameWindow.BuildNewFloorNotification))
            {
                _windowsApiService.SendClick(230, 320); // Confirm construction
                WaitMs(500);

                // Add a new floor if there is enough coins
                if (!IsImageFound(GameWindow.NewFloorNoCoinsNotification))
                {
                    _configService.AddOneFloor();
                    _logger.Log("Built a new floor");
                }
                else
                {
                    // Cooldown 10s in case building fails (to prevent repeated attempts)
                    TimeForNewFloor = DateTime.Now.AddSeconds(10);
                    _logger.Log("Not enough coins for a new floor");
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
        _configService.SaveStatRebuildTime();

        ClickAndWaitSec(305, 570, 1);
        ClickAndWaitMs(165, 435, 500);
        ClickAndWaitMs(165, 440, 500);
        ClickAndWaitMs(230, 380, 500);
        ClickAndWaitMs(230, 380, 0);

        _configService.SetCurrentFloor(1);
    }

    public void PassTheTutorial()
    {
        _logger.Log("Passing the tutorial");
        WaitSec(1);
        ClickAndWaitMs(170, 435, 500); // Continue
        MoveDown();
        WaitMs(1500);

        ClickAndWaitMs(195, 260, 500); // Build a new floor
        ClickAndWaitMs(230, 380, 500); // Confirm
        ClickAndWaitMs(20, 60, 500);   // Complete quest
        ClickAndWaitMs(170, 435, 500); // Collect bux
        ClickAndWaitMs(170, 435, 500); // Continue
        ClickAndWaitMs(190, 300, 500); // Click on a new floor
        ClickAndWaitMs(240, 150, 500); // Build a residential floor
        ClickAndWaitMs(160, 375, 500); // Continue
        ClickAndWaitMs(20, 60, 500);   // Complete quest
        ClickAndWaitMs(170, 435, 500); // Collect bux
        ClickAndWaitMs(170, 435, 1500);// Continue
        ClickAndWaitSec(21, 510, 4);   // Click on the elevator button

        // Daily rent check (in case it's past midnight)
        if (IsImageFound(Button.FreeBuxCollectButton, out OpenCvSharp.Point location))
        {
            ClickAndWaitMs(location.X, location.Y, 500); // Collect daily rent
            ClickAndWaitMs(21, 510, 4000); // Click on elevator button again
        }

        ClickAndWaitMs(230, 380, 500); // Continue
        ClickAndWaitMs(20, 60, 500);   // Complete quest
        ClickAndWaitMs(170, 435, 500); // Collect bux
        ClickAndWaitMs(170, 435, 500); // Continue
        ClickAndWaitMs(190, 200, 500); // Build a new floor
        ClickAndWaitMs(225, 380, 500); // Confirm
        ClickAndWaitMs(200, 200, 500); // Open the new floor
        ClickAndWaitMs(90, 340, 500);  // Build random food floor
        ClickAndWaitMs(170, 375, 500); // Continue
        ClickAndWaitMs(20, 60, 500);   // Complete the quest
        ClickAndWaitMs(170, 435, 500); // Collect bux
        ClickAndWaitMs(170, 435, 500); // Continue
        ClickAndWaitMs(200, 200, 500); // Open the food floor
        ClickAndWaitMs(75, 210, 500);  // Open the hire menu
        ClickAndWaitMs(80, 100, 500);  // Select bitizen
        ClickAndWaitMs(230, 380, 500); // Hire him
        ClickAndWaitMs(160, 380, 500); // Continue on dream job assignement
        ClickAndWaitMs(300, 560, 500); // Exit the food store
        ClickAndWaitMs(20, 60, 500);   // Complete the quest
        ClickAndWaitMs(170, 435, 500); // Collect bux
        ClickAndWaitMs(170, 435, 500); // Continue
        ClickAndWaitMs(200, 200, 500); // Open the food store again
        ClickAndWaitSec(200, 210, 15); // Request restock of the first item in the store
        ClickAndWaitMs(305, 190, 500); // Press restock button
        ClickAndWaitMs(20, 60, 500);   // Complete the quest
        ClickAndWaitMs(170, 435, 500); // Collect bux
        ClickAndWaitMs(170, 435, 500); // Continue
        ClickAndWaitMs(200, 200, 500); // Open food store again
        ClickAndWaitMs(170, 130, 500); // Click upgrade
        ClickAndWaitMs(230, 375, 500); // Confirm upgrade
        ClickAndWaitMs(165, 375, 500); // Continue
        ClickAndWaitMs(300, 560, 500); // Exit the food store
        ClickAndWaitMs(20, 60, 500);   // Complete the quest
        ClickAndWaitMs(170, 435, 500); // Collect bux
        ClickAndWaitMs(170, 435, 0);   // Collect more bux
    }

    public void RestartGame()
    {
        _logger.Log("Restarting the app");
        _windowsApiService.SendEscapeButton();
        WaitSec(1);
        _windowsApiService.SendClick(230, 380);
    }

    public void TryWatchAds()
    {
        if (_configService.Config.CurrentFloor >= _configService.Config.WatchAdsFromFloor)
        {
            _logger.Log("Watching the advertisement");
            ClickAndWaitSec(225, 375, 20);
        }
        else
        {
            _windowsApiService.SendClick(105, 380); // Decline the video offer
        }
    }

    public void MoveUp()
    {
        ClickAndWaitSec(22, 10, 1);
    }

    public void MoveDown()
    {
        _windowsApiService.SendClick(230, 580);
    }

    public void PressExitButton()
    {
        _logger.Log("Pressing the exit button");
        _windowsApiService.SendClick(305, 565);
    }

    public void PlayRaffle()
    {
        var lastRaffleTime = _configService.Config.LastRaffleTime;
        var dateTimeNow = DateTime.Now;
        if (lastRaffleTime > dateTimeNow.AddHours(-1))
        {
            return;
        }

        if (IsImageFound(Button.Continue))
        {
            _windowsApiService.SendClick(160, 380); // Continue
        }

        _logger.Log("Playing the raffle");
        WaitMs(500);
        ClickAndWaitMs(300, 570, 500); // Open menu
        ClickAndWaitMs(275, 440, 2000); // Open raffle
        _windowsApiService.SendClick(160, 345); // Enter raffle

        _configService.Config.LastRaffleTime = dateTimeNow;
    }

    #endregion

    #region Utility Methods

    private void WaitSec(int seconds)
    {
        int ms = seconds * 1000;
        Task.Delay(ms).Wait();
    }

    private void WaitMs(int milliseconds)
    {
        Task.Delay(milliseconds).Wait();
    }

    private void ClickAndWaitMs(int locationX, int locationY, int waitMs)
    {
        _windowsApiService.SendClick(locationX, locationY);
        WaitMs(waitMs);
    }

    private void ClickAndWaitSec(int locationX, int locationY, int waitSec)
    {
        _windowsApiService.SendClick(locationX, locationY);
        WaitSec(waitSec);
    }

    private HashSet<string> SkipButtons = new()
    {
        Button.GameIcon.GetName(),
        Button.BalanceCoin.GetName(),
        Button.RestockButton.GetName()
    };

    public Dictionary<string, int> TryFindFirstOnScreen(Image gameScreen)
    {
        if (!Templates.Any())
        {
            Templates = MakeTemplates(gameScreen);
        }

        var screenBitmap = new Bitmap(gameScreen);
        var screenMat = screenBitmap.ToMat();
        screenBitmap.Dispose();

        var result = new Dictionary<string, int>();
        foreach (var template in Templates)
        {
            if (SkipButtons.Contains(template.Key))
            {
                continue;
            }

            var item = TryFindSingle(template, screenMat);
            if (string.IsNullOrEmpty(item.Key))
            {
                Task.Delay(15).Wait(); // Smooth the CPU peak load
                continue;
            }

            result.Add(item.Key, item.Location);
            break;
        }

        screenMat.Dispose();
        return result;
    }

    public (string Key, int Location) TryFindSingle(KeyValuePair<string, Mat> template, Mat reference)
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
                Cv2.MinMaxLoc(res, out _, out double maxval, out _, out OpenCvSharp.Point maxloc);
                res.Dispose();

                if (maxval >= threshold)
                {
                    if (template.Key == Button.GiftChute.GetName())
                    {
                        return new(template.Key, _windowsApiService.MakeLParam(maxloc.X + 40, maxloc.Y + 40));
                    }
                    else
                    {
                        return new(template.Key, _windowsApiService.MakeLParam(maxloc.X, maxloc.Y + 10));
                    }
                }
                else
                {
                    return new("", 0);
                }
            }
        }
    }

    /// <summary>
    /// Checks if the image is on the game screen.
    /// </summary>
    /// <param name="imageKey">Image name from the button_names.txt</param>
    /// <returns>true if the image is found within the screen</returns>  
    public bool IsImageFound(Enum image, Dictionary<string, Mat>? templates = null, Image? screenshot = null)
    {
        var gameWindow = screenshot == null ? _windowsApiService.MakeScreenshot() : screenshot;
        var windowBitmap = new Bitmap(gameWindow);
        gameWindow.Dispose();
        var reference = windowBitmap.ToMat();
        windowBitmap.Dispose();

        var template = templates == null ? Templates[image.GetName()] : templates[image.GetName()];

        using (Mat res = new(reference.Rows - template.Rows + 1, reference.Cols - template.Cols + 1, MatType.CV_8S))
        {
            Mat gref = reference.CvtColor(ColorConversionCodes.BGR2GRAY);
            Mat gtpl = template.CvtColor(ColorConversionCodes.BGR2GRAY);

            Cv2.MatchTemplate(gref, gtpl, res, TemplateMatchModes.CCoeffNormed);
            Cv2.Threshold(res, res, 0.7, 1.0, ThresholdTypes.Tozero);

            double threshold = 0.7;
            Cv2.MinMaxLoc(res, out double minval, out double maxval, out OpenCvSharp.Point minloc, out OpenCvSharp.Point maxloc);

            return maxval >= threshold;
        }
    }

    /// <summary>
    /// Checks if the image is on the game screen and returns image location.
    /// </summary>
    /// <param name="image">Image name from the button_names.txt</param>
    /// <param name="location">OpenCvSharp.Point stuct with coordinates of the specified image, in case the image is found</param>
    /// <returns>true if the image is found within the screen</returns>
    public bool IsImageFound(Enum image, out OpenCvSharp.Point location)
    {
        var gameWindow = _windowsApiService.MakeScreenshot();
        var windowBitmap = new Bitmap(gameWindow);
        gameWindow.Dispose();
        var reference = windowBitmap.ToMat();
        windowBitmap.Dispose();

        var template = Templates[image.GetName()];
        using (Mat res = new(reference.Rows - template.Rows + 1, reference.Cols - template.Cols + 1, MatType.CV_8S))
        {
            Mat gref = reference.CvtColor(ColorConversionCodes.BGR2GRAY);
            Mat gtpl = template.CvtColor(ColorConversionCodes.BGR2GRAY);

            Cv2.MatchTemplate(gref, gtpl, res, TemplateMatchModes.CCoeffNormed);
            Cv2.Threshold(res, res, 0.7, 1.0, ThresholdTypes.Tozero);

            double threshold = 0.7;
            Cv2.MinMaxLoc(res, out double minval, out double maxval, out OpenCvSharp.Point minloc, out OpenCvSharp.Point maxloc);

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
        var result = new Dictionary<int, int>();

        // Floors 1 through 9 cost 5000
        for (int i = 1; i <= 9; i++)
        {
            result.Add(i, 5000);
        }

        // Calculate the prices for floors 10 through 50+
        for (int i = 10; i <= _configService.Config.RebuildAtFloor + 1; i++)
        {
            float floorCost = 1000 * 1 * (0.5f * (i * i) + 8 * i - 117);

            // Round up the result to match with the in-game prices
            if (i % 2 != 0)
            {
                floorCost += 500;
            }

            result.Add(i, (int)floorCost);
        }

        return result;
    }

    private byte[][] LoadButtonData()
    {
        string path = $"./Samples/samples.dat";
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
        string[] buttonNames = File.ReadAllLines($"./Samples/button_names.txt");
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
        var percentage = _imageService.GetScreenDiffPercentageForTemplates(screenshot);

        var mats = new Dictionary<string, Mat>();

        foreach (var image in images)
        {
            // Resize images before making templates
            using (var imageOld = new MagickImage(_imageService.ImageToBytes(image.Value), MagickFormat.Png))
            {
                imageOld.Resize(percentage.x, percentage.y);
                var imageBitmap = new Bitmap(_imageService.BytesToImage(imageOld.ToByteArray()));
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
