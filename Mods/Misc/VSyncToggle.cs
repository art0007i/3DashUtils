using _3DashUtils.ModuleSystem;
using BepInEx.Configuration;
using UnityEngine;

namespace _3DashUtils.Mods.Misc;

public class VSyncToggle : ToggleModule
{
    public static ConfigEntry<bool> option = _3DashUtils.ConfigFile.Bind("Misc", "VSync", true);
    public override float Priority => -0.8f;

    public override string CategoryName => "Misc";

    public override string ModuleName => "VSync";

    public override ConfigEntry<bool> Enabled => option;

    public override string Tooltip => "Enables VSync.";

    public override void Update()
    {
        //QualitySettings.vSyncCount = (Plugin.vsync.Value ? 1 : 0);
    }

    public override void OnToggle()
    {
        QualitySettings.vSyncCount = Enabled.Value ? 1 : 0;
    }
}
