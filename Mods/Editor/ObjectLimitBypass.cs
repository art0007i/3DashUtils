using _3DashUtils.ModuleSystem;
using BepInEx.Configuration;
using HarmonyLib;
using UnityEngine;

namespace _3DashUtils.Mods.Editor;

public class ObjectLimitBypass : ToggleModule
{
    public override string CategoryName => "Editor";

    public override string ModuleName => "Object Limit Bypass";

    public override string Description => "Increases the object limit to 2147483647.";

    protected override bool Default => true;
}

[HarmonyPatch(typeof(FlatEditor), "Update")]
public class ObjectLimitPatch
{
    private static void Postfix(ref int ___objectLimit)
    {
        if (Extensions.Enabled<ObjectLimitBypass>())
        {
            ___objectLimit = int.MaxValue;
        }
        else
        {
            ___objectLimit = 4000;
        }
    }
}
