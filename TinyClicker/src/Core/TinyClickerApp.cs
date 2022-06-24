using System.ComponentModel;
using System.Threading.Tasks;

namespace TinyClicker;

public class TinyClickerApp
{
    readonly ScreenScanner _screenScanner;
    readonly ClickerActionsRepo _clickerActionsRepo;

    public TinyClickerApp(bool isBluestacks)
    {
        _screenScanner = new ScreenScanner(isBluestacks);
        _clickerActionsRepo = _screenScanner._clickerActionsRepo;
    }

    public void StartInBackground(BackgroundWorker worker)
    {
        worker.WorkerSupportsCancellation = true;
        worker.DoWork += (s, e) =>
        {
            RunLoop(worker);
        };
        worker.RunWorkerAsync();
    }

    public void RunLoop(BackgroundWorker worker)
    {
        int processId = _clickerActionsRepo._processId;
        while (processId != -1 && !worker.CancellationPending)
        {
            _screenScanner.StartIteration();
            Task.Delay(1500).Wait();
        }
    }
}
