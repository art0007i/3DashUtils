using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using _3DashUtils.ModuleSystem;
using UnityEngine.UI;

namespace _3DashUtils.Mods.Shortcuts;

internal class Shortcuts : ModuleBase
{
    public override string CategoryName => "Shortcuts";

    private Dictionary<string, string> shortcuts = new() {
        {
            "Main Menu", "Menu"
        },
        {
            "Online Levels", "Online Levels Hub"
        },
        {
            "Level Editor", "Save Select"
        }
    };

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
            SceneManager.LoadScene("Menu");
            var menuManager = Object.FindObjectOfType<MenuButtonScript>();
            menuManager.panner.anchoredPosition = Utils.ChangeX(menuManager.pannerTransformGoal, 0f - menuManager.movementDistance);
            menuManager.pannerTransformGoal = Utils.ChangeX(menuManager.pannerTransformGoal, 0f - menuManager.movementDistance);
        }
    }
}
