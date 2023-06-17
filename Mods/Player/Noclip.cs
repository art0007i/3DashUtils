using _3DashUtils.ModuleSystem;
using BepInEx.Configuration;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace _3DashUtils.Mods.Player;

public class Noclip : ToggleModule
{
    public static ConfigEntry<bool> noclip = _3DashUtils.ConfigFile.Bind("Player", "Noclip", false);
    public override string CategoryName => "Player";

    public override ConfigEntry<bool> Enabled => noclip;

    public override string Tooltip => "Prevents the player from dying.";

    public override void Update()
    {
        if (noclip.Value)
        {
            GameObject.FindGameObjectsWithTag("Hazard").Do(go =>
            {
                go.GetComponentsInChildren<Collider>().Do(coll => coll.enabled = false);
            });
        }
    }
    public override void OnToggle()
    {
        if (Enabled.Value == false)
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
        return deathOverride || !Noclip.noclip.Value;
    }
}
