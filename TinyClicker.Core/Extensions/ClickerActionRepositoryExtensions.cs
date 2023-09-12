using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyClicker.Core.Logic;

namespace TinyClicker.Core.Extensions;

public static class ClickerActionRepositoryExtensions
{
    public static Dictionary<string, Action<int>> GetActionsMap(this ClickerActionsRepository actions)
    {
        var map = new Dictionary<string, Action<int>>();

        map.Add("closeAd", x => actions.CloseAd());
        map.Add("closeAd_1", x => actions.CloseAd());
        map.Add("closeAd_2", x => actions.CloseAd());
        map.Add("closeAd_3", x => actions.CloseAd());
        map.Add("closeAd_4", x => actions.CloseAd());
        map.Add("closeAd_5", x => actions.CloseAd());
        map.Add("closeAd_6", x => actions.CloseAd());
        map.Add("closeAd_7", x => actions.CloseAd());
        map.Add("closeAd_8", x => actions.CloseAd());
        map.Add("closeAd_9", x => actions.CloseAd());

        map.Add("new_gifts_button", x => actions.CollectFreeBux(x));
        map.Add("roofCustomizationWindow", x => actions.ExitRoofCustomizationMenu());
        map.Add("hurryConstructionPrompt", x => actions.CancelHurryConstruction());
        map.Add("continueButton", x => actions.PressContinue(x));
        map.Add("foundCoinsChuteNotification", x => actions.CloseChuteNotification());
        map.Add("restockButton", x => actions.Restock());
        map.Add("freeBuxButton", x => actions.PressFreeBuxButton());
        map.Add("giftChute", x => actions.ClickOnChute(x));
        map.Add("backButton", x => actions.PressExitButton());
        map.Add("elevatorButton", x => actions.RideElevator());
        map.Add("questButton", x => actions.PressQuestButton(x));
        map.Add("completedQuestButton", x => actions.CompleteQuest(x));
        map.Add("watchAdPromptCoins", x => actions.TryWatchAds());
        map.Add("watchAdPromptBux", x => actions.TryWatchAds());
        map.Add("findBitizens", x => actions.FindBitizens());
        map.Add("deliverBitizens", x => actions.DeliverBitizens());
        map.Add("newFloorMenu", x => actions.CloseNewFloorMenu());
        map.Add("buildNewFloorNotification", x => actions.CloseBuildNewFloorNotification());
        map.Add("gameIcon", x => actions.OpenTheGame(x));
        map.Add("adsLostReward", x => actions.CheckForLostAdsReward());
        map.Add("newScienceButton", x => actions.CollectNewScience(x));

        return map;
    }
}
