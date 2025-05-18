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
    public override string Description => "Shows a Noclip Deaths label in the top left corner";
    protected override bool Default => false;
    public override string text { get; set; } = "";

    private PlayerScript player;
    private bool playerPreviousSafe = true;
    private bool playerSafe = true;
    private Noclip noclip;
    private int deaths = 0;

    public override void Start()
    {
        base.Start();
        noclip = Extensions.GetModule<Noclip>();
        SceneManager.activeSceneChanged += SceneChanged;
    }

    private void SceneChanged(Scene arg0, Scene arg1)
    {
        deaths = 0;
    }

    public override void Update()
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

    public override void UpdateText()
    {
        text = $"Noclip Deaths: {deaths}";
    }
}
