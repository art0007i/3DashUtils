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
    public override string ModuleName => "No Object Fade";
    public override string CategoryName => "Visual";

    public override string Description => "Disables the fade in animation for objects.";

    protected override bool Default => false;
}

[HarmonyPatch(typeof(ItemScript), "Start")]
public static class NoObjectFadePatch
{
    public static void Postfix(ItemScript __instance)
    {
        if (Extensions.GetModule<NoObjectFade>().Enabled)
        {
            __instance.transform.localScale = new Vector3(1f, 1f, 1f);
        }
    }
}