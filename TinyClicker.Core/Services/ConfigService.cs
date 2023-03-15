using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.IO;
using System.Linq;
using TinyClicker.Core.Logic;

namespace TinyClicker.Core.Services;

public class ConfigService : IConfigService
{
    private const string STATS_PATH = "./Stats.txt";
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

    public void SaveNewRebuildTime(DateTime rebuildTime)
    {
        Config.LastRebuildTime = rebuildTime;
        SaveConfig(Config);
    }

    public Config GetConfig()
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
            var header = "rebuild time | time since last rebuild | elevator rides |\n";
            var stats = File.ReadAllLines(STATS_PATH);
            if (!stats.Contains(header))
            {
                File.AppendAllText(STATS_PATH, header);
            }
        }

        var maxRebuildLength = 24;
        if (timeSinceRebuild.Length < maxRebuildLength)
        {
            timeSinceRebuild += new string(' ', timeSinceRebuild.Length - maxRebuildLength);
        }

        var maxElevatorRides = 15;
        var rides = Config.ElevatorRides.ToString();
        if (rides.Length < maxElevatorRides)
        {
            rides += new string(' ', rides.Length - maxElevatorRides);
        }

        var line = $"{dateTimeNow:dd.MM.yyyy hh:mm:ss} | {timeSinceRebuild}| {rides}|\n";
        File.AppendAllText(STATS_PATH, line);
    }
}
