using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using TinyClicker.Core;
using TinyClicker.Core.Logging;

namespace TinyClicker.UI;

public partial class App : Application
{
    private readonly ServiceProvider _serviceProvider;

	public App()
	{
        var services = new ServiceCollection();

        services.AddCoreServices();
        services.AddUiServices();

        _serviceProvider = services.BuildServiceProvider();
    }

    private void OnStartup(object sender, StartupEventArgs e)
    {
        var mainWindow = _serviceProvider.GetRequiredService<IMainWindow>();
        mainWindow.Show();
    }
}
