using _3DashUtils.ModuleSystem;
using HarmonyLib;

namespace _3DashUtils.Mods.Player;

public class KeepCheckpoints : ToggleModule
{
    public override string CategoryName => "Player";

    public override string ModuleName => "Keep Checkpoints";

    public override string Description => "Prevents checkpoints resetting when you exit practice mode. Useful while editing levels.";

    protected override bool Default => false;
}

[HarmonyPatch(typeof(PauseMenuManager), nameof(PauseMenuManager.DestroyAllCheckpoints))]
public static class KeepCheckpointsPatch
{
    public static bool Prefix()
    {
        return !Extensions.Enabled<KeepCheckpoints>();
    }
}