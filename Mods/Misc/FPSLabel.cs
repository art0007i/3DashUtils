using _3DashUtils.ModuleSystem;
using UnityEngine;

namespace _3DashUtils.Mods.Misc;
public class FPSLabel : ModuleBase
{
    public override string CategoryName => "Misc";

    public override float Priority => -1;

    public override void OnGUI()
    {
        float f = 1f / Time.deltaTime;
        GUILayout.Label($"FPS: {Mathf.Round(f)}");
    }
}

