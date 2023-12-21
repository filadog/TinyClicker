using Microsoft.Extensions.DependencyInjection;
using TinyClicker.Core.Services;
using Xunit;

namespace TinyClicker.Tests.ImageProcessing;

public class ImageToTextTests : IClassFixture<DependencySetupFixture>
{
    private readonly IBalanceParser _balanceParser;

    public ImageToTextTests(DependencySetupFixture fixture)
    {
        var serviceProvider = fixture.ServiceProvider;
        _balanceParser = serviceProvider.GetRequiredService<IBalanceParser>();
    }

    [Fact]
    public void ParseBalanceFromImage_17890000()
    {
        var screenshot = TestHelper.LoadGameScreenshot("ElevatorButton");

        var actualBalance = _balanceParser.GetBalanceFromWindow(screenshot);
        const int expected = 17890000;

        Assert.Equal(expected, actualBalance);
    }

    [Fact]
    public void ParseBalanceFromImage_7000000()
    {
        var screenshot = TestHelper.LoadGameScreenshot("BackButton");

        var actualBalance = _balanceParser.GetBalanceFromWindow(screenshot);
        const int expected = 7000000;

        Assert.Equal(expected, actualBalance);
    }

    [Fact]
    public void ParseBalanceFromImage_47751()
    {
        var screenshot = TestHelper.LoadGameScreenshot("47751");

        var actualBalance = _balanceParser.GetBalanceFromWindow(screenshot);
        const int expected = 47751;

        Assert.Equal(expected, actualBalance);
    }

    [Fact]
    public void ParseBalanceFromImage_441825()
    {
        var screenshot = TestHelper.LoadGameScreenshot("441825");

        var actualBalance = _balanceParser.GetBalanceFromWindow(screenshot);
        const int expected = 441825;

        Assert.Equal(expected, actualBalance);
    }

    [Fact]
    public void ParseBalanceFromImage_681()
    {
        var screenshot = TestHelper.LoadGameScreenshot("681");

        var actualBalance = _balanceParser.GetBalanceFromWindow(screenshot);
        const int expected = 681;

        Assert.Equal(expected, actualBalance);
    }

    [Fact]
    public void ParseBalanceFromImage_7455000()
    {
        var screenshot = TestHelper.LoadGameScreenshot("7455000");

        var actualBalance = _balanceParser.GetBalanceFromWindow(screenshot);
        const int expected = 7455000;

        Assert.Equal(expected, actualBalance);
    }

    [Fact]
    public void ParseBalanceFromImage_1868M()
    {
        var screenshot = TestHelper.LoadGameScreenshot("1868M");

        var actualBalance = _balanceParser.GetBalanceFromWindow(screenshot);
        const int expected = 1868000;

        Assert.Equal(expected, actualBalance);
    }
}
