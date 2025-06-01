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
    protected override bool Default => false;
    public override string text => $"CPS: {clickTimes.Count}";

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

    public override void Update()
    {
        // Register new clicks
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            clickTimes.Enqueue(Time.time);
        }

        // Remove old clicks (older than 1 second)
        while (clickTimes.Count > 0 && Time.time - clickTimes.Peek() > 1f)
        {
            clickTimes.Dequeue();
        }
    }
}
