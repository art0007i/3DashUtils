using _3DashUtils.ModuleSystem;
using BepInEx.Configuration;
using HarmonyLib;
using UnityEngine;

namespace _3DashUtils.Mods.Player;

internal class SpeedHackAudio : ToggleModule
{
    public static ConfigEntry<bool> option = _3DashUtils.ConfigFile.Bind("Player", "SpeedHackAudio", false);
    public override float Priority => 0.1f;
    public override string CategoryName => "Player";
    public override string ModuleName => "SpeedHack Audio";
    public override string Tooltip => "Makes SpeedHack affect audio sources.";

    public override ConfigEntry<bool> Enabled => option;

    public override void Update()
    {
        var changePitch = Enabled.Value && SpeedHack.enabledOption.Value;
        Object.FindObjectsOfType<AudioSource>().Do(src => src.pitch = changePitch ? SpeedHack.valueOption.Value : 1f);
    }
}
