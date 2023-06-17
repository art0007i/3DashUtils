using _3DashUtils.ModuleSystem;
using HarmonyLib;
using UnityEngine;

namespace _3DashUtils.Mods.Hidden;

public class VolumeOverride : ModuleBase
{
    public override string CategoryName => "Hidden";

    public override void Update()
    {
        // this just fixes up the audio prefs to work on all audio instead of just music...
        Object.FindObjectsOfType<AudioSource>().Do(src => src.volume = _3DashUtils.volume.Value);
    }
}
