using _3DashUtils.ModuleSystem;
using _3DashUtils.ModuleSystem.Config;
using HarmonyLib;
using UnityEngine;

namespace _3DashUtils.Mods.Player;

public class SpeedHack : ToggleModule
{
    public override string CategoryName => "Player";

    public override float Priority => 0.11f;

    public static float Speed => speedConfig.Value;
    public static ConfigOptionBase<float> speedConfig;
    public static bool SpeedHackAudio => speedHackAudio.Value;
    public static ConfigOptionBase<bool> speedHackAudio;

    public override string Description => "Changes the speed of the game.";

    public override bool IsCheat => Enabled && Speed < 1;

    protected override bool Default => false;

    public SpeedHack()
    {
        speedConfig = new TextInputConfig<float>(this, "Speed", 1, "The speed that the game will play at.", (v) => v > 0);
        speedHackAudio = new ToggleConfigOption(this, "Affect Audio", true, "Determines if audio should be affected by SpeedHack.");
    }

    public override void Update()
    {
        if (!Extensions.GetPauseState())
        {
            Time.timeScale = Enabled ? Speed : 1f;
        }
        var changePitch = SpeedHackAudio && Enabled;
        Object.FindObjectsOfType<AudioSource>().Do(src => src.pitch = changePitch ? Speed : 1f);
    }
}
