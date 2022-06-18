using System;
using System.Text.Json;
using System.IO;

namespace TinyClicker;

public class ConfigManager
{
    static readonly string _configPath = Environment.CurrentDirectory + @"\Config.txt";

    public static void AddOneFloor()
    {
        var config = TinyClickerApp.currentConfig;
        config.FloorsNumber += 1;
        SaveConfig(config);
    }

    public static void ChangeCurrentFloor(int floor)
    {
        var config = TinyClickerApp.currentConfig;
        config.FloorsNumber = floor;
        SaveConfig(config);
    }

    public static void SaveNewRebuildTime(DateTime rebuildTime)
    {
        var config = TinyClickerApp.currentConfig;
        config.LastRebuildTime = rebuildTime;
        SaveConfig(config);
    }

    public static Config GetConfig()
    {
        string json = File.ReadAllText(_configPath);
        var config = JsonSerializer.Deserialize<Config>(json);
        return config;
    }

    static void SaveConfig(Config config)
    {
        var options = new JsonSerializerOptions { WriteIndented = true };
        string json = JsonSerializer.Serialize(config, options);
        File.WriteAllText(_configPath, json);
    }
}
