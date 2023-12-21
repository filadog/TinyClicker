using System;
using System.Collections.Generic;
using TinyClicker.Core.Logging;
using TinyClicker.Core.Services;

namespace TinyClicker.Core.Logic;

public class MainLoop
{
    private readonly ClickerActionsRepository _clickerActionsRepository;
    private readonly IWindowsApiService _windowsApiService;
    private readonly IUserConfiguration _userConfiguration;
    private readonly IBalanceParser _balanceParser;
    private readonly IImageFinder _imageFinder;
    private readonly ILogger _logger;

    private readonly Dictionary<string, Action<int>> _clickerActionsMap;
    private int _notFoundCount;

    public MainLoop(
        ClickerActionsRepository clickerActionsRepository,
        IUserConfiguration userConfiguration,
        IWindowsApiService windowsApiService,
        IBalanceParser balanceParser,
        IImageFinder imageFinder,
        ILogger logger)
    {
        _clickerActionsRepository = clickerActionsRepository;
        _userConfiguration = userConfiguration;
        _windowsApiService = windowsApiService;
        _balanceParser = balanceParser;
        _imageFinder = imageFinder;
        _logger = logger;
        _clickerActionsMap = clickerActionsRepository.GetActionsMap();
    }

    private int FloorToRebuildAt => _userConfiguration.Configuration.RebuildAtFloor;

    public void Start()
    {
        using var gameWindow = _windowsApiService.GetGameScreenshot();
        var currentFloor = _userConfiguration.Configuration.CurrentFloor;

        if (_imageFinder.TryFindFirstImageOnScreen(gameWindow, out var result))
        {
            var message = "Found " + result.ItemName;
            _logger.Log(message);
            _notFoundCount = 0;
        }
        else
        {
            _notFoundCount++;
            _logger.Log("Found nothing x" + _notFoundCount);

            if (_notFoundCount >= 35000 / _userConfiguration.Configuration.GameScreenScanningRateMs)
            {
                _clickerActionsRepository.TryCloseAd();
                return;
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

        if (!_userConfiguration.Configuration.BuildFloors)
        {
            return;
        }

        var balance = _balanceParser.GetBalanceFromWindow(gameWindow);
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
