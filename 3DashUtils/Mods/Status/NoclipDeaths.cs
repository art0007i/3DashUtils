using System;
using System.Collections.Generic;
using System.Numerics;
using System.Security.Permissions;
using _3DashUtils.Mods.Player;
using _3DashUtils.ModuleSystem;
using _3DashUtils.ModuleSystem.Config;
using UnityEngine;
using UnityEngine.SceneManagement;
using Vector3 = UnityEngine.Vector3;

namespace _3DashUtils.Mods.Status;
public class NoclipDeaths : TemplateLabel
{
    public override string CategoryName => "Status";
    public override string ModuleName => "Noclip Deaths";
    protected override bool Default => false;
    public override string text => $"Noclip Deaths: {deaths}";

    private PlayerScript player;
    private bool playerPreviousSafe = true;
    private bool playerSafe = true;
    private Noclip noclip;
    private int deaths = 0;

    public static ConfigOptionBase<bool> preserveAcrossAttempts;
    public static bool PreserveAcrossAttempts => preserveAcrossAttempts.Value;

    public override void Start()
    {
        base.Start();
        noclip = Extensions.GetModule<Noclip>();
        SceneManager.activeSceneChanged += SceneChanged;

        preserveAcrossAttempts = new ToggleConfigOption(this, "Preserve across attempts", false, "Preserve Noclip Deaths across attempts");
    }

    private void SceneChanged(Scene arg0, Scene arg1)
    {
        if (!PreserveAcrossAttempts) deaths = 0;
    }

    public override void LateUpdate()
    {
        base.Update();
        if(Enabled && noclip.Enabled)
        {
            playerPreviousSafe = playerSafe;
            playerSafe = noclip.IsPlayerSafe();
            if (playerPreviousSafe && !playerSafe)
            {
                deaths++;
            }
        }
    }
}
