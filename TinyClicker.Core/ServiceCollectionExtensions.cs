using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel;
using Tesseract;
using TinyClicker.Core.Logic;
using TinyClicker.Core.Services;

namespace TinyClicker.Core;

public static class ServiceCollectionExtensions
{
    public static void AddCoreServices(this IServiceCollection collection)
    {
        collection.AddSingleton<BackgroundWorker>();
        collection.AddSingleton<IConfigService, ConfigService>();
        collection.AddSingleton<TinyClickerApp>();
        collection.AddSingleton<ScreenScanner>();
        collection.AddSingleton<ClickerActionsRepository>();
        collection.AddSingleton<IWindowsApiService, WindowsApiService>();
        collection.AddSingleton<IImageService, ImageService>();

        collection.AddSingleton(x => new TesseractEngine(@"./Tessdata", "pixel", EngineMode.LstmOnly));
    }
}
