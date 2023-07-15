using _3DashUtils.Compat;
using _3DashUtils.ModuleSystem;
using HarmonyLib;
using UnityEngine;

namespace _3DashUtils.Mods.Hidden;

/// <summary>
/// This class handles saving the game settings (show path and volume) to bepinex config so that they persist bewteen restarts.
/// </summary>
public class SettingsPersistence : ModuleBase
{
    public static ConfigWrapper<float> volume = new("GameSettings", "Volume", 1f);
    public static ConfigWrapper<bool> showPath = new("GameSettings", "ShowPath", false);

    public override string CategoryName => "Hidden";

    public override void Awake()
    {
        PauseMenuManager.pathOn = showPath.Value;
        AudioListener.volume = volume.Value;
    }
}

[HarmonyPatch(typeof(PauseMenuManager), "Update")]
public static class PauseMenuManager_Update_Patch
{
    private static void Postfix(PauseMenuManager __instance)
    {
        SettingsPersistence.showPath.Value = __instance.pathToggle.isOn;
        SettingsPersistence.volume.Value = __instance.volumeSlider.value;
    }
}
