using _3DashUtils.Mods.Misc;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;

namespace _3DashUtils.Mods.Hidden;

internal class MainMenuButtons
{
}



[HarmonyPatch(typeof(MenuButtonScript), "Start")]
public class BonusMenuButtons
{
    /// <summary>
    /// Moves a main menu button to the left, changes it's text and overrides it's click function
    /// </summary>
    /// <param name="buttonObj">The game object of the button you want to affect</param>
    /// <param name="text">The text that the button will have on it</param>
    /// <param name="onClick">The unity action that will be called when the button is clicked</param>
    private static void ModifyButton(GameObject buttonObj, string text, UnityAction onClick)
    {
        var textMesh = buttonObj.GetComponentInChildren<TMPro.TextMeshProUGUI>();
        var textTransform = textMesh.GetComponent<RectTransform>();
        // fixing shitty ui using mods yippee
        textMesh.text = text;
        textTransform.offsetMax = new Vector2(-20, 0);
        textTransform.offsetMin = new Vector2(20, 0);
        textTransform.anchorMax = Vector2.one;
        textTransform.anchorMin = Vector2.zero;
        var dupedTransform = buttonObj.GetComponent<RectTransform>();
        dupedTransform.offsetMax = new Vector2(dupedTransform.offsetMin.x - 20, dupedTransform.offsetMax.y);
        dupedTransform.offsetMin = new Vector2(dupedTransform.offsetMin.x * 2, dupedTransform.offsetMin.y);
        var btn = buttonObj.transform.GetChild(0).GetComponent<Button>();
        var evt = new Button.ButtonClickedEvent();
        evt.AddListener(onClick);
        btn.onClick = evt;
    }

    public static void Postfix()
    {
        _3DashUtils.Log.Msg("MAIN MENU PATCHES");
        var menuButtons = GameObject.Find("Menu Buttons");
        var duped = GameObject.Instantiate<GameObject>(menuButtons.transform.GetChild(2).gameObject, menuButtons.transform);
        ModifyButton(duped, "3DashUtils", () =>
        {
            Extensions.GetModule<UtilityMenu>().Enabled = !Extensions.GetModule<UtilityMenu>().Enabled;
        });
        var duped2 = GameObject.Instantiate<GameObject>(menuButtons.transform.GetChild(3).gameObject, menuButtons.transform);
        ModifyButton(duped2, "Icon Kit", () =>
        {
            var iconKitName = _3DashUtils.UtilsIconKitBundle.GetAllScenePaths().Where(p => p.EndsWith("IconKit.unity")).Single();
            SceneManager.LoadScene(Path.GetFileNameWithoutExtension(iconKitName));
        });
    }
}
