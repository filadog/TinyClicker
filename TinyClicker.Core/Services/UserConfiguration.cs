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

    public UserConfiguration()
    {
        Configuration = GetConfiguration();
        SaveConfig(Configuration);
    }

    public Configuration Configuration { get; private set; }

    public void AddOneFloor()
    {
        Configuration.CurrentFloor += 1;
        SaveConfig(Configuration);
    }

    public void SetCurrentFloor(int floor)
    {
        Configuration.CurrentFloor = floor;
        SaveConfig(Configuration);
    }

    public void SaveConfig(Configuration config)
    {
        var options = new JsonSerializerOptions { WriteIndented = true };
        var json = JsonSerializer.Serialize(config, options);
        File.WriteAllText(_configPath, json);
        Configuration = config;
    }

    public void SaveConfig()
    {
        SaveConfig(Configuration);
    }

    public void SaveStatRebuildTime()
    {
        var dateTimeNow = DateTime.Now;
        var lastRebuildTime = Configuration.LastRebuildTime;
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
        var rides = Configuration.ElevatorRides.ToString();
        if (rides.Length < maxElevatorRidesLength)
        {
            rides += new string(' ', maxElevatorRidesLength - rides.Length);
        }

        var line = $"{dateTimeNow:dd.MM.yyyy HH:mm:ss} | {timeSinceRebuild}| {rides}|\n";
        File.AppendAllText(STATS_PATH, line);
    }

    private void SaveNewRebuildTime(DateTime rebuildTime)
    {
        Configuration.LastRebuildTime = rebuildTime;
        SaveConfig(Configuration);
    }

    private Configuration GetConfiguration()
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
