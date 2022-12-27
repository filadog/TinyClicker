using TinyClickerLib.Core;

namespace TinyClicker;

public class Logger
{
    private IMainWindow? _mainWindow;
    public void SetMainWindow(IMainWindow mainWindow)
    {
        _mainWindow = mainWindow;
    }

    public void Log(string message)
    {
        _mainWindow?.Log(message);
    }
}
