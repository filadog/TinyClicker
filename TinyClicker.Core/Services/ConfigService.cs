using System;
using System.Text.Json;
using System.IO;
using TinyClicker.Core.Logic;

namespace TinyClicker.Core.Services;

public class ConfigService
{
    static readonly string _configPath = Environment.CurrentDirectory + @"\Config.txt";

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
        try
        {
            string json = File.ReadAllText(_configPath);
            var config = JsonSerializer.Deserialize<Config>(json);
            if (config != null)
            {
                return config;
            }
            else
            {
                return new Config();
            }
        }
        catch (FileNotFoundException)
        {
            return new Config();
        }
    }

    public void SaveConfig(Config config)
    {
        var options = new JsonSerializerOptions { WriteIndented = true };
        string json = JsonSerializer.Serialize(config, options);
        File.WriteAllText(_configPath, json);
        Config = config;
    }

    public void SaveConfig()
    {
        SaveConfig(Config);
    }

    public void SaveStatRebuildTime()
    {
        DateTime dateTimeNow = DateTime.Now;
        DateTime lastRebuild = Config.LastRebuildTime;
        string result = "";
        if (lastRebuild != DateTime.MinValue)
        {
            TimeSpan diff = dateTimeNow - lastRebuild;
            string formatted = diff.ToString(@"hh\:mm\:ss");
            if (diff.Days >= 1)
            {
                result = string.Format("{0} days ", diff.Days) + formatted;
            }
            else
            {
                result = formatted;
            }
        }

        string statsPath = $"./Stats.txt";
        SaveNewRebuildTime(dateTimeNow);
        string data = $"{dateTimeNow} - rebuilt the tower. Time elapsed since the last rebuild: {result}\n";
        File.AppendAllText(statsPath, data);
    }
}
