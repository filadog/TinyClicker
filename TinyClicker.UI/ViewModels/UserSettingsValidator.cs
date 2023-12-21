using FluentValidation;

namespace TinyClicker.UI.ViewModels;

public class UserSettingsValidator : AbstractValidator<UserSettingsViewModel>
{
    public UserSettingsValidator()
    {
        RuleFor(x => x.RebuildAtFloor)
            .InclusiveBetween(5, int.MaxValue)
            .WithName("Floor to rebuild at");

        RuleFor(x => x.WatchAdsFromFloor)
            .InclusiveBetween(1, int.MaxValue)
            .WithName("Floor to watch ads from");

        RuleFor(x => x.FloorCostDecreasePercent)
            .InclusiveBetween(0, 10)
            .WithName("Percent by which floor costs are decreased");
        
        RuleFor(x => x.GameScreenScanningRateMs)
            .InclusiveBetween(10, 600000)
            .WithName(
                "Amount of time between scans of the game screen in milliseconds. " +
                "Recommended value is 500");
    }
}
