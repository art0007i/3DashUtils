using _3DashUtils.ModuleSystem;
using BepInEx.Configuration;
using HarmonyLib;
using System.Data.SqlTypes;
using UnityEngine;
using UnityEngine.Rendering;

namespace _3DashUtils.Mods.Player;

public class PracticeMusic : ToggleModule
{
    public static ConfigEntry<bool> option = _3DashUtils.ConfigFile.Bind("Player", "PracticeMusic", false);
    public override ConfigEntry<bool> Enabled => option;

    public override string CategoryName => "Player";

    public override string ModuleName => "Practice Music Hack";

    public override string Tooltip => "Plays the normal song during practice mode.";
}

[HarmonyPatch(typeof(PlayerScript), "Awake")]
public static class PracticeMusicPatch
{
    public static void Postfix(PlayerScript __instance)
    {
        if (PauseMenuManager.inPracticeMode && PracticeMusic.option.Value)
        {
            GameObject recentCheckpoint = PlayerScript.GetRecentCheckpoint();
            if ((bool)recentCheckpoint)
            {
                CheckpointScript cp = recentCheckpoint.GetComponent<CheckpointScript>();
                GameObject.FindGameObjectWithTag("Music").GetComponent<AudioSource>().time = cp.savedMusicPos;
            }
            GameObject gameObject = GameObject.FindGameObjectWithTag("EternalMusic");
            if ((bool)gameObject)
            {
                Object.Destroy(gameObject);
            }
        }
    }
}
[HarmonyPatch(typeof(Playtester), "Start")]
public static class PracticeMusicPatch2
{
    public static void Postfix()
    {
        if (PauseMenuManager.inPracticeMode && PracticeMusic.option.Value)
        {
            GameObject recentCheckpoint = PlayerScript.GetRecentCheckpoint();
            if ((bool)recentCheckpoint)
            {
                CheckpointScript cp = recentCheckpoint.GetComponent<CheckpointScript>();
                GameObject.FindGameObjectsWithTag("Music").Do((go) =>
                {
                    go.GetComponent<AudioSource>().time = cp.savedMusicPos;
                });
            }
            GameObject gameObject = GameObject.FindGameObjectWithTag("EternalMusic");
            if ((bool)gameObject)
            {
                Object.Destroy(gameObject);
            }
        }
    }
}

[HarmonyPatch(typeof(PauseMenuManager), "StartPractice")]
public static class PracticeMusicPatch3
{
    public static bool Prefix(PauseMenuManager __instance)
    {
        if (PracticeMusic.option.Value)
        {
            PauseMenuManager.inPracticeMode = true;
            PauseMenuManager.DestroyAllCheckpoints();
            __instance.Resume();
            return false;
        }
        return true;
    }
}