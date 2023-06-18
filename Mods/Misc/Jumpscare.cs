using _3DashUtils.Mods.Player;
using _3DashUtils.ModuleSystem;
using BepInEx.Configuration;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _3DashUtils.Mods.Misc;

public class Jumpscare : TextEditorModule<double>
{
    public static ConfigEntry<bool> option = _3DashUtils.ConfigFile.Bind("Misc", "Jumpscare", false);
    public static ConfigEntry<double> valueOption = _3DashUtils.ConfigFile.Bind("Misc", "JumpscareChance", 0.05);
    public override string CategoryName => "Misc";

    public override string ModuleName => "Jumpscare";

    public override ConfigEntry<bool> Enabled => option;
    public override ConfigEntry<double> Value => valueOption;

    public override string Tooltip => "Jumpscares the player when said player dies, with provided chance by the user.";


    public static void Death()
    {
        //roll rng here and do jumpscare
        var rand = new System.Random();
        if(rand.NextDouble() < valueOption.Value)
        {

        }
    }

    public override bool TryParseText(string text, out double parse)
    {
        return double.TryParse(text, out parse) && parse > 0 && parse <= 1;
    }
}

[HarmonyPatch(typeof(PlayerScript), "Die")]
public static class NoDeathAnimationPatch
{
    public static void Prefix()
    {
        Jumpscare.Death();
    }
}
