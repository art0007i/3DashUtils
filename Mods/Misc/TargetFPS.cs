using _3DashUtils.ModuleSystem;
using _3DashUtils.ModuleSystem.Config;
using UnityEngine;

namespace _3DashUtils.Mods.Misc;
public class TargetFPS : ToggleModule
{
    public override string CategoryName => "Misc";

    public override string ModuleName => "Target FPS";

    public override float Priority => -0.9f;

    public static int Value => valueConfig.Value;
    public static ConfigOptionBase<int> valueConfig;

    public override string Description => "Allows you to select a framerate that the game will run at.";

    protected override bool Default => false;

    public TargetFPS()
    {
        valueConfig = new TextInputConfig<int>(this, "FPS", 60, "The FPS value that the game will lock to.", TryParseFPS);
    }

    public static bool TryParseFPS(string text, out int value)
    {
        return int.TryParse(text, out value) && value > 0;
    }

    public override void Update()
    {
        Application.targetFrameRate = Enabled ? Value: -1;
    }
}