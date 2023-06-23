using _3DashUtils.Mods.Hidden;
using _3DashUtils.Mods.Player;
using _3DashUtils.ModuleSystem;
using _3DashUtils.ModuleSystem.Config;
using HarmonyLib;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace _3DashUtils.Mods.Misc;

public class Jumpscare : ToggleModule
{
    public override string CategoryName => "Misc";

    public override string ModuleName => "Jumpscare";

    public override string Description => "Jumpscares the player when said player dies, with provided chance by the user.";

    protected override bool Default => false;

    public static Sprite jumpscareSprite = null;
    public static AudioClip jumpscareAudio = null;

    public static double Chance => chanceConfig.Value;
    private static ConfigOptionBase<double> chanceConfig;

    public Jumpscare()
    {
        chanceConfig = new SliderConfig<double>(this, "Chance", 0.05f, "The chance that a jumpscare will appear. 1 means always, 0 means never.", 0, 1);
    }
    public override void Awake()
    {
        base.Awake();
        // let it load async
        _ = LoadJumpscareAssets();

    }

    public async Task LoadJumpscareAssets()
    {
        var path = Path.Combine(Extensions.GetPluginDataPath(), "Resources");

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
}

[HarmonyPatch(typeof(PlayerScript), nameof(PlayerScript.Die))]
public static class NoDeathAnimationPatch
{
    public static void Postfix(bool deathOverride)
    {
        if (!deathOverride && Extensions.Enabled<Noclip>()) return;

        if (Extensions.Enabled<Jumpscare>() && Random.value < Jumpscare.Chance)
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
            GameObject gameObject = new GameObject("jumpscare audio");
            AudioSource audioSource = (AudioSource)gameObject.AddComponent(typeof(AudioSource));
            audioSource.clip = Jumpscare.jumpscareAudio;
            audioSource.spatialBlend = 0f;
            audioSource.spatialize = false;
            audioSource.volume = 100f;
            audioSource.Play();
            Object.Destroy(gameObject, audioSource.clip.length * ((Time.timeScale < 0.01f) ? 0.01f : Time.timeScale));
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