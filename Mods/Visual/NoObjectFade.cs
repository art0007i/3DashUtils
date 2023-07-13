using _3DashUtils.ModuleSystem;
using HarmonyLib;
using UnityEngine;

namespace _3DashUtils.Mods.Visual;

public class NoObjectFade : ToggleModule
{
    public override string ModuleName => "No Object Fade";
    public override string CategoryName => "Visual";

    public override string Description => "Disables the fade in animation for objects.";

    protected override bool Default => false;

    public NoObjectFade()
    {
        var func = AccessTools.Method(typeof(ItemScript), "Start");
        // 6Dash.Patcher Compatibility 
        if(func != null)
        {
            _3DashUtils.Harmony.Patch(func, postfix: new(typeof(NoObjectFadePatch).GetMethod("Postfix")));
        }
    }
}

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
