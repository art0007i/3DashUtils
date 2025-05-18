using System;
using System.Security.Permissions;
using _3DashUtils.ModuleSystem;
using _3DashUtils.ModuleSystem.Config;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using UnityEngine.SceneManagement;

namespace _3DashUtils.Mods.Status;
public class Attempts : TemplateLabel
{
    public override string CategoryName => "Status";

    public override string ModuleName => "Attempts";

    public override string Description => "Shows an Attempts label in the top left corner";

    protected override bool Default => false;

    public override string text { get; set; } = "";

    private int attempts = 0;

    private float debounce;

    private PlayerScript player;

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
        debounce = Math.Max(0, debounce - 1);
        if (player && player.dead && debounce < 1)
        {
            debounce = 200;
            attempts++;
        }
    }

    public override void UpdateText()
    {
        text = $"Attempts: {attempts}";
    }
}

