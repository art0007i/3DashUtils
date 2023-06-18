using _3DashUtils.ModuleSystem;
using BepInEx.Configuration;
using System.Reflection;
using UnityEngine;

namespace _3DashUtils.Mods.Player;

public class SpeedHack : TextEditorModule<float>
{
    public static ConfigEntry<bool> enabledOption = _3DashUtils.ConfigFile.Bind("Player", "SpeedHackEnabled", false);
    public static ConfigEntry<float> valueOption = _3DashUtils.ConfigFile.Bind("Player", "SpeedHackValue", 1f);

    public override string CategoryName => "Player";

    public override float Priority => 0.11f;

    public override ConfigEntry<float> Value => valueOption;

    public override ConfigEntry<bool> Enabled => enabledOption;

    public override string Tooltip => "Changes the speed of the game.";

    public override bool IsCheat => Enabled.Value && Value.Value < 1;
    public override void Update()
    {
        if (!Extensions.GetPauseState())
        {
            Time.timeScale = Enabled.Value ? Value.Value : 1f;
        }
    }

    public override bool TryParseText(string text, out float parse)
    {
        return float.TryParse(lastText, out parse) && parse > 0;
    }
}
