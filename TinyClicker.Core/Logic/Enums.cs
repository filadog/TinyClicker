using System.ComponentModel;

namespace TinyClicker.Core.Logic;

public enum GameButton
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
    ParachuteGift,

    [Description("elevatorButton")]
    RideElevator,

    [Description("freeBuxButton")]
    FreeBuxGift,

    [Description("freeBuxCollectButton")]
    CollectFreeBux,

    [Description("questButton")]
    NewQuest,

    [Description("restockButton")]
    Restock,

    [Description("balanceCoin")]
    BalanceCoin,

    [Description("gameIcon")]
    GameIcon,

    [Description("menuButton")]
    MenuButton,

    [Description("newTasksButton")]
    TasksButton,

    [Description("newGiftsButton")]
    Gift,

    [Description("newTowerManagementButton")]
    TowerManagementButton,

    [Description("newCalendarButton")]
    CalendarButton,

    [Description("awesomeButton")]
    Awesome,

    [Description("completedQuestButton")]
    CompletedQuest
}

public enum GameWindow
{
    [Description("buildNewFloorNotification")]
    BuildNewFloorNotification,

    [Description("newFloorNoCoinsNotification")]
    NewFloorNoCoinsNotification,

    [Description("watchAdPromptCoins")]
    WatchCoinsAdsPrompt,

    [Description("watchAdPromptBux")]
    WatchBuxAdsPrompt,

    [Description("deliverBitizens")]
    DeliverBitizensQuestPrompt,

    [Description("findBitizens")]
    FindBitizensQuestPrompt,

    [Description("adsLostReward")]
    AdsLostRewardNotification,

    [Description("lobby")]
    Lobby,

    [Description("hurryConstructionPrompt")]
    HurryConstructionWithBux,

    [Description("roofCustomizationWindow")]
    RoofCustomization,

    [Description("foundCoinsChuteNotification")]
    FoundCoinsOnParachuteClick,

    [Description("fullyStockedBonus")]
    FullyStockedBonus,

    [Description("bitizenMovedIn")]
    BitizenMovedIn,

    [Description("newFloorMenu")]
    NewFloorMenu
}