using System.ComponentModel;

namespace TinyClicker.Core.Logic;

public enum Button
{
    [Description("continueButton")]
    Continue,

    [Description("backButton")]
    BackButton,

    [Description("newScienceButton")]
    NewScience,

    [Description("fullyStockedBonus")]
    FullyStockedBonus,

    [Description("giftChute")]
    GiftChute,

    [Description("elevatorButton")]
    ElevatorButton,

    [Description("freeBuxButton")]
    FreeBuxButton,

    [Description("freeBuxCollectButton")]
    FreeBuxCollectButton,

    [Description("questButton")]
    QuestButton,

    [Description("restockButton")]
    RestockButton,

    [Description("balanceCoin")]
    BalanceCoin,

    [Description("gameIcon")]
    GameIcon
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
    AdsLostReward,

    [Description("balanceCoin")]
    BalanceCoin,

    [Description("lobby")]
    Lobby
}
