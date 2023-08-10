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
    FreeBuxGiftButton,

    [Description("freeBuxCollectButton")]
    FreeBuxCollectButton,

    [Description("questButton")]
    QuestButton,

    [Description("restockButton")]
    RestockButton,

    [Description("balanceCoin")]
    BalanceCoin,

    [Description("gameIcon")]
    GameIcon,

    [Description("menuButton")]
    MenuButton,

    [Description("new_tasks_button")]
    TasksButton,

    [Description("new_gifts_button")]
    GiftsButton,

    [Description("new_tower_management_button")]
    TowerManagementButton,

    [Description("new_calendar_button")]
    CalendarButton
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

    [Description("lobby")]
    Lobby,

    [Description("hurryConstructionPrompt")]
    HurryConstruction,

    [Description("fullyStockedBonus")]
    FullyStockedBonus,

    [Description("bitizen_moved_in")]
    BitizenMovedIn
}
