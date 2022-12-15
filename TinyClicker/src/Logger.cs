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
        _mainWindow?.Log(message);
    }
}
