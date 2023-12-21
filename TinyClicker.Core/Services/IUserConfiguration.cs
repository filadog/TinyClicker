using TinyClicker.Core.Logic;

namespace TinyClicker.Core.Services;

public interface IUserConfiguration
{
    Configuration Configuration { get; }

    void AddOneFloor();
    void SaveConfig();
    void SaveConfig(Configuration config);
    void SaveStatRebuildTime();
    void SetCurrentFloor(int floor);
}
