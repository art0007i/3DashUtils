using _3DashUtils.ModuleSystem;
using BepInEx.Configuration;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3DashUtils.Mods.Player;

public class NoPauseSuicide : ToggleModule
{
    public static ConfigEntry<bool> option = _3DashUtils.ConfigFile.Bind("Player", "NoPauseSuicide", false);

    public override ConfigEntry<bool> Enabled => option;

    public override string CategoryName => "Player";

    public override string ModuleName => "No Pause Suicide";

    public override string Tooltip => "Prevents you from dying by pressing backspace while paused.";
}

[HarmonyPatch(typeof(PlayerScript), nameof(PlayerScript.Die))]
public class NoPauseSuicidePatch
{
    public static bool Prefix()
    {
        return !(NoPauseSuicide.option.Value && Extensions.GetPauseState());
    }
}