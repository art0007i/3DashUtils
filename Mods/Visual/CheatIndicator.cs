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

    public override void OnGUI()
    {
        base.OnGUI();
        if(Enabled.Value)
        {
            GUI.Label(new Rect(5, 5, 200, 20), "CHEATING");
        }
    }
}
