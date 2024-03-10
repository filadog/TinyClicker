using System;
using System.Collections.Generic;
using TinyClicker.Core.Logic;

namespace TinyClicker.Core;

public static class ClickerActionsExtensions
{
    public static Dictionary<string, Action<int>> GetActionsMap(this ClickerActionsRepository actions)
    {
        var map = new Dictionary<string, Action<int>>();

        map.Add("closeAd", _ => actions.TryCloseAd());
        map.Add("closeAd_1", _ => actions.TryCloseAd());
        map.Add("closeAd_2", _ => actions.TryCloseAd());
        map.Add("closeAd_3", _ => actions.TryCloseAd());
        map.Add("closeAd_4", _ => actions.TryCloseAd());
        map.Add("closeAd_5", _ => actions.TryCloseAd());
        map.Add("closeAd_6", _ => actions.TryCloseAd());
        map.Add("closeAd_7", _ => actions.TryCloseAd());
        map.Add("closeAd_8", _ => actions.TryCloseAd());
        map.Add("closeAd_9", _ => actions.TryCloseAd());

        map.AddMapping(GameButton.Continue, actions.PressContinue);
        map.AddMapping(GameButton.Awesome, actions.PressContinue);
        map.AddMapping(GameButton.Back, _ => actions.PressExitButton());
        map.AddMapping(GameButton.Gift, actions.CollectFreeBux);
        map.AddMapping(GameButton.Restock, _ => actions.Restock());
        map.AddMapping(GameButton.Collect, x => actions.ClickAndWaitMs(x, 300));
        map.AddMapping(GameButton.NewScience, actions.CollectNewScience);
        map.AddMapping(GameButton.FreeBuxGift, _ => actions.PressFreeBuxButton());
        map.AddMapping(GameButton.ParachuteGift, actions.ClickOnChute);
        map.AddMapping(GameButton.RideElevator, _ => actions.RideElevator());
        map.AddMapping(GameButton.GameIcon, actions.OpenTheGame);
        map.AddMapping(GameButton.NewQuest, actions.PressQuestButton);
        map.AddMapping(GameButton.CompletedQuest, actions.CompleteQuest);
        map.AddMapping(GameButton.TowerManagementWarning, actions.CheckTowerManagementActions);

        map.AddMapping(GameWindow.RoofCustomization, _ => actions.ExitRoofCustomizationMenu());
        map.AddMapping(GameWindow.HurryConstructionWithBux, _ => actions.CancelHurryConstructionWithBux());
        map.AddMapping(GameWindow.FoundCoinsOnParachuteClick, _ => actions.CloseChuteNotification());
        map.AddMapping(GameWindow.WatchCoinsAdsPrompt, _ => actions.TryWatchAds());
        map.AddMapping(GameWindow.WatchBuxAdsPrompt, _ => actions.TryWatchAds());
        map.AddMapping(GameWindow.FindBitizensQuestPrompt, _ => actions.FindBitizens());
        map.AddMapping(GameWindow.DeliverBitizensQuestPrompt, _ => actions.DeliverBitizens());
        map.AddMapping(GameWindow.NewFloorMenu, _ => actions.CloseNewFloorMenu());
        map.AddMapping(GameWindow.BuildNewFloorNotification, _ => actions.CloseBuildNewFloorPrompt());
        map.AddMapping(GameWindow.AdsLostRewardNotification, _ => actions.CheckIfAdsRewardWillBeLost());

        return map;
    }

    private static void AddMapping(this IDictionary<string, Action<int>> dictionary, Enum gameElement, Action<int> action)
    {
        dictionary.Add(gameElement.GetDescription(), action);
    }
}
