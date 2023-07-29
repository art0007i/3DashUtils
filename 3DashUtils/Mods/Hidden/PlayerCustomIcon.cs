
using _3DashUtils.Compat;
using _3DashUtils.Mods.Misc;
using _3DashUtils.Mods.Shortcuts;
using _3DashUtils.ModuleSystem;
using _3DashUtils.UnityScripts;
using HarmonyLib;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace _3DashUtils.Mods.Hidden;

// I could not figure out how to correctly connect the unity scripts and the actual code base but this works for now.
public class PlayerCustomIcon : ModuleBase
{
    public static List<Material> Color1Mats = null;
    public static List<Material> Color2Mats = null;

    public static List<GameObject> availableCubes = new();

    public static ConfigWrapper<string> IconName = new("PlayerCustomization", "SelectedIcon", "cube000", "The currently selected icon.");
    public static ConfigWrapper<Color> Color1 = new("PlayerCustomization", "Color1", new(1, 0.7129014f, 0), "The primary color of your player.");
    public static ConfigWrapper<Color> Color2 = new("PlayerCustomization", "Color2", new(0, 1, 1), "The secondary color of your player.");
    public static ConfigWrapper<float> RainbowSpeed1 = new("PlayerCustomization", "RainbowSpeed1", 1, "The speed of your primary color rainbow (when rainbow mode is selected).");
    public static ConfigWrapper<float> RainbowSpeed2 = new("PlayerCustomization", "RainbowSpeed2", 1, "The speed of your secondary color rainbow (when rainbow mode is selected).");
    public static ConfigWrapper<float> RainbowBright1 = new("PlayerCustomization", "RainbowBright1", 1, "The brightness of your primary color rainbow (when rainbow mode is selected).");
    public static ConfigWrapper<float> RainbowBright2 = new("PlayerCustomization", "RainbowBright2", 1, "The brightness of your secondary color rainbow (when rainbow mode is selected).");
    public static ConfigWrapper<ColorPickerScript.ColorType> ColorType1 = new("PlayerCustomization", "ColorType1", ColorPickerScript.ColorType.Static, "The type of your primary color.");
    public static ConfigWrapper<ColorPickerScript.ColorType> ColorType2 = new("PlayerCustomization", "ColorType2", ColorPickerScript.ColorType.Static, "The type of your secondary color.");

    public override string CategoryName => "Hidden";


    public PlayerCustomIcon()
    {
        availableCubes = _3DashUtils.UtilsAssetsBundle.LoadAllAssets<GameObject>().Where((go) => go.name.StartsWith("cube")).ToList();
    }

    public override void Update()
    {
        base.Update();
        if (Color1Mats != null)
        {
            var c1 = Color1.Value;
            if (ColorType1.Value == ColorPickerScript.ColorType.Rainbow)
            {
                c1 = ColorPickerScript.GetRainbowColor(false, RainbowSpeed1.Value, RainbowBright1.Value);
            }
            Color1Mats.Do((mat) => mat.color = c1);
        }
        if (Color2Mats != null)
        {
            var c2 = Color2.Value;
            if (ColorType2.Value == ColorPickerScript.ColorType.Rainbow)
            {
                c2 = ColorPickerScript.GetRainbowColor(true, RainbowSpeed2.Value, RainbowBright2.Value);
            }
            Color2Mats.Do((mat) => mat.color = c2);
        }
    }
}

[HarmonyPatch(typeof(PlayerScript), "Awake")]
public class IconReplacer
{
    public static void ReplaceWithCube(GameObject go, GameObject prefab)
    {
        for (int i = go.transform.childCount - 1; i >= 0; i--)
        {
            GameObject.Destroy(go.transform.GetChild(i).gameObject);
        }
        var newCube = GameObject.Instantiate(prefab, go.transform);
        newCube.transform.localScale = Vector3.one / 2;
    }

    public static void Postfix(PlayerScript __instance)
    {
        var ourCube = PlayerCustomIcon.availableCubes.Where((g) => g.name == PlayerCustomIcon.IconName.Value).SingleOrDefault();
        if (ourCube == null)
        {
            return;
        }
        ReplaceWithCube(__instance.shapes[0], ourCube);
        ReplaceWithCube(__instance.shapes[1].transform.Find("CubeOnShip").gameObject, ourCube);
        ReplaceWithCube(__instance.shapes[4].transform.Find("CubeOnShip").gameObject, ourCube);

        __instance.shapes[1].transform.Find("ShipParticles").gameObject.SetActive(false);

        var c1 = new List<Material>();
        var c2 = new List<Material>();
        PlayerCustomIcon.Color1Mats = c1;
        PlayerCustomIcon.Color2Mats = c2;

        var c1names = new string[]
        {
            "SHELLMAT",
            "color1",
        };
        var c2names = new string[]
        {
            "INSIDEMAT",
            "color2",
        };
        foreach (var m in Resources.FindObjectsOfTypeAll<Material>())
        {
            if (c1names.Contains(m.name))
            {
                c1.Add(m);
            }
            if (c2names.Contains(m.name))
            {
                c2.Add(m);
            }
        }
    }
}

[HarmonyPatch(typeof(PlayerScriptEditor), "Awake")]
public class IconReplacerEditor
{

    public static void Postfix(PlayerScript __instance)
    {
        IconReplacer.Postfix(__instance);
    }
}

[HarmonyPatch(typeof(IconPickerScript))]
public class IconChangeHook
{
    [HarmonyPatch(nameof(IconPickerScript.Rebuild))]
    [HarmonyPrefix]
    public static void RebuildPrefix(IconPickerScript __instance)
    {
        __instance.currentIcon = PlayerCustomIcon.IconName.Value;
        __instance.availableIcons = PlayerCustomIcon.availableCubes;
    }
    /*
    [HarmonyPatch(nameof(IconPickerScript.Rebuild))]
    [HarmonyPostfix]
    public static void RebuildPostfix(IconPickerScript __instance)
    {
        __instance.UpdateCurrentIcon(PlayerCustomIcon.IconName.Value);
    }*/

    [HarmonyPatch(nameof(IconPickerScript.UpdateCurrentIcon))]
    [HarmonyPostfix]
    public static void UpdatePostfix(IconPickerScript __instance, string name)
    {
        PlayerCustomIcon.IconName.Value = name;
    }

    [HarmonyPatch(nameof(IconPickerScript.StartPickerScript))]
    [HarmonyPrefix]
    public static void StartPrefix(ColorPickerScript s)
    {
        if (s.isColor2)
        {
            s.pickedColor = PlayerCustomIcon.Color2.Value;
            s.ChangeColorType((int)PlayerCustomIcon.ColorType2.Value);
            s.rainbowSpeedInput.text = PlayerCustomIcon.RainbowSpeed2.Value.ToString();
            s.brightnessSlider.value = PlayerCustomIcon.RainbowBright2.Value;
        }
        else
        {
            s.pickedColor = PlayerCustomIcon.Color1.Value;
            s.ChangeColorType((int)PlayerCustomIcon.ColorType1.Value);
            s.rainbowSpeedInput.text = PlayerCustomIcon.RainbowSpeed1.Value.ToString();
            s.brightnessSlider.value = PlayerCustomIcon.RainbowBright1.Value;
        }
    }

    [HarmonyPatch(nameof(IconPickerScript.UpdatePickerScript))]
    [HarmonyPostfix]
    public static void UpdatePostfix(ColorPickerScript s)
    {
        if (s.isColor2)
        {
            PlayerCustomIcon.RainbowSpeed2.Value = s.LastParsedSpeed;
            PlayerCustomIcon.RainbowBright2.Value = s.brightnessSlider.value;
        }
        else
        {
            PlayerCustomIcon.RainbowSpeed1.Value = s.LastParsedSpeed;
            PlayerCustomIcon.RainbowBright1.Value = s.brightnessSlider.value;
        }
    }
}

[HarmonyPatch(typeof(ColorPickerScript))]
public class ColorChangeHook
{

    [HarmonyPatch(nameof(ColorPickerScript.UpdatePickedColor))]
    [HarmonyPostfix]
    public static void ColorPickPostfix(ColorPickerScript __instance)
    {
        if (__instance.isColor2)
        {
            PlayerCustomIcon.Color2.Value = __instance.pickedColor;
        }
        else
        {
            PlayerCustomIcon.Color1.Value = __instance.pickedColor;
        }
    }

    [HarmonyPatch(nameof(ColorPickerScript.ChangeColorType))]
    [HarmonyPostfix]
    public static void ColorTypePostfix(ColorPickerScript __instance)
    {
        if (__instance.isColor2)
        {
            PlayerCustomIcon.ColorType2.Value = __instance.currentColorType;
        }
        else
        {
            PlayerCustomIcon.ColorType1.Value = __instance.currentColorType;
        }
    }
}
