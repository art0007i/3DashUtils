using _3DashUtils.ModuleSystem;
using BepInEx.Configuration;
using HarmonyLib;
using UnityEngine.SceneManagement;

namespace _3DashUtils.Mods.Player;

public class NoDeathAnimation : ToggleModule
{
    public static ConfigEntry<bool> option = _3DashUtils.ConfigFile.Bind("Player", "NoDeathAnimation", false);
    public override ConfigEntry<bool> Enabled => option;

    public override string ModuleName => "No Death Animation";
    public override string CategoryName => "Player";

    public override string Tooltip => "Instantly respawn the player after dying.";
}

[HarmonyPatch(typeof(DeathScript), "Update")]
public static class NoDeathAnimationPatch
{
    public static void Prefix()
    {
        if (NoDeathAnimation.option.Value)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}