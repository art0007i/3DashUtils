using _3DashUtils.Mods.Player;
using _3DashUtils.ModuleSystem;
using _3DashUtils.ModuleSystem.Config;
using System;
using UnityEngine;

namespace _3DashUtils.Mods.Misc;
public class TargetFPS : ToggleModule
{
    public override string CategoryName => "Misc";

    public override string ModuleName => "Target FPS";

    public override float Priority => -0.9f;


    public static ConfigOptionBase<int> valueConfig;
    public static int Value => valueConfig.Value;


    public static ConfigOptionBase<bool> lockDelta;
    public static bool LockDelta => lockDelta.Value;

    public override string Description => "Allows you to select a framerate that the game will run at.";

    protected override bool Default => false;

    public TargetFPS()
    {
        valueConfig = new TextInputConfig<int>(this, "FPS", 60, "The FPS value that the game will lock to.", (v) => v > 0);
        lockDelta = new ToggleConfigOption(this, "Lock Delta", false, "Locks the deltaTime of the game to make things more consistent.\nMake sure to use this when recording and playing replays.");
    }

    public override void Update()
    {
        var mult = LockDelta ? Time.timeScale : 1;
        Application.targetFrameRate = Enabled ? (int)(Value * mult) : -1;
        var f = Math.Max((int)(Value * mult), 1);
        Time.captureFramerate = Enabled && LockDelta ? f : 0;
    }
}
