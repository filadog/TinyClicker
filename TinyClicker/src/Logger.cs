namespace TinyClicker;

public class Logger
{
    private MainWindow? _mainWindow;
    public void SetMainWindow(MainWindow mainWindow)
    {
        _mainWindow = mainWindow;
    }

    public void Log(string message)
    {
        if (_mainWindow is not null)
        {
            _mainWindow.Log(message);
        }
    }
}
