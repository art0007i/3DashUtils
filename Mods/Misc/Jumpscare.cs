using _3DashUtils.Mods.Player;
using _3DashUtils.ModuleSystem;
using BepInEx.Configuration;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace _3DashUtils.Mods.Misc;

internal class Jumpscare : ToggleModule
{
    public static ConfigEntry<bool> option = _3DashUtils.ConfigFile.Bind("Misc", "Jumpscare", false);
    public override string CategoryName => "Misc";

    public override string ModuleName => "Jumpscare";

    public override ConfigEntry<bool> Enabled => option;

    public override string Tooltip => "Jumpscares the player when said player dies, with provided chance by the user.";

    public int chance;

    public override void OnGUI()
    {
        chance = ((int)UnityEngine.GUILayout.HorizontalSlider(5, 0, 100)); //chance slider
    }
}

[HarmonyPatch(typeof(PlayerScript), "Die")]
public static class NoDeathAnimationPatch
{
    public static void Prefix()
    {
        var rand = new Random();
        rand.Next(((int)OnGUI.chance), 100);
    }
}
