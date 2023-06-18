using _3DashUtils.ModuleSystem;
using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace _3DashUtils.Mods.Visual;

internal class CheatIndicator : ToggleModule
{
    public static ConfigEntry<bool> option = _3DashUtils.ConfigFile.Bind("Visual", "CheatIndicator", false);
    public override string CategoryName => "Visual";

    public override string ModuleName => "Cheat Indicator";

    public override ConfigEntry<bool> Enabled => option;

    public override string Tooltip => "Displays a red dot when playing a level if any cheat modules are turned on.";

    const int spacing = 5;
    const int fontSize = 24;

    public override void OnUnityGUI()
    {
        if(Enabled.Value && Extensions.CheatsEnabled())
        {
            var style = new GUIStyle(GUI.skin.label);
            style.fontSize = fontSize;
            style.normal.textColor = Color.red;
            GUI.Label(new Rect(spacing, Screen.height - fontSize - spacing, 200, fontSize * 2), "●", style);
        }

        //add thingy when suboption is not kil to make it green when not epic gaming hacking
    }
}
