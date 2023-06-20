using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using _3DashUtils.ModuleSystem;
using BepInEx.Configuration;
using HarmonyLib;

namespace _3DashUtils.Mods.Shortcuts;

public class Shortcuts : ModuleBase
{
    public override string CategoryName => "Shortcuts";

    private Dictionary<string, string> shortcuts = new() {
        {
            "Main Menu", "Menu"
        },
        {
            "Level Editor", "Save Select"
        },
        {
            "Online Levels", "Online Levels Hub"
        },
    };

    private bool waitingForSceneChange;

    private List<ConfigEntry<KeyCode>> keyBinds = new();

    public Shortcuts()
    {
        keyBinds.Add(_3DashUtils.ConfigFile.Bind("Keybinds", "MainMenuShortcut", KeyCode.None, "Keybind for loading the main menu instantly."));
        keyBinds.Add(_3DashUtils.ConfigFile.Bind("Keybinds", "LevelEditorShortcut", KeyCode.None, "Keybind for loading the level editor instantly."));
        keyBinds.Add(_3DashUtils.ConfigFile.Bind("Keybinds", "OnlineLevelsShortcut", KeyCode.None, "Keybind for loading the online levels hub instantly."));
        keyBinds.Add(_3DashUtils.ConfigFile.Bind("Keybinds", "OfflineLevelsShortcut", KeyCode.None, "Keybind for loading the offline levels page instantly."));
        keyBinds.Add(_3DashUtils.ConfigFile.Bind("Keybinds", "QuitGameShortcut", KeyCode.None, "Keybind for quitting the game instantly."));
    }

    public override void Awake()
    {
        SceneManager.activeSceneChanged += (oldScene, newScene) =>
        {
            if (!waitingForSceneChange) return;
            waitingForSceneChange = false;
            if (newScene.name == "Menu")
            {
                var menuManager = Object.FindObjectOfType<MenuButtonScript>();
                menuManager.panner.anchoredPosition = Utils.ChangeX(menuManager.pannerTransformGoal, 0f - menuManager.movementDistance);
                menuManager.pannerTransformGoal = Utils.ChangeX(menuManager.pannerTransformGoal, 0f - menuManager.movementDistance);
            }
        };
    }

    public static string levelIdText;
    public static bool dontAutoLoad;

    public override void OnGUI()
    {
        foreach(var shortcut in shortcuts)
        {
            if(GUILayout.Button(shortcut.Key))
            {
                SceneManager.LoadScene(shortcut.Value);
            }
        }

        if(GUILayout.Button("Offline Levels"))
        {
            waitingForSceneChange = true;
            SceneManager.LoadScene("Menu");
        }

        GUILayout.BeginHorizontal();
        levelIdText = GUILayout.TextArea(levelIdText, GUILayout.Width(50));
        if(GUILayout.Button("Open Level ID", GUILayout.ExpandWidth(true)))
        {
            dontAutoLoad = true;
            SceneManager.LoadScene("Online Levels Hub");
        }
        GUILayout.EndHorizontal();

        var quitText = "Quit Game";
        if (GUILayout.Button($"<color=orange>{quitText}</color>"))
        {
            Application.Quit();
        }
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

        // if autoload is true, return false (skips func which loads online levels)
        return !d;
    }
}