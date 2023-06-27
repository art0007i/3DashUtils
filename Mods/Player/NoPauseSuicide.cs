using _3DashUtils.ModuleSystem;
using HarmonyLib;

namespace _3DashUtils.Mods.Player;

public class NoPauseSuicide : ToggleModule
{
    public override string CategoryName => "Player";

    public override string ModuleName => "No Pause Suicide";

    public override string Description => "Prevents you from dying by pressing backspace while paused.";

    protected override bool Default => true;
}

[HarmonyPatch(typeof(PlayerScript), nameof(PlayerScript.Die))]
public class NoPauseSuicidePatch
{
    public static bool Prefix()
    {
        return !(Extensions.Enabled<NoPauseSuicide>() && Extensions.GetPauseState());
    }
}
