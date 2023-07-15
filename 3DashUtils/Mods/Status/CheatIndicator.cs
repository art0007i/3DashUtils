using _3DashUtils;
using _3DashUtils.ModuleSystem;
using _3DashUtils.ModuleSystem.Config;
using UnityEngine;

namespace Mods.Status;

internal class CheatIndicator : ToggleModule
{
    public override string CategoryName => "Status";

    public override string ModuleName => "Cheat Indicator";

    public override string Description => "Displays a red dot when playing a level if any cheat modules are turned on.";

    public static bool ShowNonCheats => showNonCheatsOption.Value;

    protected override bool Default => true;

    public static ConfigOptionBase<bool> showNonCheatsOption;

    const int spacing = 5;
    const int fontSize = 24;

    public CheatIndicator()
    {
        showNonCheatsOption = new ToggleConfigOption(this, "Show Non Cheats", false, "Show a green dot when no cheats are enabled.");
    }

    private static GUIStyle dotStyle;
    public override void OnUnityGUI()
    {
        var c = Extensions.CheatsEnabled();
        if (Enabled && (c || ShowNonCheats))
        {
            if (dotStyle == null)
            {
                dotStyle = new GUIStyle(GUI.skin.label);
                dotStyle.fontSize = fontSize;
            }
            dotStyle.normal.textColor = c ? Color.red : Color.green;
            GUI.Label(new Rect(spacing, Screen.height - fontSize - spacing, 200, fontSize * 2), "●", dotStyle);
        }
    }
}
