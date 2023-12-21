using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using TinyClicker.Core.Logic;

namespace TinyClicker.Core.Services;

public class UserConfiguration : IUserConfiguration
{
    private const string STATS_PATH = "./Stats.txt";
    private const string HEADER = "rebuild time        | time since last rebuild | elevator rides |";

    private readonly string _configPath = Environment.CurrentDirectory + "/Config.txt";
    private Configuration _configuration;

    public UserConfiguration()
    {
        _configuration = ReadConfiguration();
        SaveConfiguration(_configuration);
    }

    public bool VipPackage => _configuration.VipPackage;
    public float ElevatorSpeed => _configuration.ElevatorSpeed;
    public int CurrentFloor => _configuration.CurrentFloor;
    public DateTime LastRebuildTime => _configuration.LastRebuildTime;
    public int RebuildAtFloor => _configuration.RebuildAtFloor;
    public int WatchAdsFromFloor => _configuration.WatchAdsFromFloor;
    public bool WatchBuxAds => _configuration.WatchBuxAds;
    public bool BuildFloors => _configuration.BuildFloors;
    public DateTime LastRaffleTime => _configuration.LastRaffleTime;
    public bool IsBluestacks => _configuration.IsBluestacks;
    public int ElevatorRides => _configuration.ElevatorRides;
    public int FloorCostDecrease => _configuration.FloorCostDecrease;
    public int GameScreenScanningRateMs => _configuration.GameScreenScanningRateMs;

    public void AddOneFloor()
    {
        _configuration.CurrentFloor++;
        SaveConfiguration();
    }

    public void AddElevatorRide()
    {
        _configuration.ElevatorRides++;
        SaveConfiguration();
    }

    public void ResetElevatorRides()
    {
        _configuration.ElevatorRides = 0;
        SaveConfiguration();
    }

    public void SetCurrentFloor(int floor)
    {
        _configuration.CurrentFloor = floor;
        SaveConfiguration();
    }

    public void SaveLastUsedEmulator(bool isBluestacks)
    {
        _configuration.IsBluestacks = isBluestacks;
        SaveConfiguration();
    }

    public void SaveLastRaffleTime(DateTime rebuildTime)
    {
        if (rebuildTime == default)
        {
            throw new ArgumentNullException(nameof(rebuildTime));
        }

        _configuration.LastRaffleTime = rebuildTime;
        SaveConfiguration();
    }

    public void SaveConfiguration(Configuration newConfiguration)
    {
        var options = new JsonSerializerOptions { WriteIndented = true };
        var json = JsonSerializer.Serialize(newConfiguration, options);
        File.WriteAllText(_configPath, json);
        _configuration = newConfiguration;
    }

    public void SaveConfiguration()
    {
        SaveConfiguration(_configuration);
    }

    public void SaveRebuildTime()
    {
        var dateTimeNow = DateTime.Now;
        var lastRebuildTime = _configuration.LastRebuildTime;
        var timeSinceRebuild = string.Empty;

        if (lastRebuildTime != DateTime.MinValue)
        {
            var diff = dateTimeNow - lastRebuildTime;
            var formatted = diff.ToString(@"hh\:mm\:ss");
            timeSinceRebuild = diff.Days >= 1 ? $"{diff.Days} days " + formatted : formatted;
        }

        SaveNewRebuildTime(dateTimeNow);

        if (File.Exists(STATS_PATH))
        {
            var stats = File.ReadAllLines(STATS_PATH);
            if (!stats.Contains(HEADER))
            {
                File.AppendAllText(STATS_PATH, HEADER + "\n");
            }
        }

        const int maxRebuildLength = 24;
        if (timeSinceRebuild.Length < maxRebuildLength)
        {
            timeSinceRebuild += new string(' ', maxRebuildLength - timeSinceRebuild.Length);
        }

        const int maxElevatorRidesLength = 15;
        var rides = _configuration.ElevatorRides.ToString();
        if (rides.Length < maxElevatorRidesLength)
        {
            rides += new string(' ', maxElevatorRidesLength - rides.Length);
        }

        var line = $"{dateTimeNow:dd.MM.yyyy HH:mm:ss} | {timeSinceRebuild}| {rides}|\n";
        File.AppendAllText(STATS_PATH, line);
    }

    private void SaveNewRebuildTime(DateTime rebuildTime)
    {
        _configuration.LastRebuildTime = rebuildTime;
        SaveConfiguration();
    }

    private Configuration ReadConfiguration()
    {
        if (!File.Exists(_configPath))
        {
            return new Configuration();
        }

        var json = File.ReadAllText(_configPath);
        var result = JsonSerializer.Deserialize<Configuration>(json);

        return result ?? throw new InvalidOperationException("Invalid configuration file");
    }
}
