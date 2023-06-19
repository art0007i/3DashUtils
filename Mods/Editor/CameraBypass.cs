using _3DashUtils.ModuleSystem;
using BepInEx.Configuration;
using HarmonyLib;
using UnityEngine;

namespace _3DashUtils.Mods.Editor;

public class CameraBypass : ToggleModule
{
    public override string CategoryName => "Editor";

    public override string ModuleName => "Camera Bypass";


    public override string Description => "Allows you to move the editor camera outside of the normal area.";

    protected override bool Default => false;
}

[HarmonyPatch(typeof(FlatCamera), "ClampPosition")]
public class CameraBypassPatch
{
    private static bool Prefix()
    {
        return !Extensions.Enabled<CameraBypass>();
    }
}