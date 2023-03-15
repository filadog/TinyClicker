using System;
using System.Text.Json;
using System.IO;
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
        var result = "";
        if (lastRebuildTime != DateTime.MinValue)
        {
            var diff = dateTimeNow - lastRebuildTime;
            var formatted = diff.ToString(@"hh\:mm\:ss");
            if (diff.Days >= 1)
            {
                result = $"{diff.Days} days " + formatted;
            }
            else
            {
                result = formatted;
            }
        }

        SaveNewRebuildTime(dateTimeNow);
        var data = $"{dateTimeNow} - rebuilt the tower. Time elapsed since the last rebuild: {result}\n";
        File.AppendAllText(STATS_PATH, data);
    }
}
