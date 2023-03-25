using System.Collections.Generic;
using System.Linq;
using TinyClicker.Core.Logging;
using TinyClicker.Core.Services;

namespace TinyClicker.Core.Logic;

public class ScreenScanner
{
    private readonly IConfigService _configService;
    private readonly ClickerActionsRepository _clickerActionsRepository;
    private readonly IOpenCvService _openCvService;
    private readonly IWindowsApiService _windowsApiService;
    private readonly ILogger _logger;
    private readonly IImageService _imageService;

    public ScreenScanner(
        IConfigService configService,
        ClickerActionsRepository clickerActionsRepository,
        IWindowsApiService windowsApiService,
        IOpenCvService openCvService,
        IImageService imageService,
        ILogger logger)
    {
        _configService = configService;
        _clickerActionsRepository = clickerActionsRepository;
        _windowsApiService = windowsApiService;
        _openCvService = openCvService;
        _imageService = imageService;
        _logger = logger;
    }

    private Dictionary<string, int> FoundImages { get; set; } = new();
    private int FloorToRebuildAt => _configService.Config.RebuildAtFloor;
    private int FoundCount { get; set; }

    public void StartIteration()
    {
        using var gameWindow = _windowsApiService.GetGameScreenshot();
        var currentFloor = _configService.Config.CurrentFloor;

        FoundImages = _openCvService.TryFindFirstOnScreen(gameWindow);

        foreach (var image in FoundImages)
        {
            var msg = "Found " + image.Key;
            _logger.Log(msg);
            FoundCount = 0;
        }

        if (FoundImages.Count == 0)
        {
            FoundCount++;
            var msg = "Found nothing x" + FoundCount;
            _logger.Log(msg);

            if (FoundCount >= 35) // todo multiply count by loop speed here
            {
                _clickerActionsRepository.CloseAd();
            }
        }

        if (currentFloor == 1)
        {
            _clickerActionsRepository.PassTheTutorial();
            _configService.SetCurrentFloor(4);
            return;
        }

        if (currentFloor != FloorToRebuildAt)
        {
            if (FoundImages.Any())
            {
                var image = FoundImages.First();
                PerformActions((image.Key, image.Value));
            }
            else
            {
                _clickerActionsRepository.PlayRaffle();
            }
        }

        if (_configService.Config.BuildFloors)
        {
            var balance = _imageService.GetBalanceFromWindow(gameWindow);
            if (balance == -1 || currentFloor < 4)
            {
                return;
            }

            if (currentFloor > _configService.Config.RebuildAtFloor)
            {
                return;
            }

            _clickerActionsRepository.CheckForNewFloor(currentFloor, balance);
        }

        FoundImages.Clear();
    }

    private void PerformActions((string Key, int Location) item)
    {
        switch (item.Key)
        {
            case "closeAd":
            case "closeAd_2":
            case "closeAd_3":
            case "closeAd_4":
            case "closeAd_5":
            case "closeAd_6":
            case "closeAd_7":
            case "closeAd_8":
            case "closeAd_9": _clickerActionsRepository.CloseAd(); break;
            case "freeBuxCollectButton": _clickerActionsRepository.CollectFreeBux(item.Location); break;
            case "roofCustomizationWindow": _clickerActionsRepository.ExitRoofCustomizationMenu(); break;
            case "hurryConstructionPrompt": _clickerActionsRepository.CancelHurryConstruction(); break;
            case "continueButton": _clickerActionsRepository.PressContinue(item.Location); break;
            case "foundCoinsChuteNotification": _clickerActionsRepository.CloseChuteNotification(); break;
            case "restockButton": _clickerActionsRepository.Restock(); break;
            case "freeBuxButton": _clickerActionsRepository.PressFreeBuxButton(); break;
            case "giftChute": _clickerActionsRepository.ClickOnChute(item.Location); break;
            case "backButton": _clickerActionsRepository.PressExitButton(); break;
            case "elevatorButton": _clickerActionsRepository.RideElevator(); break;
            case "questButton": _clickerActionsRepository.PressQuestButton(item.Location); break;
            case "completedQuestButton": _clickerActionsRepository.CompleteQuest(item.Location); break;
            case "watchAdPromptCoins":
            case "watchAdPromptBux": _clickerActionsRepository.TryWatchAds(); break;
            case "findBitizens": _clickerActionsRepository.FindBitizens(); break;
            case "deliverBitizens": _clickerActionsRepository.DeliverBitizens(); break;
            case "newFloorMenu": _clickerActionsRepository.CloseNewFloorMenu(); break;
            case "buildNewFloorNotification": _clickerActionsRepository.CloseBuildNewFloorNotification(); break;
            case "gameIcon": _clickerActionsRepository.OpenTheGame(item.Location); break;
            case "adsLostReward": _clickerActionsRepository.CheckForLostAdsReward(); break;
            case "newScienceButton": _clickerActionsRepository.CollectNewScience(item.Location); break;
            default: break;
        }
    }
}
