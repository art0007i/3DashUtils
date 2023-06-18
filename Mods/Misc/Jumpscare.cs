using _3DashUtils.Mods.Hidden;
using _3DashUtils.ModuleSystem;
using BepInEx.Configuration;
using HarmonyLib;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
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

    public static Sprite jumpscareSprite = null;
    public static AudioClip jumpscareAudio = null;

    public override void Awake()
    {
        base.Awake();
        // let it load async
        _ = LoadJumpscareAssets();

    }

    public async Task LoadJumpscareAssets()
    {
        var path = Path.Combine(Path.GetDirectoryName(this.GetType().Assembly.Location), "Resources");
        _3DashUtils.Log.LogMessage(path);

        var req = UnityWebRequestMultimedia.GetAudioClip("file:///" + Path.Combine(path, "jumpscare.mp3"), AudioType.MPEG);
        var reqask = req.SendWebRequest();
        reqask.completed += (op) =>
        {
            jumpscareAudio = DownloadHandlerAudioClip.GetContent(req);
        };

        var fs = File.OpenRead(Path.Combine(path, "jumpscare.png"));
        var bytes = new byte[fs.Length];
        await fs.ReadAsync(bytes, 0, (int)fs.Length);
        var jumpscareTex = new Texture2D(4, 4);
        ImageConversion.LoadImage(jumpscareTex, bytes);
        jumpscareTex.Apply(true, true);
        jumpscareSprite = Sprite.Create(jumpscareTex, new Rect(0.0f, 0.0f, jumpscareTex.width, jumpscareTex.height), new Vector2(0.5f, 0.5f), 100.0f);

        
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
        if (UnityEngine.Random.value < Jumpscare.valueOption.Value)
        {
            var gamer = new GameObject("Jumpscare");
            gamer.transform.SetParent(GameObject.Find("PauseCanvas").transform, false);
            gamer.AddComponent<JumpscareScript>();
        }
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
        var image = gameObject.AddComponent<Image>();
        rect = gameObject.GetComponent<RectTransform>();
        image.sprite = Jumpscare.jumpscareSprite;
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        if(Jumpscare.jumpscareAudio != null)
        {
            AudioSource.PlayClipAtPoint(Jumpscare.jumpscareAudio, Vector3.zero, 100f);
        }
    }
}

[HarmonyPatch(typeof(PauseMenuManager), "Update")]
public static class NoDeathAnimationPatch2
{
    public static void Postfix(PauseMenuManager __instance)
    {
        // MAKE IT LOUD!!
        if (Object.FindObjectOfType<JumpscareScript>() != null)
        {
            AudioListener.volume = 1f;
        }
    }
}
[HarmonyPatch(typeof(PauseMenuManager), "Start")]
public static class NoDeathAnimationPatch3
{
    public static void Postfix(PauseMenuManager __instance)
    {
        __instance.volumeSlider.value = SettingsPersistence.volume.Value;
    }
}