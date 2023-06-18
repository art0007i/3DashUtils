using _3DashUtils.ModuleSystem;
using BepInEx.Configuration;
using UnityEngine;

namespace _3DashUtils.Mods.Player;

public class InstantComplete : ToggleModule
{
    public static ConfigEntry<bool> option = _3DashUtils.ConfigFile.Bind("Player", "InstantComplete", false);
    public override string CategoryName => "Player";

    public override string ModuleName => "Instant Complete";

    public override ConfigEntry<bool> Enabled => option;

    public override bool IsCheat => Enabled.Value;

    public override string Tooltip => "Automatically wins any level you enter.";

    public override void Update()
    {
        if (Enabled.Value)
        {
            Object.FindObjectOfType<PlayerScript>()?.Win();
        }
    }
}
