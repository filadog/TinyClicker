using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.IO;

namespace TinyClicker
{
    internal class Config
    {
        private readonly bool _vip;
        private readonly float _elevatorSpeed;

        public bool Vip => _vip;
        public float ElevatorSpeed => _elevatorSpeed;

        public Config() : this(true, 10f) { }
        public Config(bool vip, float elevatorSpeed)
        {
            _vip = vip;
            _elevatorSpeed = elevatorSpeed;
        }
    }

    internal class ConfigManager
    {
        static string configPath = Environment.CurrentDirectory + @"\config.txt";

        public static void SaveConfig()
        {
            Config config = new Config();
            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(config, options);
            File.WriteAllText(configPath, json);
        }

        public static Config GetConfig()
        {
            string json = File.ReadAllText(configPath);
            var config = JsonSerializer.Deserialize<Config>(json);
            return config;
        }
    }
}
