using Microsoft.Extensions.DependencyInjection;
using TinyClicker.Core.Logging;
using TinyClicker.UI.Windows;

namespace TinyClicker.UI;

public static class ServiceCollectionExtensions
{
    public static void AddUiServices(this IServiceCollection collection)
    {
        collection.AddSingleton<ILogger, Logger>();
        collection.AddSingleton<IMainWindow, MainWindow>();

        collection.AddSingleton<SettingsWindow>();
    }
}
