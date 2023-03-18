using System;
using System.Text.Json;
using System.IO;
using System.Linq;
using TinyClicker.Core.Logic;

namespace TinyClicker.Core.Services;

public class ConfigService : IConfigService
{
    private const string STATS_PATH = "./Stats.txt";
    private const string HEADER = "rebuild time        | time since last rebuild | elevator rides |";

    private readonly string _configPath = Environment.CurrentDirectory + "/Config.txt";

    public Config Config { get; private set; }

    public ConfigService()
    {
        Config = GetConfig();
        SaveConfig(Config);
    }

    public void AddOneFloor()
    {
        Config.CurrentFloor += 1;
        SaveConfig(Config);
    }

    public void SetCurrentFloor(int floor)
    {
        Config.CurrentFloor = floor;
        SaveConfig(Config);
    }

    private void SaveNewRebuildTime(DateTime rebuildTime)
    {
        Config.LastRebuildTime = rebuildTime;
        SaveConfig(Config);
    }

    private Config GetConfig()
    {
        if (!File.Exists(_configPath))
        {
            return new Config();
        }

        var json = File.ReadAllText(_configPath);
        var result = JsonSerializer.Deserialize<Config>(json);

        return result ?? throw new InvalidOperationException("Invalid configuration file");
    }

    public void SaveConfig(Config config)
    {
        var options = new JsonSerializerOptions { WriteIndented = true };
        var json = JsonSerializer.Serialize(config, options);
        File.WriteAllText(_configPath, json);
        Config = config;
    }

    public void SaveConfig()
    {
        SaveConfig(Config);
    }

    public void SaveStatRebuildTime()
    {
        var dateTimeNow = DateTime.Now;
        var lastRebuildTime = Config.LastRebuildTime;
        var timeSinceRebuild = "";
        if (lastRebuildTime != DateTime.MinValue)
        {
            var diff = dateTimeNow - lastRebuildTime;
            var formatted = diff.ToString(@"hh\:mm\:ss");
            if (diff.Days >= 1)
            {
                timeSinceRebuild = $"{diff.Days} days " + formatted;
            }
            else
            {
                timeSinceRebuild = formatted;
            }
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
        var rides = Config.ElevatorRides.ToString();
        if (rides.Length < maxElevatorRidesLength)
        {
            rides += new string(' ', maxElevatorRidesLength - rides.Length);
        }

        var line = $"{dateTimeNow:dd.MM.yyyy HH:mm:ss} | {timeSinceRebuild}| {rides}|\n";
        File.AppendAllText(STATS_PATH, line);
    }
}
