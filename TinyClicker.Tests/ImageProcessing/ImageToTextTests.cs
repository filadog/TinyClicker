using Microsoft.Extensions.DependencyInjection;
using TinyClicker.Core.Helpers;
using Xunit;

namespace TinyClicker.Tests.ImageProcessing;

public class ImageToTextTests : IClassFixture<DependencySetupFixture>
{
    private readonly ServiceProvider _serviceProvider;
    private readonly ImageToText _imageToText;
    public ImageToTextTests(DependencySetupFixture fixture)
    {
        _serviceProvider = fixture.ServiceProvider;
        _imageToText = _serviceProvider.GetRequiredService<ImageToText>();
    }

    [Fact]
    public void ParseBalanceFromImage_17890000()
    {
        var screenshot = TestHelper.LoadGameScreenshot("ElevatorButton");

        int actualBalance = _imageToText.ParseBalance(screenshot);
        int expected = 17890000;

        Assert.Equal(expected, actualBalance);
    }

    [Fact]
    public void ParseBalanceFromImage_6989000()
    {
        var screenshot = TestHelper.LoadGameScreenshot("QuestButton");

        int actualBalance = _imageToText.ParseBalance(screenshot);
        int expected = 6989000;

        Assert.Equal(expected, actualBalance);
    }

    [Fact]
    public void ParseBalanceFromImage_7000000()
    {
        var screenshot = TestHelper.LoadGameScreenshot("BackButton");

        int actualBalance = _imageToText.ParseBalance(screenshot);
        int expected = 7000000;

        Assert.Equal(expected, actualBalance);
    }

    [Fact]
    public void ParseBalanceFromImage_47751()
    {
        var screenshot = TestHelper.LoadGameScreenshot("47751");

        int actualBalance = _imageToText.ParseBalance(screenshot);
        int expected = 47751;

        Assert.Equal(expected, actualBalance);
    }

    [Fact]
    public void ParseBalanceFromImage_441825()
    {
        var screenshot = TestHelper.LoadGameScreenshot("441825");

        int actualBalance = _imageToText.ParseBalance(screenshot);
        int expected = 441825;

        Assert.Equal(expected, actualBalance);
    }
}
