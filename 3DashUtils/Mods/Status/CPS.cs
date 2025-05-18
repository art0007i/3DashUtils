using System;
using System.Collections.Generic;
using System.Security.Permissions;
using _3DashUtils.ModuleSystem;
using _3DashUtils.ModuleSystem.Config;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _3DashUtils.Mods.Status;
public class CPS : TemplateLabel
{
    public override string CategoryName => "Status";
    public override string ModuleName => "CPS";
    public override string Description => "Shows a CPS label in the top left corner";
    protected override bool Default => false;
    public override string text { get; set; } = "";

    private Queue<float> clickTimes = new Queue<float>();

    public override void Start()
    {
        base.Start();
        SceneManager.activeSceneChanged += SceneChanged;
    }

    private void SceneChanged(Scene arg0, Scene arg1)
    {
        clickTimes.Clear();
    }

    public override void UpdateText()
    {
        // Register new clicks
        if (Input.GetMouseButtonDown(0))
        {
            clickTimes.Enqueue(Time.time);
        }

        // Remove old clicks (older than 1 second)
        while (clickTimes.Count > 0 && Time.time - clickTimes.Peek() > 1f)
        {
            clickTimes.Dequeue();
        }

        // Calculate adjusted CPS with division by 4
        int rawCPS = clickTimes.Count;
        int adjustedCPS = rawCPS / 4;  // Integer division

        // Update display text
        text = $"CPS: {adjustedCPS}";
    }
}
