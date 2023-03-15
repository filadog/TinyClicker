using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using TinyClicker.Core;
using TinyClicker.Core.Logic;
using TinyClicker.Core.Services;
using Xunit;

namespace TinyClicker.Tests.ImageProcessing;

public class ScreenScannerTests : IClassFixture<DependencySetupFixture>
{
    private readonly ServiceProvider _serviceProvider;
    public ScreenScannerTests(DependencySetupFixture fixture)
    {
        _serviceProvider = fixture.ServiceProvider;
    }

    [Fact]
    public void FindElevatorButtonOnScreen()
    {
        var itemName = Button.ElevatorButton.GetName();
        var foundName = TryFindFirstItemOnScreen(itemName);

        Assert.Equal(itemName, foundName);
    }

    [Fact]
    public void FindFreeBuxButtonOnScreen()
    {
        var itemName = Button.FreeBuxButton.GetName();
        var foundName = TryFindFirstItemOnScreen(itemName);

        Assert.Equal(itemName, foundName);
    }

    [Fact]
    public void FindFreeBuxCollectButtonOnScreen()
    {
        var itemName = Button.FreeBuxCollectButton.GetName();
        var foundName = TryFindFirstItemOnScreen(itemName);

        Assert.Equal(itemName, foundName);
    }

    [Fact]
    public void FindQuestButtonOnScreen()
    {
        var itemName = Button.QuestButton.GetName();
        var foundName = TryFindFirstItemOnScreen(itemName);

        Assert.Equal(itemName, foundName);
    }

    [Fact]
    public void FindBackButtonOnScreen()
    {
        var itemName = Button.BackButton.GetName();
        var foundName = TryFindFirstItemOnScreen(itemName);

        Assert.Equal(itemName, foundName);
    }

    [Fact]
    public void FindGiftChuteOnScreen()
    {
        var itemName = Button.GiftChute.GetName();
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
        var foundItems = openCvService?.TryFindFirstOnScreen(screenshot);

        Assert.True(foundItems?.Count == 0);
    }

    [Fact]
    public void FindBuildNewFloorNotificationOnScreen()
    {
        var itemName = GameWindow.BuildNewFloorNotification.GetName();
        var foundName = TryFindFirstItemOnScreen(itemName);

        Assert.Equal(itemName, foundName);
    }

    private string TryFindFirstItemOnScreen(string itemName, string? screenshotName = null)
    {
        var openCvService = _serviceProvider.GetService<IOpenCvService>() ?? throw new NullReferenceException();
        var screenshot = TestHelper.LoadGameScreenshot(screenshotName ?? itemName);
        var foundItems = openCvService.TryFindFirstOnScreen(screenshot);

        var item = foundItems.FirstOrDefault();

        return item.Key;
    }

    private bool IsItemOnScreen(Enum item)
    {
        var itemName = item.GetName();

        var openCvService = _serviceProvider.GetService<IOpenCvService>() ?? throw new NullReferenceException();
        var screenshot = TestHelper.LoadGameScreenshot(itemName);
        var templates = openCvService.MakeTemplates(screenshot);
        var isImageFound = openCvService.IsImageFound(item, templates, screenshot);

        return isImageFound;
    }
}
