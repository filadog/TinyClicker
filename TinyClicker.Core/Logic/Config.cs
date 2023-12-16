using System;

namespace TinyClicker.Core.Logic;

public class Config
{
    public Config() : this(true, 10f, 4, 50, 25, false, DateTime.Now, true, default, true, 0, 0)
    {
    }

    public Config(
        bool vip,
        float elevatorSpeed,
        int currentFloor,
        int rebuildAtFloor,
        int watchAdsFromFloor,
        bool watchBuxAds,
        DateTime lastRebuildTime,
        bool buildFloors,
        DateTime lastRaffleTime,
        bool isBluestacks,
        int elevatorRides,
        int floorCostDecrease)
    {
        VipPackage = vip;
        ElevatorSpeed = elevatorSpeed;
        CurrentFloor = currentFloor;
        LastRebuildTime = lastRebuildTime;
        RebuildAtFloor = rebuildAtFloor;
        WatchAdsFromFloor = watchAdsFromFloor;
        WatchBuxAds = watchBuxAds;
        BuildFloors = buildFloors;
        LastRaffleTime = lastRaffleTime;
        IsBluestacks = isBluestacks;
        ElevatorRides = elevatorRides;
        FloorCostDecrease = floorCostDecrease;
    }

    public bool VipPackage { get; init; }
    public float ElevatorSpeed { get; init; }
    public int CurrentFloor { get; set; }
    public DateTime LastRebuildTime { get; set; }
    public int RebuildAtFloor { get; init; }
    public int WatchAdsFromFloor { get; init; }
    public bool WatchBuxAds { get; init; }
    public bool BuildFloors { get; init; }
    public DateTime LastRaffleTime { get; set; }
    public bool IsBluestacks { get; set; }
    public int ElevatorRides { get; set; }
    public int FloorCostDecrease { get; init; }
}
