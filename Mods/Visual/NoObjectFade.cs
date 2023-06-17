using _3DashUtils.ModuleSystem;
using BepInEx.Configuration;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace _3DashUtils.Mods.Visual;

public class NoObjectFade : ToggleModule
{
    public static ConfigEntry<bool> option = _3DashUtils.ConfigFile.Bind("Visual", "NoObjectFade", false);
    public override ConfigEntry<bool> Enabled => option;

    public override string ModuleName => "No Object Fade";
    public override string CategoryName => "Visual";

    public override string Tooltip => "Disables the fade in animation for objects.";
}

[HarmonyPatch(typeof(ItemScript), "Start")]
public static class NoObjectFadePatch
{
    public static void Postfix(ItemScript __instance)
    {
        if (NoObjectFade.option.Value)
        {
            __instance.transform.localScale = new Vector3(1f, 1f, 1f);
        }
    }
}