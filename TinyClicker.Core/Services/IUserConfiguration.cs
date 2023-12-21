using System;
using TinyClicker.Core.Logic;

namespace TinyClicker.Core.Services;

public interface IUserConfiguration
{
    public bool VipPackage { get; }
    public float ElevatorSpeed { get; }
    public int CurrentFloor { get; }
    public DateTime LastRebuildTime { get; }
    public int RebuildAtFloor { get; }
    public int WatchAdsFromFloor { get; }
    public bool WatchBuxAds { get; }
    public bool BuildFloors { get; }
    public DateTime LastRaffleTime { get; }
    public bool IsBluestacks { get; }
    public int ElevatorRides { get; }
    public int FloorCostDecrease { get; }
    public int GameScreenScanningRateMs { get; }

    void AddElevatorRide();
    void AddOneFloor();
    void ResetElevatorRides();
    void SaveConfiguration();
    void SaveLastRaffleTime(DateTime rebuildTime);
    void SaveLastUsedEmulator(bool isBluestacks);
    void SaveConfiguration(Configuration newConfiguration);
    void SaveRebuildTime();
    void SetCurrentFloor(int floor);
}
