using System;
using System.Text.Json;
using System.IO;

namespace TinyClicker;

public class ConfigManager
{
    public Config curConfig;
    static readonly string _configPath = Environment.CurrentDirectory + @"\Config.txt";

    public ConfigManager()
    {
        curConfig = GetConfig();
        SaveConfig(curConfig);
    }

    public void AddOneFloor()
    {
        curConfig.CurrentFloor += 1;
        SaveConfig(curConfig);
    }

    public void SetCurrentFloor(int floor)
    {
        curConfig.CurrentFloor = floor;
        SaveConfig(curConfig);
    }

    public void SaveNewRebuildTime(DateTime rebuildTime)
    {
        curConfig.LastRebuildTime = rebuildTime;
        SaveConfig(curConfig);
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
    }

    public void SaveConfig()
    {
        SaveConfig(curConfig);
    }

    public void SaveStatRebuildTime()
    {
        DateTime dateTimeNow = DateTime.Now;
        DateTime lastRebuild = curConfig.LastRebuildTime;
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
