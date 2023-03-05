using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel;
using TinyClicker.Core.Helpers;
using TinyClicker.Core.Logic;

namespace TinyClicker.Core;

public static class ServiceCollectionExtensions
{
    public static void AddCoreServices(this IServiceCollection collection)
    {
        collection.AddSingleton<BackgroundWorker>();
        collection.AddSingleton<ConfigManager>();
        collection.AddSingleton<TinyClickerApp>();
        collection.AddSingleton<ScreenScanner>();
        collection.AddSingleton<ClickerActionsRepository>();
        collection.AddSingleton<InputSimulator>();
        collection.AddSingleton<WindowToImage>();
        collection.AddSingleton<ImageEditor>();
        collection.AddSingleton<ImageToText>();
    }
}
