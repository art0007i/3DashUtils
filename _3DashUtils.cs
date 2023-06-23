using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using _3DashUtils.Mods.Hidden;
using _3DashUtils.ModuleSystem;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;
using UnityEngine.Rendering;

namespace _3DashUtils;

[BepInPlugin(GUID, MODNAME, VERSION)]
public class _3DashUtils : BaseUnityPlugin
{
	public const string MODNAME = "3DashUtils";

	public const string AUTHOR = "art0007i";

	public const string GUID = "me.art0007i.3DashUtils";

	public const string VERSION = "1.0.0";

	public static ConfigFile ConfigFile = new ConfigFile(Path.Combine(Paths.ConfigPath, MODNAME + ".cfg"), saveOnInit: true);

	internal static ManualLogSource Log;
	internal Harmony Harmony;
    internal static Material CustomMaterial;
    internal static Material RedMaterial;
    internal AssetBundle bundle;

    /// <summary>
    /// A list of all loaded <see cref="IMenuModule">modules</see>.
    /// </summary>
	public static HashSet<IMenuModule> moduleList = new();
    
    /// <summary>
    /// Contains a mapping of category names to <see cref="ModuleCategory"/> structs.
    /// </summary>
    /// <remarks>
    /// A category named '<b>Hidden</b>' will not exist in this dict. To access hidden modules use <see cref="moduleList"/>
    /// </remarks>
	public static Dictionary<string, ModuleCategory> moduleCategories = new();

    public static KeyBindEditInfo currentKeybindEditing;
	public class ModuleCategory
	{
		public List<IMenuModule> modules;
		internal int internalWindowID;
		public Rect windowRect;

        public ModuleCategory(List<IMenuModule> modules, int internalWindowID)
        {
            this.modules = modules;
            this.internalWindowID = internalWindowID;
        }
    }

    private void Awake()
    {
        Log = base.Logger;
        Log.LogDebug("Plugin Constructing...");
        Harmony = new Harmony(GUID);
        bundle = AssetBundle.LoadFromMemory(Properties.Resources.shaderbundle);
        var niceMaterial = bundle.LoadAsset<Material>("assets/vcolmat.mat");
        niceMaterial.SetFloat("_Alpha", .69f); // maybe expose this as config later?
        var redMat = new Material(niceMaterial);
        redMat.SetColor("_Color", Color.red);
        CustomMaterial = niceMaterial;
        RedMaterial = redMat;

        int i = 0;
        var modules = typeof(_3DashUtils).Assembly.GetTypes().Where((t) => !t.IsAbstract && !t.IsInterface && typeof(IMenuModule).IsAssignableFrom(t));
        foreach (var mod in modules)
        {
            Log.LogDebug("	Adding Module " + mod.Name);
            var modObj = (IMenuModule)Activator.CreateInstance(mod);
            moduleList.Add(modObj);

            // These don't need to be categorized
            if (modObj.CategoryName == "Hidden") continue;

            if (moduleCategories.TryGetValue(modObj.CategoryName, out var list))
            {
                list.modules.Add(modObj);
            }
            else
            {
                var lst = new List<IMenuModule>(new IMenuModule[] { modObj });
                var mcat = new ModuleCategory(lst, i);
                mcat.windowRect = new Rect(20 + (i * 200), 20, 100, 100);
                moduleCategories.Add(modObj.CategoryName, mcat);
                i++;
            }
        }
        Harmony.PatchAll();
        _3DashUtils.moduleList.Do((p) => p.Awake());
    }

	public void Start()
    {
        _3DashUtils.moduleList.Do((p) => p.Start());
    }

    private string lastTooltipContent;
    private float lastTooltipTime;


    public void Update()
    {
        _3DashUtils.moduleList.Do((p) => p.Update());
    }

    public void FixedUpdate()
    {
        _3DashUtils.moduleList.Do((p) => p.FixedUpdate());
    }
    public void LateUpdate()
    {
        _3DashUtils.moduleList.Do((p) => p.LateUpdate());
    }

    private Rect keyBindEditRect = Extensions.GetMiddleOfScreenRect(new(300,100));

    public void OnGUI()
    {
        //GUI.enabled = false;
        //GUI.Box(new(0, 0, Screen.width, Screen.height), "");
        if (currentKeybindEditing is KeyBindEditInfo i)
        {
            keyBindEditRect = GUI.ModalWindow(-69420, keyBindEditRect, (windowID) =>
            {
                GUILayout.BeginVertical();
                GUILayout.Label($"Enter a new key for <b>{i.keyBindName}</b>");
                GUILayout.Label($"Press <b>ESC</b> to cancel.");
                GUILayout.Label($"Press <b>Backspace</b> to reset the key.");
                
                // I don't know what the best option for these keys are but I think this is a good option.

                // it makes sense, plus it's the pause keybind so we can use it.
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    i.editingFinished = true;
                }
                // it makes sense, plus it's the suicide keybind so we can use it.
                else if (Input.GetKeyDown(KeyCode.Backspace))
                {
                    i.callback(KeyCode.None);
                    i.editingFinished = true;
                }
                else
                {
                    foreach (var item in Enum.GetValues(typeof(KeyCode)))
                    {
                        if(item is KeyCode k)
                        {
                            // you can't bind modules to the key you use to bind keybinds...
                            // also you can't bind to mouse 0 or 1 cause you use that to drag the keybind window (and also mouse0 jumps in game)
                            if(k != Extensions.keyBindEditKeyBind && k != KeyCode.Mouse0 && k != KeyCode.Mouse1 && Input.GetKeyDown(k))
                            {
                                i.callback(k);
                                i.editingFinished = true;
                                break;
                            }
                        }
                    }
                }
                GUILayout.EndVertical();
                GUI.DragWindow();
            }, "Editing Keybind");
            GUI.enabled = false;
        }
        if (MenuHandler.menuOpen.Value)
        {
            foreach (var key in _3DashUtils.moduleCategories.Keys)
            {
                if (key == "Hidden")
                {
                    continue;
                }

                var cat = _3DashUtils.moduleCategories[key];

                cat.windowRect = GUILayout.Window(cat.internalWindowID, cat.windowRect, (windowID) =>
                {
                    if(currentKeybindEditing is KeyBindEditInfo) GUI.enabled = false;
                    cat.modules.Sort((mod1, mod2) => Math.Sign(mod2.Priority - mod1.Priority));
                    foreach (var gui in cat.modules)
                    {
                        gui.OnGUI();
                    }
                    if (!string.IsNullOrWhiteSpace(GUI.tooltip))
                    {
                        lastTooltipContent = GUI.tooltip;
                        lastTooltipTime = Time.realtimeSinceStartup;
                    }
                    GUI.DragWindow();
                }, key);
            }

            if (!string.IsNullOrWhiteSpace(lastTooltipContent) && Time.realtimeSinceStartup - lastTooltipTime < 0.5)
            {
                var content = new GUIContent(lastTooltipContent);
                var size = GUIStyles.Tooltip.CalcSize(content);
                var rect = new Rect(20, Screen.height - 60 - size.y, size.x + 20, size.y + 2);
                GUI.Box(rect, content, GUIStyles.Tooltip);
            }
        }
        GUI.enabled = true;
        _3DashUtils.moduleList.Do((p) => p.OnUnityGUI());
    }

    void OnDestroy()
    {
        Harmony.UnpatchSelf();
        bundle.Unload(true);
    }
}
