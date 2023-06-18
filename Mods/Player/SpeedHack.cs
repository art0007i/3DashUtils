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

    public override void Start()
    {
        lastText = valueOption.Value.ToString();
    }
    public override void Update()
    {
        if (!Extensions.GetPauseState())
        {
            Time.timeScale = Enabled.Value ? Value.Value : 1f;
        }
    }
    /*
    public override void OnGUI()
    {
        GUILayout.BeginHorizontal();
        {
            lastText = GUILayout.TextField(lastText, GUILayout.Width(50));
            if (float.TryParse(lastText, out var i))
            {
                if (i > 0)
                {
                    valueOption.Value = i;
                }
            }
            if (GUILayout.Button("Speed Hack: " + ModuleUtils.GetEnabledText(toggleOption.Value), GUILayout.ExpandWidth(true)))
            {
                toggleOption.Value = !toggleOption.Value;
            }
        }
        GUILayout.EndHorizontal();
    }*/


    public override bool TryParseText(string text, out float parse)
    {
        return float.TryParse(lastText, out parse) && parse > 0;
    }
}
