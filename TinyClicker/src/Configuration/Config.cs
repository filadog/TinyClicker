using System;

namespace TinyClicker;

public class Config
{
    private bool _vipPackage;
    private float _elevatorSpeed;
    private int _floorsNumber;
    private int _coins;
    private DateTime _lastRebuildTime;

    public Config() : this(true, 10f, 3) { }
    public Config(bool vip, float elevatorSpeed, int floorsNumber)
    {
        VipPackage = vip;
        ElevatorSpeed = elevatorSpeed;
        FloorsNumber = floorsNumber;
        Coins = 0;
    }

    public bool VipPackage { get => _vipPackage; set => _vipPackage = value; }
    public float ElevatorSpeed { get => _elevatorSpeed; set => _elevatorSpeed = value; }
    public int FloorsNumber { get => _floorsNumber; set => _floorsNumber = value; }
    public int Coins { get => _coins; set => _coins = value; }
    public DateTime LastRebuildTime { get => _lastRebuildTime; set => _lastRebuildTime = value; }
}
