using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using _3DashUtils.ModuleSystem;
using HarmonyLib;
using System;
using UnityEngine.Rendering;
using System.IO;
using System.Linq;

namespace _3DashUtils.Mods.Shortcuts;

public class Shortcuts : ButtonsModule
{
    public override string CategoryName => "Shortcuts";

    protected override List<KeyBindButton> Buttons { get; } = new();

    private bool waitingForSceneChange = false;

    public Shortcuts()
    {
        //var s = _3DashUtils.UtilsIconKitBundle.LoadAsset("Utils/IconKit.unity");
        var iconKitName = _3DashUtils.UtilsIconKitBundle.GetAllScenePaths().Where(p => p.EndsWith("IconKit.unity")).Single();
        Buttons.AddRange(new KeyBindButton[] {
            new("Main Menu", "Loads the main menu instantly.",
                new("MainMenuShortcut",
                ()=>SceneManager.LoadScene("Menu"),
                "Keybind for loading the main menu instantly.")
            ),
            new("Icon Kit", "Loads the icon kit instantly.",
                new("IconKitShortcut",
                ()=>SceneManager.LoadScene(Path.GetFileNameWithoutExtension(iconKitName)),
                "Keybind for loading the icon kit instantly.")
            ),
            new("Level Editor", "Loads the level editor instantly.",
                new("LevelEditorShortcut",
                ()=>SceneManager.LoadScene("Save Select"),
                "Keybind for loading the level editor instantly.")
            ),
            new("Online Levels", "Loads the online levels page instantly.",
                new("OnlineLevelsShortcut",
                ()=>SceneManager.LoadScene("Online Levels Hub"),
                "Keybind for loading the online levels page instantly.")
            ),
            new("Offline Levels", "Loads the offline levels page instantly.",
                new("OfflineLevelsShortcut",
                ()=>{
                    waitingForSceneChange = true;
                    SceneManager.LoadScene("Menu");
                },
                "Keybind for loading the offline levels page instantly.")
            ),
            new("<color=orange>Quit Game</color>", "Loads the offline levels page instantly.",
                new("QuitGameShortcut",
                Application.Quit,
                "Keybind for quitting the game instantly.")
            ),
        }) ;
    }

    public override void Awake()
    {
        SceneManager.activeSceneChanged += (oldScene, newScene) =>
        {
            if (!waitingForSceneChange) return;
            waitingForSceneChange = false;
            if (newScene.name == "Menu")
            {
                var menuManager = UnityEngine.Object.FindObjectOfType<MenuButtonScript>();
                menuManager.panner.anchoredPosition = Utils.ChangeX(menuManager.pannerTransformGoal, 0f - menuManager.movementDistance);
                menuManager.pannerTransformGoal = Utils.ChangeX(menuManager.pannerTransformGoal, 0f - menuManager.movementDistance);
            }
        };
    }

    public static string levelIdText;
    public static bool dontAutoLoad;
    public override void OnGUI()
    {
        base.OnGUI();
        DrawKeybindButton();
        DrawKeybindButton();
        DrawKeybindButton();
        DrawKeybindButton();

        GUILayout.BeginHorizontal();
        levelIdText = GUILayout.TextArea(levelIdText, GUILayout.Width(50));
        if (GUILayout.Button("Open Level ID", GUILayout.ExpandWidth(true)))
        {
            dontAutoLoad = true;
            SceneManager.LoadScene("Online Levels Hub");
        }
        GUILayout.EndHorizontal();

        DrawKeybindButton();
    }
}

[HarmonyPatch(typeof(OnlineLevelsHub), nameof(OnlineLevelsHub.Awake))]
class ShortcutsPatch
{
    public static bool Prefix(OnlineLevelsHub __instance)
    {
        var d = int.TryParse(Shortcuts.levelIdText, out var levelId) && Shortcuts.dontAutoLoad;
        Shortcuts.dontAutoLoad = false;
        if (d)
        {
            __instance.LoadLevel(levelId);
        }

        return !d;
    }
}
