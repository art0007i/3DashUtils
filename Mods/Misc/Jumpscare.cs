using _3DashUtils.Mods.Player;
using _3DashUtils.ModuleSystem;
using BepInEx.Configuration;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace _3DashUtils.Mods.Misc;

public class Jumpscare : TextEditorModule<double>
{
    public static ConfigEntry<bool> option = _3DashUtils.ConfigFile.Bind("Misc", "Jumpscare", false);
    public static ConfigEntry<double> valueOption = _3DashUtils.ConfigFile.Bind("Misc", "JumpscareChance", 0.05);
    public override string CategoryName => "Misc";

    public override string ModuleName => "Jumpscare";

    public override ConfigEntry<bool> Enabled => option;
    public override ConfigEntry<double> Value => valueOption;

    public override string Tooltip => "Jumpscares the player when said player dies, with provided chance by the user.";

    public static Sprite jumpscareSprite;

    public override void Awake()
    {
        base.Awake();
        var img = Properties.Resources.jumpscare;
        var jumpscareTex = new Texture2D(img.Width, img.Height);
        var converter = new System.Drawing.ImageConverter();
        var bytes = (byte[])converter.ConvertTo(img, typeof(byte[]));
        jumpscareTex.LoadRawTextureData(bytes);

        jumpscareSprite = Sprite.Create(jumpscareTex, new Rect(0.0f, 0.0f, jumpscareTex.width, jumpscareTex.height), new Vector2(0.5f, 0.5f), 100.0f);
    }

    public static void Death()
    {
        //roll rng here and do jumpscare
        var rand = new System.Random();
        if(rand.NextDouble() < valueOption.Value)
        {
            var gamer = new GameObject("Jumpscare");
            gamer.transform.SetParent(GameObject.Find("PauseCanvas").transform, false);
            gamer.AddComponent<JumpscareScript>();
        }
    }

    public override bool TryParseText(string text, out double parse)
    {
        return double.TryParse(text, out parse) && parse > 0 && parse <= 1;
    }
}

[HarmonyPatch(typeof(PlayerScript), "Die")]
public static class NoDeathAnimationPatch
{
    public static void Prefix()
    {
        Jumpscare.Death();
    }
}

public class JumpscareScript : MonoBehaviour
{
    RectTransform rect;
    public void Update()
    {
        rect.anchorMin -= new Vector2(Time.deltaTime, Time.deltaTime);
        rect.anchorMax += new Vector2(Time.deltaTime, Time.deltaTime);
    }

    public void Start()
    {
        rect = gameObject.GetComponent<RectTransform>();
        var image = gameObject.AddComponent<Image>();
        image.sprite = Jumpscare.jumpscareSprite;
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
    }
}