using _3DashUtils.ModuleSystem;
using BepInEx.Configuration;
using UnityEngine;

namespace _3DashUtils.Mods.Misc;

public class VSyncToggle : ToggleModule
{
    public override float Priority => -0.8f;

    public override string CategoryName => "Misc";

    public override string ModuleName => "VSync";

    public override string Description => "Enables VSync.";

    protected override bool Default => true;

    public override void OnToggle()
    {
        QualitySettings.vSyncCount = Enabled ? 1 : 0;
    }
}
