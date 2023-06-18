using _3DashUtils.ModuleSystem;
using BepInEx.Configuration;
using UnityEngine;

namespace _3DashUtils.Mods.Misc;
public class TargetFPS : TextEditorModule<int>
{
    public static ConfigEntry<bool> enabledOption = _3DashUtils.ConfigFile.Bind("Misc", "TargetFpsEnabled", false);
    public static ConfigEntry<int> valueOption = _3DashUtils.ConfigFile.Bind("Misc", "TargetFpsValue", 60);

    public override string CategoryName => "Misc";

    public override string ModuleName => "Target FPS";

    public override float Priority => -0.9f;

    public override ConfigEntry<int> Value => valueOption;

    public override ConfigEntry<bool> Enabled => enabledOption;

    public override string Tooltip => "Allows you to select a framerate that the game will run at.";

    public override bool TryParseText(string text, out int parse)
    {
        return int.TryParse(lastText, out parse) && parse > 0;
    }
    public override void Update()
    {
        Application.targetFrameRate = Enabled.Value ? Value.Value : -1;
    }
}