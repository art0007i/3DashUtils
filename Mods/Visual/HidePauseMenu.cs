using _3DashUtils.ModuleSystem;
using BepInEx.Configuration;
using HarmonyLib;
using UnityEngine;

namespace _3DashUtils.Mods.Visual;

public class HidePauseMenu : ToggleModule
{
    public override string CategoryName => "Visual";

    public override string ModuleName => "Hide Pause Menu";

    public override string Description => "Makes the pause menu invisible.";

    protected override bool Default => false;

    internal void SetPauseMenuVisibility(bool visible)
    {
        Object.FindObjectsOfType<PauseMenuManager>().Do(p => p.uiObject.SetActive(visible));
    }

    public override void OnToggle()
    {
        SetPauseMenuVisibility(!Enabled && Extensions.GetPauseState());
    }
}

[HarmonyPatch(typeof(PauseMenuManager), nameof(PauseMenuManager.Pause))]
class HidePauseMenuPatch
{
    public static void Postfix(PauseMenuManager __instance)
    {
        __instance.uiObject.SetActive(!Extensions.Enabled<HidePauseMenu>());
    }
}
