using System.ComponentModel;
using System.Threading.Tasks;

namespace TinyClicker;

public class TinyClickerApp
{
    readonly ScreenScanner screenScanner;

    public TinyClickerApp(bool isBluestacks)
    {
        screenScanner = new ScreenScanner(isBluestacks);
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
        while (!worker.CancellationPending)
        {
            screenScanner.StartIteration();
            Task.Delay(1500).Wait();
        }
    }
}
