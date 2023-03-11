using System;
using TinyClicker.Core.Logic;

namespace TinyClicker.Core.Services
{
    public interface IConfigService
    {
        Config Config { get; }

        void AddOneFloor();
        Config GetConfig();
        void SaveConfig();
        void SaveConfig(Config config);
        void SaveNewRebuildTime(DateTime rebuildTime);
        void SaveStatRebuildTime();
        void SetCurrentFloor(int floor);
    }
}