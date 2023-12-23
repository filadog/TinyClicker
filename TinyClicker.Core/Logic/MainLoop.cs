using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
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

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            await Task.Run(() => ScanScreenAndPerformActions(cancellationToken), cancellationToken);
            await Task.Delay(_userConfiguration.GameScreenScanningRateMs, cancellationToken);
        }
    }

    private void ScanScreenAndPerformActions(CancellationToken cancellationToken)
    {
        using var gameWindow = _windowsApiService.GetGameScreenshot();

        if (_imageFinder.TryFindFirstImageOnScreen(gameWindow, out var result))
        {
            var message = "Found " + result.ItemName;
            _logger.Log(message);
            _notFoundCount = 0;
        }
        else
        {
            OnNotFoundImages();
        }

        var currentFloor = _userConfiguration.CurrentFloor;
        if (currentFloor != _userConfiguration.RebuildAtFloor)
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

        if (!_userConfiguration.BuildFloors)
        {
            return;
        }

        var balance = _balanceParser.GetBalanceFromWindow(gameWindow);
        if (balance == -1 || currentFloor < 4)
        {
            return;
        }

        _clickerActionsRepository.CheckForNewFloor(currentFloor, balance, cancellationToken);
    }

    private void OnNotFoundImages()
    {
        _notFoundCount++;
        _logger.Log("Found nothing x" + _notFoundCount);

        var scanningRate = _userConfiguration.GameScreenScanningRateMs;
        var multiplier = scanningRate switch
        {
            > 500 => scanningRate / 500f,
            < 500 => 500f / scanningRate,
            _ => 1f
        };

        var maxAttempts = 75 * multiplier;

        if (_notFoundCount >= maxAttempts)
        {
            _clickerActionsRepository.TryCloseAd();
        }
    }

    private void PerformAction((string Key, int Location) item)
    {
        if (_clickerActionsMap.TryGetValue(item.Key, out var action))
        {
            action(item.Location);
        }
    }
}
