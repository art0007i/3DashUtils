using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using _3DashUtils.ModuleSystem;
using HarmonyLib;
using System;
using UnityEngine.Rendering;

namespace _3DashUtils.Mods.Shortcuts;

public class Shortcuts : ModuleBase, IKeybindModule
{
    public override string CategoryName => "Shortcuts";
    public List<KeyBindInfo> KeyBinds { get; private set; } = new();

    private List<Shortcut> shortcuts;

    private bool waitingForSceneChange = false;

    private struct Shortcut
    {
        public string Name;
        public string Description;
        public KeyBindInfo KeyBind;

        public Shortcut(string name, string description, KeyBindInfo keyBind)
        {
            Name = name;
            Description = description;
            KeyBind = keyBind;
        }
    }

    public Shortcuts()
    {
        shortcuts = new() {
            new("Main Menu", "Loads the main menu instantly.",
                new("MainMenuShortcut",
                ()=>SceneManager.LoadScene("Menu"),
                "Keybind for loading the main menu instantly.")
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
        };
        shortcuts.Do((sh) => KeyBinds.Add(sh.KeyBind));
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

    private void KeybindButton(ref int i)
    {
        var shortcut = shortcuts[i];
        var keys = Extensions.EditingKeybinds();
        var buttonLabel = shortcut.Name + (keys ? $": <b>{shortcut.KeyBind.KeyBind}</b>" : "");
        var content = new GUIContent(buttonLabel, shortcut.Description);
        if (GUILayout.Button(content))
        {
            if (keys)
            {
                _3DashUtils.EditKey(new(KeyCode.None, (key) => shortcut.KeyBind.KeyBind = key, shortcut.Name + " Shortcut"));
            }
            else
            {
                shortcut.KeyBind.KeyCallback();
            }
        }
        i++;
    }

    public override void OnGUI()
    {
        var i = 0;
        KeybindButton(ref i);
        KeybindButton(ref i);
        KeybindButton(ref i);
        KeybindButton(ref i);

        GUILayout.BeginHorizontal();
        levelIdText = GUILayout.TextArea(levelIdText, GUILayout.Width(50));
        if (GUILayout.Button("Open Level ID", GUILayout.ExpandWidth(true)))
        {
            dontAutoLoad = true;
            SceneManager.LoadScene("Online Levels Hub");
        }
        GUILayout.EndHorizontal();

        KeybindButton(ref i);
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
