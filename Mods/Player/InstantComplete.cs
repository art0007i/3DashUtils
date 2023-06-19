using _3DashUtils.ModuleSystem;
using BepInEx.Configuration;
using UnityEngine;

namespace _3DashUtils.Mods.Player;

public class InstantComplete : ToggleModule
{
    public static ConfigEntry<bool> option = _3DashUtils.ConfigFile.Bind("Player", "InstantComplete", false);
    public override string CategoryName => "Player";

    public override string ModuleName => "Instant Complete";
    public override bool IsCheat => Enabled;

    public override string Description => "Automatically wins any level you enter.";

    protected override bool Default => false;

    public override void Update()
    {
        if (Enabled)
        {
            Object.FindObjectOfType<PlayerScript>()?.Win();
        }
    }
}
