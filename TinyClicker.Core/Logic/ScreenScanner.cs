using System;
using System.Collections.Generic;
using System.Linq;
using TinyClicker.Core.Extensions;
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

    private Dictionary<string, Action<int>> _clickerActionsMap;
    private Dictionary<string, int> _lastFoundImages = new();
    private int _foundCount;

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
        _clickerActionsMap = clickerActionsRepository.GetActionsMap();
    }

    private int FloorToRebuildAt => _configService.Config.RebuildAtFloor;

    public void StartIteration()
    {
        using var gameWindow = _windowsApiService.GetGameScreenshot();
        var currentFloor = _configService.Config.CurrentFloor;

        _lastFoundImages = _openCvService.TryFindFirstOnScreen(gameWindow);

        foreach (var image in _lastFoundImages)
        {
            if (_lastFoundImages.Any())
            {
                var msg = "Found " + image.Key;
                _logger.Log(msg);
            }
            else
            {
                _foundCount = 0;
            }
        }

        if (_lastFoundImages.Count == 0)
        {
            _foundCount++;
            _logger.Log("Found nothing x" + _foundCount.ToString());

            if (_foundCount >= 100) // todo multiply count by loop speed here
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
            if (_lastFoundImages.Any())
            {
                var image = _lastFoundImages.First();
                PerformAction((image.Key, image.Value));
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

            _clickerActionsRepository.CheckForNewFloor(currentFloor, balance);
        }

        _lastFoundImages.Clear();
    }

    private void PerformAction((string Key, int Location) item)
    {
        var found = _clickerActionsMap.TryGetValue(item.Key, out var action);
        if (found && action != null)
        {
            action(item.Location);
        }
    }
}
