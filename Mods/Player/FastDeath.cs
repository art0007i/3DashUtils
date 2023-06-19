using _3DashUtils.ModuleSystem;
using _3DashUtils.ModuleSystem.Config;
using BepInEx.Configuration;
using HarmonyLib;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _3DashUtils.Mods.Player;

public class FastDeath : ToggleModule
{
    public override string ModuleName => "Fast Death";
    public override string CategoryName => "Player";
    public override string Description => "Changes the time to respawn the player after dying.";
    protected override bool Default => false;

    public static float Time;
    private static ConfigOptionBase<float> timeOption;
    public FastDeath()
    {
        timeOption = new TextInputConfig<float>(this, "Time", 0, "The time that it takes the player to respawn. (1.2 is the base game default)", TryParseTime);
    }
    public static bool TryParseTime(string text, out float value)
    {
        return float.TryParse(text, out value) && value >= 0;
    }
}

[HarmonyPatch(typeof(DeathScript), "Update")]
public static class NoDeathAnimationPatch
{
    public static bool Prefix(DeathScript __instance)
    {
        var e = Extensions.Enabled<FastDeath>();
        if (e)
        {
            var t = Traverse.Create(__instance).Field("timePassed");
            var n = t.GetValue<float>() + Time.deltaTime;
            t.SetValue(n);
            if(n > FastDeath.Time) SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        return !e;
    }
}