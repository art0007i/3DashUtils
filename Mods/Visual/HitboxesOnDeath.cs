﻿using _3DashUtils.ModuleSystem;
using BepInEx.Configuration;
using HarmonyLib;
using UnityEngine.SceneManagement;

namespace _3DashUtils.Mods.Visual;

internal class HitboxesOnDeath : ToggleModule
{
    public static ConfigEntry<bool> option = _3DashUtils.ConfigFile.Bind("Visual", "HitboxesOnDeath", false);
    public override ConfigEntry<bool> Enabled => option;

    public override string CategoryName => "Visual";

    public override string ModuleName => "Hitboxes On Death";

    public override string Tooltip => "Shows hitboxes of all interactable objects on player death.";

    public override void Update()
    {
        //if (ShowHitboxes.option.Value || !option.Value) return;
        var player = UnityEngine.Object.FindObjectOfType<PlayerScript>();
        if(player != null && player.dead)
        {
            ShowHitboxes.RenderHitboxes();
        }
    }
}
