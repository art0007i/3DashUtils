using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using _3DashUtils.Compat;
using _3DashUtils.Mods.Player;
using _3DashUtils.ModuleSystem;
using _3DashUtils.ModuleSystem.Config;
using HarmonyLib;
using UnityEngine;

namespace _3DashUtils.Mods.Replays;

public enum ReplayMode
{
    Recording,
    Replaying
}
public class ReplayModule : ToggleModule, IConfigurableModule
{
    public override string CategoryName => "Replays";

    public override string ModuleName => "[Beta] Replay System";

    public override string Description => "Allows you to record and play replay files. Currently slightly unstable.";

    // these 2 are for recording
    static public double lastOffset;
    static public bool lastClick = false;

    // these 2 are for playback
    public static int lastKframe = -1;
    public static bool shouldClick;

    static public ClickReplay CurrentReplay = new();

    public static double CurrentTime => lastOffset + Time.timeSinceLevelLoadAsDouble;

    public static ReplayMode Mode { get => modeConfig.Value; set { modeConfig.Value = value; } }

    protected override bool Default => false;

    public static ConfigWrapper<ReplayMode> modeConfig = new("Replays", "Mode", ReplayMode.Recording, "Current mode of the ReplayModule.");

    public static string ReplayName { get => replayNameOption.Value; set => replayNameOption.Value = value; }
    public static string ReplayPath { get => Path.Combine(basePath, ReplayName) + ".3dr"; }
    public static ConfigOptionBase<string> replayNameOption { get; set; }

    static string basePath = Path.Combine(Extensions.GetPluginDataPath(), "Replays"); //3dash replay (.3dr)

    public ReplayModule()
    {
        replayNameOption = new TextInputConfig<string>(this, "ReplayName", "replay name", "The name of the replay you will save/load.", PathValidator);
    }

    public static bool PathValidator(string path)
    {
        var boo = true;
        try
        {
            Path.GetFullPath(Path.Combine(basePath, path) + ".3dr");
        }
        catch
        {
            boo = false;
        }
        return boo;
    }

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
            File.WriteAllText(ReplayPath, ReplayConverter.ReplayToString(CurrentReplay));
        }
        if (GUILayout.Button("Load Replay"))
        {
            CurrentReplay = ReplayConverter.StringToReplay(File.ReadAllText(ReplayPath));
        }
    }


    public static void RemoveFutureClicks()
    {
        _3DashUtils.Log.Dbg("removeing everything after " + CurrentTime);
        for (int i = CurrentReplay.Count - 1; i >= 0; i--)
        {
            if (CurrentReplay[i].time >= CurrentTime)
            {
                CurrentReplay.RemoveAt(i);
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
    public static CheckpointScript Injection(CheckpointScript script, PlayerScript p)
    {
        var add = script.gameObject.AddComponent<CheckpointAddon>();
        add.SaveCP(p);

        _3DashUtils.Log.Dbg("adding cp " + add.savedSceneTime);
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
            if (code.operand is MethodInfo mi && mi == lookFor)
            {
                yield return new(OpCodes.Ldarg_0);
                yield return new(OpCodes.Call, injectFunc);
            }
        }
    }
    [HarmonyPostfix]
    [HarmonyPatch("Awake")]
    public static void AwakePostfix(PlayerScript __instance)
    {
        _3DashUtils.Log.Dbg("death at " + ReplayModule.CurrentTime);
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
                _3DashUtils.Log.Dbg("loading cp " + a.savedSceneTime);
                a.LoadCP(__instance);
            }
        }
    }



    [HarmonyPrefix]
    [HarmonyPatch("Update")]
    public static void UpdatePrefix(PlayerScript __instance)
    {
        if (Extensions.Enabled<ReplayModule>() && ReplayModule.Mode == ReplayMode.Replaying)
        {
            // safety window of 0.001 (dT at 60fps is 0.1666... so 0.001 shouldn't fuck up subframe :)
            var i = ReplayModule.CurrentReplay.FindLastIndex((l) => l.time <= (ReplayModule.CurrentTime + 0.001));
            if (i >= 0)
            {
                var currentKframe = ReplayModule.CurrentReplay[i];
                var shouldClick = currentKframe.click;
                var last = ReplayModule.lastKframe;
                if (last != i)
                {
                    var count = Math.Abs(i - last);
                    // check if we skipped any click frames. if so click them
                    if (count >= 1 && i + count < ReplayModule.CurrentReplay.Count && ReplayModule.CurrentReplay.GetRange(i, count).Any((k) => k.click == true))
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
    public static void UpdatePostfix(PlayerScript __instance)
    {
        if (Extensions.Enabled<ReplayModule>() && ReplayModule.Mode == ReplayMode.Recording)
        {
            var player = GameObject.FindObjectOfType<PlayerScript>();
            if (player == null) return;
            var click = player.jumpInput;
            if (ReplayModule.lastClick != click)
            {
                var t = ReplayModule.CurrentTime;
                ReplayModule.CurrentReplay.Add(t, click);
                //_3DashUtils.Log.LogMessage("adding click " + click + " at " + ReplayModule.CurrentTime);
            }
            ReplayModule.lastClick = click;
        }
        //if(!PauseMenuManager.paused)
        //    _3DashUtils.Log.LogMessage("jumpo!! " + ReplayModule.CurrentTime.ToString() + " " + __instance.jumpInput);
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

public class CheckpointAddon : MonoBehaviour
{
    public static FieldInfo[] props;
    public double savedSceneTime;
    public bool lastClick;

    // PlayerScript
    // not necessary..
    public bool noDeath;
    public bool noCollision;

    public bool jumpInput;
    public bool jumpInputPressed;
    public bool canHitOrb;
    public bool canHedron;
    // not necessary
    //public bool dead;
    public bool onGround;
    // not necessary (inherits from PathFollower, which is saved)
    //public float speed;
    public float gravityMultiplier;

    //PlayerScriptEditor
    public bool jumpInputWasPressed;

    // prob excessive
    public Vector3 gfx_localScale;
    public Quaternion gfx_rotation;
    public Vector3 rb_velocity;
    public bool CubeCollider_enabled;
    public bool isCube;
    public bool isRocket;
    public bool isWave;
    public bool isHedron;
    public bool isUfo;
    public bool isSmall;


    public void SaveCP(PlayerScript p)
    {
        savedSceneTime = ReplayModule.CurrentTime;
        lastClick = ReplayModule.lastClick;

        noDeath = p.noDeath;
        noCollision = p.noCollision;
        jumpInput = p.jumpInput;
        jumpInputPressed = p.jumpInputPressed;
        canHitOrb = p.canHitOrb;
        canHedron = p.canHedron;
        onGround = p.onGround;
        gravityMultiplier = p.gravityMultiplier;

        // UPDATE
        gfx_localScale = p.gfx.transform.localScale;

        //FIXEDUPDATE
        gfx_rotation = p.gfx.transform.localRotation;
        rb_velocity = p.rb.velocity;
        CubeCollider_enabled = p.CubeCollider.enabled;
        isCube = p.isCube;
        isRocket = p.isRocket;
        isWave = p.isWave;
        isHedron = p.isHedron;
        isUfo = p.isUfo;
        isSmall = p.isSmall;


        if (p is PlayerScriptEditor e)
        {
            jumpInputWasPressed = e.jumpInputWasPressed;
        }
    }
    public void LoadCP(PlayerScript p)
    {
        ReplayModule.lastOffset = savedSceneTime;
        ReplayModule.lastClick = lastClick;
        if (Extensions.Enabled<CheckpointFix>() || Extensions.Enabled<ReplayModule>())
        {
            p.noDeath = noDeath;
            p.noCollision = noCollision;
            p.jumpInput = jumpInput;
            p.jumpInputPressed = jumpInputPressed;
            p.canHitOrb = canHitOrb;
            p.canHedron = canHedron;
            p.onGround = onGround;
            p.gravityMultiplier = gravityMultiplier;

            // UPDATE
            p.gfx.transform.localScale = gfx_localScale;

            //FIXEDUPDATE
            p.gfx.transform.localRotation = gfx_rotation;
            p.rb.velocity = rb_velocity;
            p.CubeCollider.enabled = CubeCollider_enabled;
            p.isCube = isCube;
            p.isRocket = isRocket;
            p.isWave = isWave;
            p.isHedron = isHedron;
            p.isUfo = isUfo;
            p.isSmall = isSmall;

            if (p is PlayerScriptEditor e)
            {
                e.jumpInputWasPressed = jumpInputWasPressed;
            }
        }
    }
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
        var lines = replay.Split(new char[] { '\n', '\r' });
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
                    if (double.TryParse(line.Substring(1), out var time))
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
