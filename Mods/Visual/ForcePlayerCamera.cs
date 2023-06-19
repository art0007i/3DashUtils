using _3DashUtils.ModuleSystem;
using BepInEx.Configuration;
using HarmonyLib;
using System.Reflection.Emit;
using UnityEngine;

namespace _3DashUtils.Mods.Visual;

public class ForcePlayerCamera : ToggleModule
{
    public override string CategoryName => "Visual";
    public override string ModuleName => "Force Player Camera";
    public override string Description => "Locks the camera to be in front of the player no matter what.";

    public override bool IsCheat => Enabled;

    protected override bool Default => false;

    public override void Update()
    {
        var conf = Enabled;
        if (conf)
        {
            var cam = GameObject.Find("Boom Arm");
            if(cam != null)
            {
                var player = GameObject.Find("ActualPlayer").transform.localRotation;
                player.ToAngleAxis(out var angle, out var axis);
                cam.transform.localRotation = Quaternion.AngleAxis(angle + 90, axis);
                cam.GetComponent<Animator>().enabled = false;
            }
        }
        Object.FindObjectsOfType<CameraPlayback>().Do(cam =>
        {
            cam.enabled = !conf;
        });
    }
}
