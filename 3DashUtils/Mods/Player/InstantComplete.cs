using _3DashUtils.ModuleSystem;
using UnityEngine;

namespace _3DashUtils.Mods.Player;

public class InstantComplete : ToggleModule
{
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
