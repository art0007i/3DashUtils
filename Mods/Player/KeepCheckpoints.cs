using _3DashUtils.ModuleSystem;
using BepInEx.Configuration;
using HarmonyLib;

namespace _3DashUtils.Mods.Player;

public class KeepCheckpoints : ToggleModule
{
    public static ConfigEntry<bool> option = _3DashUtils.ConfigFile.Bind("Player", "KeepCheckpoints", false);
    public override ConfigEntry<bool> Enabled => option;

    public override string CategoryName => "Player";

    public override string ModuleName => "Keep Checkpoints";

    public override string Tooltip => "Prevents checkpoints resetting when you exit practice mode. Useful while editing levels.";
}

[HarmonyPatch(typeof(PauseMenuManager), nameof(PauseMenuManager.DestroyAllCheckpoints))]
public static class KeepCheckpointsPatch
{
    public static bool Prefix()
    {
        return !KeepCheckpoints.option.Value;
    }
}