using System;

namespace TinyClicker.UI.ViewModels;

public class UserSettingsViewModel
{
    public int CurrentFloor { get; set; }
    public int RebuildAtFloor { get; set; }
    public int WatchAdsFromFloor { get; set; }
    public int FloorCostDecreasePercent { get; set; }
    public bool WatchBuxAds { get; set; }
    public bool BuildFloors { get; set; }
    public DateTime LastRebuildTime { get; set; }
    public DateTime LastRaffleTime { get; set; }
    public int GameScreenScanningRateMs { get; set; }
}
