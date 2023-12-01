using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using TinyClicker.Core;
using TinyClicker.Core.Extensions;
using TinyClicker.Core.Logic;
using TinyClicker.Core.Services;
using Xunit;

namespace TinyClicker.Tests.ImageProcessing;

public class MainLoopTests : IClassFixture<DependencySetupFixture>
{
    private readonly ServiceProvider _serviceProvider;
    public MainLoopTests(DependencySetupFixture fixture)
    {
        _serviceProvider = fixture.ServiceProvider;
    }

    [Fact]
    public void FindElevatorButtonOnScreen()
    {
        var itemName = Button.ElevatorButton.GetDescription();
        var foundName = TryFindFirstItemOnScreen(itemName);

        Assert.Equal(itemName, foundName);
    }

    [Fact]
    public void FindFreeBuxCollectButtonOnScreen()
    {
        var itemName = Button.FreeBuxCollectButton.GetDescription();
        var foundName = TryFindFirstItemOnScreen(itemName);

        Assert.Equal(itemName, foundName);
    }

    [Fact]
    public void FindQuestButtonOnScreen()
    {
        var itemName = Button.QuestButton.GetDescription();
        var foundName = TryFindFirstItemOnScreen(itemName);

        Assert.Equal(itemName, foundName);
    }

    [Fact]
    public void FindBackButtonOnScreen()
    {
        var itemName = Button.BackButton.GetDescription();
        var foundName = TryFindFirstItemOnScreen(itemName);

        Assert.Equal(itemName, foundName);
    }

    [Fact]
    public void FindGiftChuteOnScreen()
    {
        var itemName = Button.GiftChute.GetDescription();
        var foundName = TryFindFirstItemOnScreen(itemName);

        Assert.Equal(itemName, foundName);
    }

    [Fact]
    public void FindNewFloorNoCoinsNotificationOnScreen()
    {
        var item = GameWindow.NewFloorNoCoinsNotification;
        var isItemFound = IsItemOnScreen(item);

        Assert.True(isItemFound);
    }

    [Fact]
    public void FindNothingOnScreen()
    {
        var openCvService = _serviceProvider.GetService<IOpenCvService>();

        var screenshot = TestHelper.LoadGameScreenshot("NothingOnScreen");
        var foundItems = openCvService.TryFindFirstImageOnScreen(screenshot, out _);

        Assert.False(foundItems);
    }

    private string TryFindFirstItemOnScreen(string itemName)
    {
        var openCvService = _serviceProvider.GetService<IOpenCvService>();
        var screenshot = TestHelper.LoadGameScreenshot(itemName);
        if (openCvService.TryFindFirstImageOnScreen(screenshot, out var item))
        {
            return item.ItemName;
        }
        else
        {
            throw new InvalidOperationException("Nothing found");
        }
    }

    private bool IsItemOnScreen(Enum item)
    {
        var itemName = item.GetDescription();

        var openCvService = _serviceProvider.GetService<IOpenCvService>() ?? throw new NullReferenceException();
        var screenshot = TestHelper.LoadGameScreenshot(itemName);
        var templates = openCvService.MakeTemplatesFromSamples(screenshot);
        var isImageFound = openCvService.IsImageOnScreen(item, templates, screenshot);

        return isImageFound;
    }
}
