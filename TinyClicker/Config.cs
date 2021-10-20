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
        private bool _vip;
        private float _elevatorSpeed;

        public bool Vip { get => _vip; set => _vip = value; }
        public float ElevatorSpeed { get => _elevatorSpeed; set => _elevatorSpeed = value; }

        public Config() : this(true, 10f) { }
        public Config(bool vip, float elevatorSpeed)
        {
            Vip = vip;
            ElevatorSpeed = elevatorSpeed;
        }


    }

    internal class ConfigManager
    {
        static string configPath = Environment.CurrentDirectory + @"\Config.txt";

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
