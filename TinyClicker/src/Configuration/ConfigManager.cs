using System;
using System.Text.Json;
using System.IO;

namespace TinyClicker;

public class ConfigManager
{
    public Config _curConfig;
    static readonly string _configPath = Environment.CurrentDirectory + @"\Config.txt";

    public ConfigManager()
    {
        _curConfig = GetConfig();
        SaveConfig(_curConfig);
    }

    public void AddOneFloor()
    {
        //var config = _clickerApp._currentConfig;
        _curConfig.CurrentFloor += 1;
        SaveConfig(_curConfig);
    }

    public void ChangeCurrentFloor(int floor)
    {
        //var config = _clickerApp._currentConfig;
        _curConfig.CurrentFloor = floor;
        SaveConfig(_curConfig);
    }

    public void SaveNewRebuildTime(DateTime rebuildTime)
    {
        //var config = _clickerApp._currentConfig;
        _curConfig.LastRebuildTime = rebuildTime;
        SaveConfig(_curConfig);
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
        SaveConfig(_curConfig);
    }
}
