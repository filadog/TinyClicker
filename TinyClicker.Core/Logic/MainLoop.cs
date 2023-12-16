using System;
using System.Collections.Generic;
using TinyClicker.Core.Extensions;
using TinyClicker.Core.Logging;
using TinyClicker.Core.Services;

namespace TinyClicker.Core.Logic;

public class MainLoop
{
    private readonly ClickerActionsRepository _clickerActionsRepository;
    private readonly IConfigService _configService;
    private readonly IOpenCvService _openCvService;
    private readonly IWindowsApiService _windowsApiService;
    private readonly ILogger _logger;
    private readonly IImageToTextService _imageService;

    private readonly Dictionary<string, Action<int>> _clickerActionsMap;
    private int _notFoundCount;

    public MainLoop(
        IConfigService configService,
        ClickerActionsRepository clickerActionsRepository,
        IWindowsApiService windowsApiService,
        IOpenCvService openCvService,
        IImageToTextService imageService,
        ILogger logger)
    {
        _clickerActionsRepository = clickerActionsRepository;
        _configService = configService;
        _windowsApiService = windowsApiService;
        _openCvService = openCvService;
        _imageService = imageService;
        _logger = logger;
        _clickerActionsMap = clickerActionsRepository.GetActionsMap();
    }

    private int FloorToRebuildAt => _configService.Config.RebuildAtFloor;

    public void Start()
    {
        using var gameWindow = _windowsApiService.GetGameScreenshot();
        var currentFloor = _configService.Config.CurrentFloor;

        if (_openCvService.TryFindFirstImageOnScreen(gameWindow, out var result))
        {
            var message = "Found " + result.ItemName;
            _logger.Log(message);
            _notFoundCount = 0;
        }
        else
        {
            _notFoundCount++;
            _logger.Log("Found nothing x" + _notFoundCount.ToString());

            if (_notFoundCount >= 100) // todo multiply count by loop speed here
            {
                _clickerActionsRepository.TryCloseAd();
            }
        }

        if (currentFloor != FloorToRebuildAt)
        {
            if (result != default)
            {
                PerformAction((result.ItemName, result.Location));
            }
            else
            {
                _clickerActionsRepository.TryPlayRaffle();
            }
        }

        if (!_configService.Config.BuildFloors)
        {
            return;
        }

        var balance = _imageService.GetBalanceFromWindow(gameWindow);
        if (balance == -1 || currentFloor < 4)
        {
            return;
        }

        _clickerActionsRepository.CheckForNewFloor(currentFloor, balance);
    }

    private void PerformAction((string Key, int Location) item)
    {
        if (_clickerActionsMap.TryGetValue(item.Key, out var action))
        {
            action(item.Location);
        }
    }
}
