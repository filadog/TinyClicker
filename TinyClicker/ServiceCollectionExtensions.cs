using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel;

namespace TinyClicker;

public static class ServiceCollectionExtensions
{
    public static void Configure(this IServiceCollection collection)
    {
        collection.AddSingleton<MainWindow>();
        collection.AddSingleton<SettingsWindow>();

        collection.AddSingleton<BackgroundWorker>();
        collection.AddSingleton<ConfigManager>();
        collection.AddSingleton<TinyClickerApp>();
        collection.AddSingleton<ScreenScanner>();
        collection.AddSingleton<ClickerActionsRepo>();
        collection.AddSingleton<Logger>();
        collection.AddSingleton<InputSimulator>();
        collection.AddSingleton<WindowToImage>();
    }
}
