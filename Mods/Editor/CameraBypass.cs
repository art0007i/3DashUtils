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

    public override void OnGUI()
    {
        base.OnGUI();
    }
}

[HarmonyPatch(typeof(FlatCamera), "Update")]
public class CameraBypassPatch
{
    private static void Postfix(ref float ___rightBound, ref float ___leftBound, ref float ___topBound, ref float ___bottomBound)
    {
        if (CameraBypass.option.Value)
        {
            ___rightBound = 2100f;
            ___leftBound = -100f;
            ___topBound = 100f;
            ___bottomBound = -100f;
        }
        else
        {
            ___rightBound = 2000f;
            ___leftBound = -1f;
            ___topBound = 30f;
            ___bottomBound = -11f;
        }
    }
}