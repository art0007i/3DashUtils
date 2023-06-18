using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using _3DashUtils.ModuleSystem;
using UnityEngine.UI;

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

        if(GUILayout.Button("<color=orange>Quit Game</color>"))
        {
            Application.Quit();
        }
    }
}
