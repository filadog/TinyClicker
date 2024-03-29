﻿using Microsoft.Extensions.DependencyInjection;
using TinyClicker.Core;
using TinyClicker.UI;

namespace TinyClicker.Tests;

// ReSharper disable once ClassNeverInstantiated.Global
public class DependencySetupFixture
{
    public DependencySetupFixture()
    {
        var collection = new ServiceCollection();

        collection.AddCoreServices();
        collection.AddUiServices();

        ServiceProvider = collection.BuildServiceProvider();
    }

    public ServiceProvider ServiceProvider { get; }
}
