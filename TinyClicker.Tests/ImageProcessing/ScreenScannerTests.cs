using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using TinyClicker.Core;
using TinyClicker.Core.Logic;
using Xunit;

namespace TinyClicker.Tests.ImageProcessing;

public class ScreenScannerTests : IClassFixture<DependencySetupFixture>
{
    private ServiceProvider _serviceProvider;
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
        var clickerActionsRepo = _serviceProvider.GetService<ClickerActionsRepository>();

        var screenshot = TestHelper.LoadGameScreenshot("NothingOnScreen");
        var foundItems = clickerActionsRepo.TryFindFirstOnScreen(screenshot);

        Assert.True(foundItems.Count == 0);
    }

    private string TryFindFirstItemOnScreen(string itemName, string? screenshotName = null)
    {
        var clickerActionsRepo = _serviceProvider.GetService<ClickerActionsRepository>();

        var screenshot = TestHelper.LoadGameScreenshot(screenshotName == null ? itemName : screenshotName);
        var foundItems = clickerActionsRepo.TryFindFirstOnScreen(screenshot);

        var item = foundItems.FirstOrDefault();
        return item.Key;
    }

    private bool IsItemOnScreen(Enum item)
    {
        var itemName = item.GetName();

        var clickerActionsRepo = _serviceProvider.GetService<ClickerActionsRepository>();

        var screenshot = TestHelper.LoadGameScreenshot(itemName);
        var templates = clickerActionsRepo.MakeTemplates(screenshot);
        var isImageFound = clickerActionsRepo.IsImageFound(item, templates, screenshot);

        return isImageFound;
    }
}
