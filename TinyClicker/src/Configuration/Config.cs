using System;

namespace TinyClicker;

public class Config
{
    public bool VipPackage { get; set; }
    public float ElevatorSpeed { get; set; }
    public int CurrentFloor { get; set; }
    public DateTime LastRebuildTime { get; set; }
    public int RebuildAtFloor { get; set; }
    public int WatchAdsFromFloor { get; set; }
    public bool WatchBuxAds { get; set; }

    public Config() : this(true, 10f, 3, 50, 25, false, DateTime.Now) { }
    public Config(bool vip, float elevatorSpeed, int currentFloor, int rebuildAtFloor, int watchAdsFromFloor, bool watchBuxAds, DateTime lastRebuildTime)
    {
        VipPackage = vip;
        ElevatorSpeed = elevatorSpeed;
        CurrentFloor = currentFloor;
        LastRebuildTime = lastRebuildTime;
        RebuildAtFloor = rebuildAtFloor;
        WatchAdsFromFloor = watchAdsFromFloor;
        WatchBuxAds = watchBuxAds;
    }
}
