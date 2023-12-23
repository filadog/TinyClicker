using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TinyClicker.Core.Logging;
using TinyClicker.Core.Services;

namespace TinyClicker.Core.Logic;

public class ClickerActionsRepository
{
    private readonly IUserConfiguration _userConfiguration;
    private readonly IWindowsApiService _windowsApiService;
    private readonly IImageFinder _imageFinder;
    private readonly ILogger _logger;

    public ClickerActionsRepository(
        IUserConfiguration userConfiguration,
        IWindowsApiService windowsApiService,
        IImageFinder imageFinder,
        ILogger logger)
    {
        _userConfiguration = userConfiguration;
        _windowsApiService = windowsApiService;
        _imageFinder = imageFinder;
        _logger = logger;

        FloorPrices = CalculateFloorPrices();
    }

    private Dictionary<int, int> FloorPrices { get; }
    private DateTime AttemptNextFloorBuildAt { get; set; } = DateTime.Now;

    public void CancelHurryConstructionWithBux()
    {
        _logger.Log("Exiting the construction menu");
        ClickAndWaitSec(100, 375, 1); // cancel action
    }

    public void CollectFreeBux(int location)
    {
        _logger.Log("Collecting free bux");
        ClickAndWaitMs(location, 300);

        if (!_imageFinder.TryFindOnScreen(GameButton.FreeBuxGift, out var collectLocation))
        {
            return;
        }

        ClickAndWaitMs(collectLocation.X + 40, collectLocation.Y + 40, 300);
        _windowsApiService.SendClick(225, 375); // collect bux
    }

    public void ClickOnChute(int location)
    {
        _logger.Log("Clicking on the parachute");
        ClickAndWaitMs(location, 500);

        if (_imageFinder.IsImageOnScreen(GameWindow.WatchCoinsAdsPrompt)
            && _userConfiguration.CurrentFloor >= _userConfiguration.WatchAdsFromFloor)
        {
            TryWatchAds();
        }
        else if (_userConfiguration.WatchBuxAds
            && _userConfiguration.CurrentFloor >= _userConfiguration.WatchAdsFromFloor)
        {
            if (_imageFinder.IsImageOnScreen(GameWindow.WatchBuxAdsPrompt))
            {
                TryWatchAds();
            }
        }
        else
        {
            _windowsApiService.SendClick(155, 380); // decline the video offer
        }
    }

    public void TryCloseAd()
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

        CheckIfAdsRewardWillBeLost();
    }

    public void CloseChuteNotification()
    {
        _logger.Log("Closing the parachute notification");
        _windowsApiService.SendClick(165, 375); // close the notification
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
        MoveUp();
    }

    public void Restock()
    {
        _logger.Log("Restocking");
        MoveDown();
        WaitMs(500);
        ClickAndWaitMs(100, 480, 500); // stock all
        ClickAndWaitMs(225, 375, 500);

        if (_imageFinder.IsImageOnScreen(GameButton.FullyStockedBonus))
        {
            ClickAndWaitSec(165, 375, 1); // close the bonus tooltip
            MoveUp();
        }
        else
        {
            MoveUp();
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
        ClickAndWaitSec(21, 510, 1); // move up

        if (_imageFinder.IsImageOnScreen(GameButton.Back))
        {
            WaitMs(300);
            PressExitButton();
        }
        else if (_imageFinder.TryFindOnScreen(GameButton.Continue, out var location))
        {
            // click continue in case a new bitizen moved in
            _windowsApiService.SendClick(location.X, location.Y);
        }
        else
        {
            while (!_imageFinder.IsImageOnScreen(GameButton.MenuButton))
            {
                WaitMs(500);
            }

            MoveUp();
            _userConfiguration.AddElevatorRide();
            _userConfiguration.SaveConfiguration();
        }
    }

    public void PressQuestButton(int location)
    {
        _logger.Log("Clicking on the quest button");
        _windowsApiService.SendClick(location);
        WaitMs(500);

        if (_imageFinder.IsImageOnScreen(GameWindow.DeliverBitizensQuestPrompt))
        {
            DeliverBitizens();
        }
        else if (_imageFinder.IsImageOnScreen(GameWindow.FindBitizensQuestPrompt))
        {
            FindBitizens();
        }
        else
        {
            ClickAndWaitMs(90, 440, 500); // skip the quest
            ClickAndWaitMs(230, 380, 0);  // confirm
        }
    }

    public void FindBitizens()
    {
        _logger.Log("Skipping the quest");
        ClickAndWaitMs(95, 445, 500); // skip the quest
        ClickAndWaitMs(225, 380, 0);  // confirm skip
    }

    public void DeliverBitizens()
    {
        _logger.Log("Delivering bitizens");
        _windowsApiService.SendClick(230, 440); // continue
    }

    public void OpenTheGame(int location)
    {
        WaitSec(1);
        _windowsApiService.SendClick(location);
        WaitSec(7);
    }

    public void CheckIfAdsRewardWillBeLost()
    {
        WaitMs(500);
        if (_imageFinder.IsImageOnScreen(GameWindow.AdsLostRewardNotification))
        {
            _windowsApiService.SendClick(240, 344); // click "keep watching"
            WaitSec(15);
        }
        else
        {
            _windowsApiService.SendClick(240, 344);
        }
    }

    public void CloseNewFloorMenu()
    {
        _logger.Log("Exiting from new floor menu");
        PressExitButton();
    }

    public void CloseBuildNewFloorPrompt()
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

    public void ClickAndWaitMs(int location, int waitTimeMs)
    {
        _windowsApiService.SendClick(location);
        WaitMs(waitTimeMs);
    }

    public void CheckForNewFloor(int currentFloor, int balance, CancellationToken cancellationToken)
    {
        while (true)
        {
            cancellationToken.ThrowIfCancellationRequested();
            CheckObstructingWindows();

            if (currentFloor >= _userConfiguration.RebuildAtFloor)
            {
                RebuildTower();
                return;
            }

            if (balance < FloorPrices[currentFloor + 1])
            {
                return;
            }

            if (_imageFinder.IsImageOnScreen(GameButton.RideElevator))
            {
                WaitMs(300); // important, wait until elevator button is in correct position
                RideElevator();
                return;
            }

            TryBuildNewFloor();

            balance -= FloorPrices[currentFloor + 1];
            currentFloor = _userConfiguration.CurrentFloor;
        }
    }

    private void TryBuildNewFloor()
    {
        if (AttemptNextFloorBuildAt <= DateTime.Now)
        {
            _logger.Log("Building new floor");

            MoveUp();
            ClickAndWaitMs(300, 360, 300); // click on a new floor

            if (!_imageFinder.IsImageOnScreen(GameWindow.BuildNewFloorNotification))
            {
                CheckObstructingWindows();
                return;
            }

            _windowsApiService.SendClick(230, 320); // confirm build
            WaitMs(300); // important, wait until all possible windows are showed

            if (!_imageFinder.IsImageOnScreen(GameWindow.NewFloorNoCoinsNotification))
            {
                _userConfiguration.AddOneFloor();
                _logger.Log("Built a new floor");
            }
            else
            {
                _windowsApiService.SendClick(230, 380); // continue
                AttemptNextFloorBuildAt = DateTime.Now.AddSeconds(5);
                _logger.Log("Not enough coins for a new floor");
            }

            MoveUp();
        }
        else
        {
            _logger.Log("Too early to build a floor");
            WaitSec(1);
        }
    }

    private void CheckObstructingWindows()
    {
        using var gameScreen = _windowsApiService.GetGameScreenshot();

        if (_imageFinder.TryFindOnScreen(GameButton.Continue, out var location, gameScreen))
        {
            _windowsApiService.SendClick(location.X, location.Y);
            return;
        }

        if (_imageFinder.TryFindOnScreen(GameButton.Collect, out location, gameScreen))
        {
            _windowsApiService.SendClick(location.X, location.Y);
            return;
        }

        if (_imageFinder.TryFindOnScreen(GameButton.Awesome, out location, gameScreen))
        {
            _windowsApiService.SendClick(location.X, location.Y);
            return;
        }

        if (_imageFinder.IsImageOnScreen(GameButton.Back, gameScreen))
        {
            PressExitButton();
            return;
        }

        if (_imageFinder.IsImageOnScreen(GameWindow.BitizenMovedIn, gameScreen))
        {
            PressExitButton();
            return;
        }

        if (_imageFinder.IsImageOnScreen(GameWindow.Lobby, gameScreen))
        {
            _logger.Log("Found lobby window");
            PressExitButton();
            MoveUp();
        }
    }

    private void RebuildTower()
    {
        _logger.Log("Rebuilding the tower");
        _userConfiguration.SaveRebuildTime();
        _userConfiguration.ResetElevatorRides();

        ClickAndWaitMs(305, 570, 350); // menu
        ClickAndWaitMs(165, 435, 350); // rebuild menu
        ClickAndWaitMs(165, 440, 350); // rebuild button
        ClickAndWaitMs(230, 380, 350); // confirm rebuild
        ClickAndWaitMs(230, 380, 600); // yes, skip tutorial (finally)
        //ClickAndWaitMs(165, 405, 200); // click to claim easter GT bonus

        _userConfiguration.SetCurrentFloor(4);
    }

    // ReSharper disable once UnusedMember.Global
    public void PassTheTutorial()
    {
        _logger.Log("Passing the tutorial");
        if (_imageFinder.IsImageOnScreen(GameButton.Continue))
        {
            ClickAndWaitMs(170, 435, 400); // Continue
        }

        MoveDown();
        WaitMs(1000);

        ClickAndWaitMs(195, 260, 200);  // build a new floor
        ClickAndWaitMs(230, 380, 200);  // confirm
        ClickAndWaitMs(20, 60, 200);    // complete quest
        ClickAndWaitMs(170, 435, 200);  // collect bux
        ClickAndWaitMs(170, 435, 200);  // continue
        ClickAndWaitMs(190, 300, 200);  // click on a new floor
        ClickAndWaitMs(240, 150, 200);  // build a residential floor
        ClickAndWaitMs(160, 375, 200);  // continue
        ClickAndWaitMs(20, 60, 200);    // complete quest
        ClickAndWaitMs(170, 435, 200);  // collect bux
        ClickAndWaitMs(170, 435, 1400); // continue
        ClickAndWaitSec(21, 510, 4);    // click on the elevator button

        // daily rent check (in case it's past midnight)
        if (_imageFinder.TryFindOnScreen(GameButton.Collect, out var location))
        {
            ClickAndWaitMs(location.X, location.Y, 500); // collect daily rent
            ClickAndWaitMs(21, 510, 4000);               // click on elevator button again
        }

        ClickAndWaitMs(230, 380, 200); // continue
        ClickAndWaitMs(20, 60, 200);   // complete quest
        ClickAndWaitMs(170, 435, 200); // collect bux
        ClickAndWaitMs(170, 435, 200); // continue
        ClickAndWaitMs(190, 200, 200); // build a new floor
        ClickAndWaitMs(225, 380, 200); // confirm
        ClickAndWaitMs(200, 200, 200); // open the new floor
        ClickAndWaitMs(90, 340, 200);  // build random food floor
        ClickAndWaitMs(170, 375, 200); // continue
        ClickAndWaitMs(20, 60, 200);   // complete the quest
        ClickAndWaitMs(170, 435, 200); // collect bux
        ClickAndWaitMs(170, 435, 200); // continue
        ClickAndWaitMs(200, 200, 200); // open the food floor
        ClickAndWaitMs(75, 210, 200);  // open the hire menu
        ClickAndWaitMs(80, 100, 200);  // select bitizen
        ClickAndWaitMs(230, 380, 200); // hire him
        ClickAndWaitMs(160, 380, 200); // continue on dream job assignment
        ClickAndWaitMs(300, 560, 200); // exit the food store
        ClickAndWaitMs(20, 60, 200);   // complete the quest
        ClickAndWaitMs(170, 435, 200); // collect bux
        ClickAndWaitMs(170, 435, 200); // continue
        ClickAndWaitMs(200, 200, 200); // open the food store again
        ClickAndWaitSec(200, 210, 5);  // request restock of the first item in the store

        // wait until the floor is restocked
        while (!_imageFinder.IsImageOnScreen(GameButton.Restock))
        {
            WaitMs(700);
        }

        ClickAndWaitMs(284, 188, 200); // press restock button (easter fix)
        ClickAndWaitMs(20, 60, 200);   // complete the quest
        ClickAndWaitMs(170, 435, 200); // collect bux
        ClickAndWaitMs(170, 435, 200); // continue
        ClickAndWaitMs(200, 200, 200); // open food store again
        ClickAndWaitMs(170, 130, 200); // click upgrade
        ClickAndWaitMs(230, 375, 200); // confirm upgrade
        ClickAndWaitMs(165, 375, 200); // continue
        ClickAndWaitMs(300, 560, 200); // exit the food store
        ClickAndWaitMs(20, 60, 200);   // complete the quest
        ClickAndWaitMs(170, 435, 200); // collect bux
        ClickAndWaitMs(170, 435, 0);   // collect more bux

        MoveUp();
    }

    public void TryWatchAds()
    {
        if (_userConfiguration.CurrentFloor >= _userConfiguration.WatchAdsFromFloor)
        {
            _logger.Log("Watching the advertisement");
            ClickAndWaitSec(225, 375, 20);
        }
        else
        {
            _windowsApiService.SendClick(105, 380); // decline the video offer
        }
    }

    private void MoveUp()
    {
        _windowsApiService.SendClick(22, 10);
        WaitMs(500);
    }

    private void MoveDown()
    {
        _windowsApiService.SendClick(230, 537);
    }

    public void PressExitButton()
    {
        _logger.Log("Pressing the exit button");
        _windowsApiService.SendClick(305, 565);
    }

    public void TryPlayRaffle()
    {
        var lastRaffleTime = _userConfiguration.LastRaffleTime;
        var dateTimeNow = DateTime.Now;
        if (lastRaffleTime > dateTimeNow.AddHours(-1))
        {
            return;
        }

        if (_imageFinder.IsImageOnScreen(GameButton.Continue))
        {
            _windowsApiService.SendClick(160, 380); // continue
        }

        _logger.Log("Playing the raffle");
        WaitMs(500);
        ClickAndWaitMs(300, 570, 500);          // open menu
        ClickAndWaitMs(275, 440, 3000);         // open raffle
        _windowsApiService.SendClick(160, 345); // enter raffle

        _userConfiguration.SaveLastRaffleTime(dateTimeNow);
        _userConfiguration.SaveConfiguration();
    }

    private static void WaitSec(int seconds)
    {
        var ms = seconds * 1000;
        Task.Delay(ms).Wait();
    }

    private static void WaitMs(int milliseconds)
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

    private Dictionary<int, int> CalculateFloorPrices()
    {
        var result = new Dictionary<int, int>();
        var floorCostDecrease = _userConfiguration.FloorCostDecrease;

        for (var i = 1; i <= 9; i++)
        {
            var floorPrice = 5000 - (5000 * floorCostDecrease / 100);
            result.Add(i, floorPrice);
        }

        for (var i = 10; i <= _userConfiguration.RebuildAtFloor + 1; i++)
        {
            var floorCost = 1000 * 1 * ((0.5f * (i * i)) + (8 * i) - 117);

            if (i % 2 != 0)
            {
                floorCost += 500;
            }

            var floorPrice = floorCost - (floorCost * floorCostDecrease / 100);
            result.Add(i, (int)floorPrice);
        }

        return result;
    }
}
