namespace TinyClicker.Core.Logging;

public class Logger : ILogger
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
