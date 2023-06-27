using _3DashUtils.ModuleSystem;
using HarmonyLib;

namespace _3DashUtils.Mods.Player;

public class CheckpointFix : ToggleModule
{
    public override string CategoryName => "Player";

    protected override bool Default => true;

    public override string Description => "Fixes practice checkpoint input buffering and fixes placing checkpoints while dead.";
}

[HarmonyPatch(typeof(PlayerScript), nameof(PlayerScript.ManagePracticeMode))]
public class CheckpointFixPatch
{
    public static bool Prefix(PlayerScript __instance)
    {
        if (Extensions.Enabled<CheckpointFix>() && !__instance.pathFollower.isMoving)
        {
            return false;
        }
        return true;
    }
}
