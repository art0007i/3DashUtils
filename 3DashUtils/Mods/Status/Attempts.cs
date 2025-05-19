using System;
using System.Security.Permissions;
using _3DashUtils.ModuleSystem;
using _3DashUtils.ModuleSystem.Config;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using UnityEngine.SceneManagement;
using _3DashUtils.Mods.Player;
using TMPro;

namespace _3DashUtils.Mods.Status;
public class Attempts : TemplateLabel
{
    public override string CategoryName => "Status";

    public override string ModuleName => "Attempts";

    protected override bool Default => false;

    public override string text { get; set; } = "";

    private int attempts = 1;

    private float debounce;

    private PlayerScript player;

    private bool previousDead = false;
    private bool currentDead = false;

    public override void Start()
    {
        base.Start();
        SceneManager.activeSceneChanged += SceneChanged;
        player = UnityEngine.Object.FindObjectOfType<PlayerScript>();
    }

    public void SceneChanged(Scene current, Scene next)
    {
        player = UnityEngine.Object.FindObjectOfType<PlayerScript>();
        if(!player) attempts = 0;
    }

    public override void Update()
    {
        previousDead = currentDead;
        currentDead = player && player.dead;
        if(!previousDead && currentDead)
        {
            attempts++;
        }
    }

    public override void UpdateText()
    {
        text = $"Attempts: {attempts}";
    }
}

