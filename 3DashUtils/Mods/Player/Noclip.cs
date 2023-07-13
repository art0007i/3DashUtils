using _3DashUtils.ModuleSystem;
using HarmonyLib;
using System.Linq;
using UnityEngine;

namespace _3DashUtils.Mods.Player;

public class Noclip : ToggleModule
{
    public override string CategoryName => "Player";

    public override string Description => "Prevents the player from dying.";

    public override bool IsCheat => Enabled;

    protected override bool Default => false;

    public override void Update()
    {
        if (Enabled)
        {
            GameObject.FindGameObjectsWithTag("Hazard").SelectMany(go => go.GetComponents<Collider>()).Do(coll => coll.enabled = false);
        }
    }
    public override void OnToggle()
    {
        if (Enabled == false)
        {
            GameObject.FindGameObjectsWithTag("Hazard").Do(go =>
            {
                go.GetComponentsInChildren<Collider>().Do(coll => coll.enabled = true);
            });
        }
    }
}

[HarmonyPatch(typeof(PlayerScript), nameof(PlayerScript.Die))]
public class NoClipPatch
{
    public static bool Prefix(bool deathOverride)
    {
        return deathOverride || !Extensions.Enabled<Noclip>();
    }
}
