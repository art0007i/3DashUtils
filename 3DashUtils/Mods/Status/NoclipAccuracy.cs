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
public class NoclipAccuracy : TemplateLabel
{
    public override string CategoryName => "Status";
    public override string ModuleName => "Noclip Accuracy";
    public override string Description => "Shows a Noclip Accuracy label in the top left corner";
    protected override bool Default => false;
    public override string text { get; set; } = "";

    private PlayerScript player;
    private Noclip noclip;
    private int deaths = 0;
    private int frames = 0;

    public override void Start()
    {
        base.Start();
        noclip = Extensions.GetModule<Noclip>();
        SceneManager.activeSceneChanged += SceneChanged;
    }

    private void SceneChanged(Scene arg0, Scene arg1)
    {
        deaths = 0;
        frames = 0;
    }

    public override void Update()
    {
        if(noclip.Enabled && !noclip.IsPlayerSafe()) deaths++;
        frames++;
    }

    public override void UpdateText()
    {
        text = $"Noclip Accuracy: {CalculateAccuracy()}%";
    }

    private int CalculateAccuracy()
    {
        if (frames == 0) return 100; 
        if (deaths == 0) return 100;

        float accuracy = ((float)(frames - deaths) / frames) * 100;
        return Mathf.Clamp(Mathf.RoundToInt(accuracy), 0, 100);
    }
}
