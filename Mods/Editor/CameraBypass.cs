using _3DashUtils.ModuleSystem;
using BepInEx.Configuration;
using HarmonyLib;
using UnityEngine;

namespace _3DashUtils.Mods.Editor;

public class CameraBypass : ToggleModule
{
    public static ConfigEntry<bool> option = _3DashUtils.ConfigFile.Bind("Editor", "CameraBypass", false);
    public override string CategoryName => "Editor";

    public override string ModuleName => "Camera Bypass";

    public override ConfigEntry<bool> Enabled => option;

    public override string Tooltip => "Allows you to move the editor camera outside of the normal area.";
}

[HarmonyPatch(typeof(FlatCamera), "ClampPosition")]
public class CameraBypassPatch
{
    private static bool Prefix()
    {
        return !CameraBypass.option.Value;
    }
}