using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using _3DashUtils.ModuleSystem;
using BepInEx.Configuration;
using HarmonyLib;
using UnityEngine;

namespace _3DashUtils.Mods.Replays;

public enum ReplayMode
{
    Recording,
    Replaying
}
public class ReplayModule : ToggleModule
{
    public override string CategoryName => "Replays";

    public override string ModuleName => "Replay System";

    public override string Description => "Allows you to record and play replay files";

    // these 2 are for recording
    static public double lastOffset;
    static public bool lastClick = false;

    // these 2 are for playback
    public static int lastKframe = -1;
    public static bool shouldClick;

    static public ClickReplay testReplay = new();

    public static double CurrentTime => lastOffset + Time.timeSinceLevelLoadAsDouble;

    public static ReplayMode Mode { get => modeConfig.Value; set { modeConfig.Value = value; } }

    protected override bool Default => false;

    public static ConfigEntry<ReplayMode> modeConfig = _3DashUtils.ConfigFile.Bind("Replays", "Mode", ReplayMode.Recording, "Current mode of the ReplayModule.");


    string path = Path.Combine(Extensions.GetPluginDataPath(), "Replays", "testMacro.3dr"); //3dash replay (.3dr)

    public override void Awake()
    {
        Directory.CreateDirectory(Path.Combine(Extensions.GetPluginDataPath(), "Replays"));
    }

    public override void OnGUI()
    {
        base.OnGUI();
        //var w = GUILayoutUtility.GetLastRect().width /2;
        var SelectedButtonStyle = new GUIStyle(GUI.skin.button);
        Mode = (ReplayMode)GUILayout.Toolbar((int)Mode, new string[] { "Record", "Replay" });

        if (GUILayout.Button("Save Replay"))
        {
            File.WriteAllText(path, ReplayConverter.ReplayToString(testReplay));
        }
        if (GUILayout.Button("Load Replay"))
        {
            testReplay = ReplayConverter.StringToReplay(File.ReadAllText(path));
        }
    }


    public static void RemoveFutureClicks()
    {
        _3DashUtils.Log.LogMessage("removeing everything after " + CurrentTime);
        for (int i = testReplay.Count - 1; i >= 0; i--)
        {
            if(i >= CurrentTime)
            {
                testReplay.RemoveAt(i);
            }
        }
    }


    /*
        this is how it should look:
        (totally not stolen from megahack)
        ---------------------------
        | textbox  (filename)     |
        ---------------------------
        -------------  ------------
        |  record   |  |   replay |
        -------------  ------------

        click at 123ms, release at 256ms    
         \/
        +0.123
        -0.256
        any line that doesn't start with a number is a comment (this one is)

    */
}

[HarmonyPatch(typeof(CameraRaiser), "Start")]
class Fuckpatch
{
    public static void Prefix()
    {
        // this code is just to patch any unity start method.
        if (ReplayModule.Mode == ReplayMode.Recording)
        {
            ReplayModule.RemoveFutureClicks();
        }

    }
}

[HarmonyPatch(typeof(PlayerScript))]
class ReplayModulePatch
{
    public static MethodInfo lookFor = AccessTools.FirstMethod(
        typeof(GameObject), 
        (m) => m.Name == "GetComponent" && 
        m.IsGenericMethod).MakeGenericMethod(new Type[] {
            typeof(CheckpointScript)
        });

    public static MethodInfo injectFunc = typeof(ReplayModulePatch).GetMethod("Injection");
    public static CheckpointScript Injection(CheckpointScript script)
    {
        var add = script.gameObject.AddComponent<CheckpointAddon>();
        add.savedSceneTime = Time.timeSinceLevelLoadAsDouble;
        add.lastClick = ReplayModule.lastClick;
        // doing this so i dont need an extra il for dup cuz i hate transpiler lol
        return script;
    }

    [HarmonyTranspiler]
    [HarmonyPatch(nameof(PlayerScriptEditor.MakeCheckpoint))]
    public static IEnumerable<CodeInstruction> CPTranspiler(IEnumerable<CodeInstruction> codes)
    {
        foreach (var code in codes)
        {
            yield return code;
            if(code.operand is MethodInfo mi && mi == lookFor)
            {
                yield return new(OpCodes.Call, injectFunc);
            }
        }
    }
    [HarmonyPostfix]
    [HarmonyPatch("Awake")]
    public static void AwakePostfix(PlayerScript __instance)
    {
        ReplayModule.shouldClick = false;
        ReplayModule.lastClick = false;
        ReplayModule.lastKframe = -1;
        ReplayModule.lastOffset = 0;
        if (PauseMenuManager.inPracticeMode)
        {
            GameObject recentCheckpoint = PlayerScript.GetRecentCheckpoint();
            if (recentCheckpoint)
            {
                var a = recentCheckpoint.GetComponent<CheckpointAddon>();
                ReplayModule.lastOffset = a.savedSceneTime;
                ReplayModule.lastClick = a.lastClick;
            }
        }
    }



    [HarmonyPrefix]
    [HarmonyPatch("Update")]
    public static void UpdatePrefix(PlayerScriptEditor __instance)
    {
        if(Extensions.Enabled<ReplayModule>() && ReplayModule.Mode == ReplayMode.Replaying)
        {
            var i = ReplayModule.testReplay.FindLastIndex((l) => l.time <= ReplayModule.CurrentTime);
            if (i >= 0)
            {
                var currentKframe = ReplayModule.testReplay[i];
                var shouldClick = currentKframe.click;
                var last = ReplayModule.lastKframe;

                if (last != i)
                {
                    var count = i - last;
                    // check if we skipped any click frames. if so click them
                    if (count > 1 && ReplayModule.testReplay.GetRange(ReplayModule.lastKframe, count).Any((k) => k.click = true))
                    {
                        shouldClick = true;
                    }
                    ReplayModule.lastKframe = i;
                    __instance.jumpInputPressed = shouldClick;
                    
                }
                ReplayModule.shouldClick = shouldClick;
                //_3DashUtils.Log.LogMessage($"ct: {ReplayModule.CurrentTime}");
                //_3DashUtils.Log.LogMessage($"t: {currentKframe.time}, c: {currentKframe.click}");
            }
        }
    }

    public static bool InputCheck()
    {
        return Extensions.Enabled<ReplayModule>() && ReplayModule.shouldClick;
    }

    [HarmonyTranspiler]
    [HarmonyPatch("Update")]
    public static IEnumerable<CodeInstruction> UpdateTranspiler(IEnumerable<CodeInstruction> codes)
    {
        foreach (var code in codes)
        {
            yield return code;
            if (code.Calls(typeof(Input).GetMethod(nameof(Input.GetMouseButton))))
            {
                yield return new(OpCodes.Call, typeof(ReplayModulePatch).GetMethod(nameof(InputCheck)));
                yield return new(OpCodes.Or);
            }
        }
    }

    [HarmonyPostfix]
    [HarmonyPatch("Update")]
    public static void UpdatePostfix(PlayerScriptEditor __instance)
    {
        if (Extensions.Enabled<ReplayModule>() && ReplayModule.Mode == ReplayMode.Recording)
        {
            var player = GameObject.FindObjectOfType<PlayerScript>();
            if (player == null) return;
            var click = player.jumpInput;
            if (ReplayModule.lastClick != click)
            {
                _3DashUtils.Log.LogMessage("adding click " + click + " at " + ReplayModule.CurrentTime);
                ReplayModule.testReplay.Add(ReplayModule.CurrentTime, click);
            }
            ReplayModule.lastClick = click;
        }
    }
}

[HarmonyPatch(typeof(PlayerScriptEditor))]
class ReplayModulePatch2
{
    // what a good fucking coder that duplicated all the code for player script
    [HarmonyTranspiler]
    [HarmonyPatch(nameof(PlayerScriptEditor.MakeCheckpoint))]
    public static IEnumerable<CodeInstruction> CPTranspiler(IEnumerable<CodeInstruction> codes)
    {
        return ReplayModulePatch.CPTranspiler(codes);
    }
    [HarmonyPostfix]
    [HarmonyPatch("Awake")]
    public static void AwakePostfix(PlayerScriptEditor __instance)
    {
        ReplayModulePatch.AwakePostfix(__instance);
    }
    [HarmonyPostfix]
    [HarmonyPatch("Update")]
    public static void UpdatePostfix(PlayerScriptEditor __instance)
    {
        ReplayModulePatch.UpdatePostfix(__instance);
    }
    [HarmonyTranspiler]
    [HarmonyPatch("Update")]
    public static IEnumerable<CodeInstruction> UpdateTranspiler(IEnumerable<CodeInstruction> codes)
    {
        return ReplayModulePatch.UpdateTranspiler(codes);
    }
    [HarmonyPrefix]
    [HarmonyPatch("Update")]
    public static void UpdatePrefix(PlayerScriptEditor __instance)
    {
        ReplayModulePatch.UpdatePrefix(__instance);
    }
} 

class CheckpointAddon : MonoBehaviour
{
    public double savedSceneTime;
    public bool lastClick;
}

public class ClickReplay : List<Keyframe>
{
    public void Add(double time, bool click) => Add(new Keyframe(time, click));
}

public struct Keyframe : IComparable<Keyframe>
{
    public double time;
    public bool click;

    public Keyframe(double time, bool click)
    {
        this.time = time;
        this.click = click;
    }

    public int CompareTo(Keyframe other) => Math.Sign(time - other.time);
    
}

public static class ReplayConverter
{
    public static string ReplayToString(ClickReplay replay)
    {
        replay.Sort();
        StringBuilder sb = new();
        foreach (var pair in replay)
        {
            sb.AppendLine((pair.click ? '+' : '-') + pair.time.ToString());
        }
        return sb.ToString();
    }

    public static ClickReplay StringToReplay(string replay)
    {
        ClickReplay result = new();
        var lines = replay.Split(new char[] {'\n', '\r'});
        foreach (var line in lines)
        {
            if (line.Length < 1) continue;
            var click = false;
            switch (line[0]) 
            { 
                case '+':
                    click = true;
                    goto case '-';
                case '-':
                    if(double.TryParse(line.Substring(1), out var time))
                    {
                        result.Add(time, click);
                    }
                    break;

                default: 
                    continue;
            }
        }
        return result;
    }
}
