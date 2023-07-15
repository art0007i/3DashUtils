using System;
using System.Collections.Generic;
using System.Linq;
using _3DashUtils.Mods.Misc;
using _3DashUtils.ModuleSystem;
#if BEPINEX
using BepInEx;
using BepInEx.Configuration;
using System.IO; // maybe this should be outside, but it's unused in melon rn and i remove unused too often
#elif MELON
using MelonLoader;
#endif
using HarmonyLib;
using UnityEngine;
using _3DashUtils.Compat;
using System.Runtime.CompilerServices;
using System.Reflection;

namespace _3DashUtils;

#if BEPINEX
[BepInPlugin(GUID, MODNAME, VERSION)]
#endif

public class _3DashUtils 
    :
#if BEPINEX
    BaseUnityPlugin
#elif MELON
    MelonMod
#endif
{
    public const string MODNAME = "3DashUtils";

    public const string AUTHOR = "art0007i";

    public const string GUID = "me.art0007i.3DashUtils";

    public const string VERSION = "1.1.1";

    public static UniversalLogger Log { get; private set; }
    public static new HarmonyLib.Harmony Harmony;
#if BEPINEX
    public static ConfigFile ConfigFile = new ConfigFile(Path.Combine(Paths.ConfigPath, MODNAME + ".cfg"), saveOnInit: true);
#endif

    internal static AssetBundle UtilsAssetsBundle;
    internal static AssetBundle UtilsIconKitBundle;

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
#if BEPINEX
    private void Awake()
#elif MELON
    public override void OnInitializeMelon()
#endif
    {
#if BEPINEX
        Harmony = new Harmony(GUID);
#elif MELON
        Harmony = HarmonyInstance;
#endif
        Log = new(this);
        Log.Dbg("Plugin Constructing...");
        UtilsAssetsBundle = AssetBundle.LoadFromMemory(Properties.Resources.utilsassets);
        UtilsIconKitBundle = AssetBundle.LoadFromMemory(Properties.Resources.utilsiconkit);
        var pt = Path.GetDirectoryName(typeof(_3DashUtils).Assembly.Location);
        var asm = Assembly.LoadFile(pt + "./3DashUtilsUnityScripts.dll");
        Log.Dbg("Types: \n" + string.Join("\n  ", asm.GetTypes().AsEnumerable()));
        int i = 0;
        var modules = typeof(_3DashUtils).Assembly.GetTypes().Where((t) => !t.IsAbstract && !t.IsInterface && typeof(IMenuModule).IsAssignableFrom(t));
        foreach (var mod in modules)
        {
            Log.Dbg("	Adding Module " + mod.Name);
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
                mcat.windowRect = new Rect(20 + (i * 200), 20, 180, 100);
                moduleCategories.Add(modObj.CategoryName, mcat);
                i++;
            }
        }
#if BEPINEX
        Harmony.PatchAll();
#endif
        moduleList.Do((p) => p.Awake());
#if MELON
        MelonEvents.OnApplicationLateStart.Subscribe(() => moduleList.Do((p) => p.Start()));
#endif

    }
#if BEPINEX
    public void Start()
    {
        moduleList.Do((p) => p.Start());
    }
    void OnDestroy()
    {
        Harmony.UnpatchSelf();
        UtilsAssetsBundle.Unload(true);
    }
#endif

    private string lastTooltipContent;
    private float lastTooltipTime;

#if BEPINEX
    public void Update()
#elif MELON
    public override void OnUpdate()
#endif
    {
        moduleList.Do((p) => p.Update());
    }

#if BEPINEX
    public void FixedUpdate()
#elif MELON
    public override void OnFixedUpdate()
#endif
    {
        moduleList.Do((p) => p.FixedUpdate());
    }
#if BEPINEX
    public void LateUpdate()
#elif MELON
    public override void OnLateUpdate()
#endif
    {
        moduleList.Do((p) => p.LateUpdate());
    }

    private Rect keyBindEditRect = Extensions.GetMiddleOfScreenRect(new(320, 100));

    public static List<KeyBindInfo> conflicts;
    public static bool menuOpenFallback;

    public static void EditKey(KeyBindEditInfo info)
    {
        currentKeybindEditing = info;
        conflicts = null;
    }

#if BEPINEX
    public void OnGUI()
#elif MELON
    public override void OnGUI()
#endif
    {
        //GUI.enabled = false;
        //GUI.Box(new(0, 0, Screen.width, Screen.height), "");
        if (currentKeybindEditing is KeyBindEditInfo i)
        {
            keyBindEditRect.height = conflicts == null ? 100 : 200;
            keyBindEditRect = GUI.ModalWindow(-69420, keyBindEditRect, (windowID) =>
            {
                GUILayout.BeginVertical();
                if (conflicts == null)
                {

                    GUILayout.Label(menuOpenFallback ?
                        $"It seems you have unbound the <b>Menu Open</b> key, please select a new key for it." :
                        $"Enter a new key for <b>{i.keyBindName}</b>"
                    );
                    if (!menuOpenFallback) GUILayout.Label($"Press <b>ESC</b> to cancel.");
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
                        i.callback(i.defaultKey);
                        i.editingFinished = true;
                    }
                    else
                    {
                        foreach (var item in Enum.GetValues(typeof(KeyCode)))
                        {
                            if (item is KeyCode k)
                            {
                                // you can't bind modules to the key you use to bind keybinds...
                                // also you can't bind to mouse 0 or 1 cause you use that to drag the keybind window (and also mouse0 jumps in game)
                                if (k != KeyCode.None && k != KeyCode.Mouse0 && k != KeyCode.Mouse1 && k != Extensions.keyBindEditKeyBind && Input.GetKeyDown(k))
                                {
                                    var kbs = Extensions.CollectKeyBindInfos().Where((kbi) => kbi.KeyBind == k).ToList();
                                    if (kbs.Count > 0)
                                    {
                                        i.selectedKey = k;
                                        conflicts = kbs;
                                    }
                                    else
                                    {
                                        i.callback(k);
                                        i.editingFinished = true;
                                    }
                                    break;
                                }
                            }
                        }
                    }
                }
                else
                {
                    GUILayout.Label($"<color=orange>Warning!</color> One or more keybinds have the same key (<b>{i.selectedKey}</b>):");
                    GUILayout.Label(string.Join(", ", conflicts.Select((c) => "<b>" + Extensions.SplitPascalCase(c.Name) + "</b>")));
                    var opt = GUILayout.Toolbar(-1, new string[] { "Continue", "Unbind Others", "Cancel" });
                    if (opt == 0) // continue
                    {
                        i.callback(i.selectedKey);
                    }
                    else if (opt == 1) // unbind others
                    {
                        conflicts.Do((kbi) => kbi.KeyBind = KeyCode.None);
                        i.callback(i.selectedKey);
                    } // cancel
                    if (opt >= 0 || Input.GetKeyDown(KeyCode.Escape))
                    {
                        i.editingFinished = true;
                        conflicts = null;
                    }
                }
                GUILayout.EndVertical();
                GUI.DragWindow();
            }, "Editing Keybind");
            GUI.enabled = false;
        }
        if (Extensions.Enabled<UtilityMenu>())
        {
            foreach (var key in moduleCategories.Keys)
            {
                if (key == "Hidden")
                {
                    continue;
                }
                var cat = moduleCategories[key];
                cat.windowRect = GUILayout.Window(cat.internalWindowID, cat.windowRect, (windowID) =>
                {
                    if (currentKeybindEditing is KeyBindEditInfo) GUI.enabled = false;
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
        moduleList.Do((p) => p.OnUnityGUI());
    }
}
