using Microsoft.Extensions.DependencyInjection;
using TinyClicker.Core;
using TinyClicker.Core.Helpers;
using TinyClicker.Core.Logic;
using TinyClicker.UI;

namespace TinyClicker.Tests;

public class DependencySetupFixture
{
    public DependencySetupFixture()
    {
        var collection = new ServiceCollection();

        collection.AddCoreServices();
        collection.AddUiServices();

        ServiceProvider = collection.BuildServiceProvider();
    }

    public ServiceProvider ServiceProvider { get; private set; }
}
