using System;
using System.Text.Json;
using System.IO;

namespace TinyClicker.Core.Logic;

public class ConfigManager
{
    static readonly string _configPath = Environment.CurrentDirectory + @"\Config.txt";

    public Config CurrentConfig { get; private set; }

    public ConfigManager()
    {
        CurrentConfig = GetConfig();
        SaveConfig(CurrentConfig);
    }

    public void AddOneFloor()
    {
        CurrentConfig.CurrentFloor += 1;
        SaveConfig(CurrentConfig);
    }

    public void SetCurrentFloor(int floor)
    {
        CurrentConfig.CurrentFloor = floor;
        SaveConfig(CurrentConfig);
    }

    public void SaveNewRebuildTime(DateTime rebuildTime)
    {
        CurrentConfig.LastRebuildTime = rebuildTime;
        SaveConfig(CurrentConfig);
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
        CurrentConfig = config;
    }

    public void SaveConfig()
    {
        SaveConfig(CurrentConfig);
    }

    public void SaveStatRebuildTime()
    {
        DateTime dateTimeNow = DateTime.Now;
        DateTime lastRebuild = CurrentConfig.LastRebuildTime;
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
