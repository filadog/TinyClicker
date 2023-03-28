using TinyClicker.Core.Logic;

namespace TinyClicker.Core.Services;

public interface IConfigService
{
    Config Config { get; }

    void AddOneFloor();
    void SaveConfig();
    void SaveConfig(Config config);
    void SaveStatRebuildTime();
    void SetCurrentFloor(int floor);
}