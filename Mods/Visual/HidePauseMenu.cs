using _3DashUtils.ModuleSystem;
using BepInEx.Configuration;
using HarmonyLib;
using UnityEngine;

namespace _3DashUtils.Mods.Visual;

public class HidePauseMenu : ToggleModule
{
    public static ConfigEntry<bool> option = _3DashUtils.ConfigFile.Bind("Visual", "HidePauseMenu", false);

    public override ConfigEntry<bool> Enabled => option;

    public override string CategoryName => "Visual";

    public override string ModuleName => "Hide Pause Menu";

    public override string Tooltip => "Makes the pause menu invisible.";

    internal void SetPauseMenuVisibility(bool visible)
    {
        Object.FindObjectsOfType<PauseMenuManager>().Do(p => p.uiObject.SetActive(visible));
    }

    public override void OnToggle()
    {
        SetPauseMenuVisibility(!Enabled.Value && Extensions.GetPauseState());
    }
}

[HarmonyPatch(typeof(PauseMenuManager), nameof(PauseMenuManager.Pause))]
class HidePauseMenuPatch
{
    public static void Postfix(PauseMenuManager __instance)
    {
        __instance.uiObject.SetActive(!HidePauseMenu.option.Value);
    }
}
