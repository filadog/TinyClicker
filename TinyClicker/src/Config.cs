using System;
using System.Text.Json;
using System.IO;

namespace TinyClickerUI
{
    public class Config
    {
        private bool _vipPackage;
        private float _elevatorSpeed;
        private int _floorsNumber;
        private int _coins;
        private DateTime _lastRebuildTime;

        public Config() : this(true, 10f, 3) { }
        public Config(bool vip, float elevatorSpeed, int floorsNumber)
        {
            VipPackage = vip;
            ElevatorSpeed = elevatorSpeed;
            FloorsNumber = floorsNumber;
            Coins = 0;
        }

        public bool VipPackage { get => _vipPackage; set => _vipPackage = value; }
        public float ElevatorSpeed { get => _elevatorSpeed; set => _elevatorSpeed = value; }
        public int FloorsNumber { get => _floorsNumber; set => _floorsNumber = value; }
        public int Coins { get => _coins; set => _coins = value; }
        public DateTime LastRebuildTime { get => _lastRebuildTime; set => _lastRebuildTime = value; }
    }

    public class ConfigManager
    {
        static readonly string _configPath = Environment.CurrentDirectory + @"\Config.txt";

        public static void AddOneFloor()
        {
            var config = TinyClicker.currentConfig;
            config.FloorsNumber += 1;
            SaveConfig(config);
        }

        public static void ChangeCurrentFloor(int floor)
        {
            var config = TinyClicker.currentConfig;
            config.FloorsNumber = floor;
            SaveConfig(config);
        }

        public static void SaveNewRebuildTime(DateTime rebuildTime)
        {
            var config = TinyClicker.currentConfig;
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
}
