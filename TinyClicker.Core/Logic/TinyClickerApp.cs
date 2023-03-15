using System;
using System.ComponentModel;
using System.Threading.Tasks;
using TinyClicker.Core.Logging;

namespace TinyClicker.Core.Logic;

public class TinyClickerApp
{
    private readonly ScreenScanner _screenScanner;
    private readonly BackgroundWorker _backgroundWorker;
    private readonly ILogger _logger;
    public TinyClickerApp(BackgroundWorker backgroundWorker, ScreenScanner screenScanner, ILogger logger)
    {
        _backgroundWorker = backgroundWorker;
        _screenScanner = screenScanner;
        _logger = logger;
    }

    public void StartInBackground()
    {
        _backgroundWorker.WorkerSupportsCancellation = true;
        _backgroundWorker.DoWork += (s, e) =>
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
                _screenScanner.StartIteration();
                Task.Delay(1500).Wait();
            }
            catch (InvalidOperationException ex)
            {
                _logger.Log(ex.Message);
            }
        }
    }
}
