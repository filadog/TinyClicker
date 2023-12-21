using System;
using System.ComponentModel;
using System.Threading.Tasks;
using TinyClicker.Core.Logging;
using TinyClicker.Core.Services;

namespace TinyClicker.Core.Logic;

public class TinyClickerApp
{
    private readonly BackgroundWorker _backgroundWorker;
    private readonly MainLoop _mainLoop;
    private readonly IUserConfiguration _userConfiguration;
    private readonly ILogger _logger;

    public TinyClickerApp(
        BackgroundWorker backgroundWorker,
        MainLoop mainLoop,
        IUserConfiguration userConfiguration,
        ILogger logger)
    {
        _backgroundWorker = backgroundWorker;
        _mainLoop = mainLoop;
        _logger = logger;
        _userConfiguration = userConfiguration;
    }

    public void StartInBackground()
    {
        _backgroundWorker.WorkerSupportsCancellation = true;
        _backgroundWorker.DoWork += (_, _) =>
        {
            RunLoop(_backgroundWorker);
        };

        _backgroundWorker.RunWorkerAsync();
    }

    private void RunLoop(BackgroundWorker worker)
    {
        while (!worker.CancellationPending)
        {
            try
            {
                _mainLoop.Start();
                Task.Delay(_userConfiguration.Configuration.GameScreenScanningRateMs).Wait();
            }
            catch (InvalidOperationException ex)
            {
                _logger.Log(ex.Message);
            }
        }
    }
}
