using _3DashUtils.ModuleSystem;
using BepInEx.Configuration;
using HarmonyLib;
using UnityEngine;

namespace _3DashUtils.Mods.Visual;

public class IgnoreCameraPath : ToggleModule
{
    public static ConfigEntry<bool> option = _3DashUtils.ConfigFile.Bind("Visual", "IgnoreCameraPath", false);
    public override ConfigEntry<bool> Enabled => option;
    public override string CategoryName => "Visual";
    public override string ModuleName => "Ignore Camera Path";
    public override string Tooltip => "Makes the camera ignore the user defined camera animations.\n(Only works on user levels)";

    public override void Update()
    {
        var conf = Enabled.Value;
        Object.FindObjectsOfType<CameraPlayback>().Do(cam =>
        {
            cam.enabled = !conf;
            if (conf) cam.transform.localRotation = Quaternion.identity;
        });
    }
}
