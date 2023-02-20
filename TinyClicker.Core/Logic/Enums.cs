using System.ComponentModel;

namespace TinyClicker.Core.Logic;

public enum Button
{
    [Description("continueButton")]
    Continue,

    [Description("backButton")]
    Back,

    [Description("newScienceButton")]
    NewScience,

    [Description("fullyStockedBonus")]
    FullyStockedBonus,

    [Description("giftChute")]
    GiftChute
}

public enum GameWindow
{
    [Description("buildNewFloorNotification")]
    BuildNewFloorNotification,

    [Description("newFloorNoCoinsNotification")]
    NewFloorNoCoinsNotification,

    [Description("watchAdPromptCoins")]
    WatchAdPromptCoins,

    [Description("watchAdPromptBux")]
    WatchAdPromptBux,

    [Description("deliverBitizens")]
    DeliverBitizens,

    [Description("findBitizens")]
    FindBitizens,

    [Description("adsLostReward")]
    AdsLostReward
}
