using Microsoft.Extensions.DependencyInjection;
using Tesseract;
using TinyClicker.Core.Logic;
using TinyClicker.Core.Services;

namespace TinyClicker.Core;

public static class ServiceCollectionExtensions
{
    public static void AddCoreServices(this IServiceCollection collection)
    {
        collection.AddSingleton<MainLoop>();
        collection.AddSingleton<ClickerActionsRepository>();
        collection.AddSingleton<IUserConfiguration, UserConfiguration>();
        collection.AddSingleton<IWindowsApiService, WindowsApiService>();
        collection.AddSingleton<IBalanceParser, BalanceParser>();
        collection.AddSingleton<IImageFinder, ImageFinder>();

        collection.AddSingleton(new TesseractEngine("./Tessdata", "pixel", EngineMode.LstmOnly));
    }
}
