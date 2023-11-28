using System;
using System.ComponentModel;
using System.Threading.Tasks;
using TinyClicker.Core.Logging;

namespace TinyClicker.Core.Logic;

public class TinyClickerApp
{
    private readonly MainLoop _mainLoop;
    private readonly BackgroundWorker _backgroundWorker;
    private readonly ILogger _logger;

    public TinyClickerApp(
        BackgroundWorker backgroundWorker,
        MainLoop mainLoop,
        ILogger logger)
    {
        _backgroundWorker = backgroundWorker;
        _mainLoop = mainLoop;
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
                // todo add custom loop delay
                _mainLoop.Start();
                Task.Delay(500).Wait();
            }
            catch (InvalidOperationException ex)
            {
                _logger.Log(ex.Message);
            }
        }
    }
}
