using _3DashUtils.ModuleSystem;
using BepInEx.Configuration;
using HarmonyLib;
using UnityEngine;

namespace _3DashUtils.Mods.Editor;

public class ObjectLimitBypass : ToggleModule
{
    public static ConfigEntry<bool> objectLimitBypass = _3DashUtils.ConfigFile.Bind("Editor", "ObjectLimitBypass", false);
    public override string CategoryName => "Editor";

    public override string ModuleName => "Object Limit Bypass";

    public override ConfigEntry<bool> Enabled => objectLimitBypass;

    public override string Tooltip => "Increases the object limit to 2147483647.";
}

[HarmonyPatch(typeof(FlatEditor), "Update")]
public class ObjectLimitPatch
{
    private static void Postfix(ref int ___objectLimit)
    {
        if (ObjectLimitBypass.objectLimitBypass.Value)
        {
            ___objectLimit = int.MaxValue;
        }
        else
        {
            ___objectLimit = 4000;
        }
    }
}
