using _3DashUtils.ModuleSystem;
using HarmonyLib;
using UnityEngine;

namespace _3DashUtils.Mods.Player;

public class PracticeMusic : ToggleModule
{
    public override string CategoryName => "Player";

    public override string ModuleName => "Practice Music Hack";

    public override string Description => "Plays the normal song during practice mode.";

    protected override bool Default => true;
}

[HarmonyPatch(typeof(PlayerScript), "Awake")]
public static class PracticeMusicPatch
{
    public static void Postfix(PlayerScript __instance)
    {
        if (PauseMenuManager.inPracticeMode && Extensions.Enabled<PracticeMusic>())
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
        if (PauseMenuManager.inPracticeMode && Extensions.Enabled<PracticeMusic>())
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
        if (Extensions.Enabled<PracticeMusic>())
        {
            PauseMenuManager.inPracticeMode = true;
            PauseMenuManager.DestroyAllCheckpoints();
            __instance.Resume();
            return false;
        }
        return true;
    }
}