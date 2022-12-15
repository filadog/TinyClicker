using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace TinyClicker;

public partial class App : Application
{
    private readonly ServiceProvider _serviceProvider;

	public App()
	{
        var services = new ServiceCollection();
        services.Configure();
        _serviceProvider = services.BuildServiceProvider();
    }

    private void OnStartup(object sender, StartupEventArgs e)
    {
        var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
        mainWindow.Show();
    }
}
