namespace TinyClicker.Core.Logging;

public interface ILogger
{
    void Log(string message);
    void SetMainWindow(IMainWindow mainWindow);
}
