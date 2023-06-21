using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using _3DashUtils.Mods.Hidden;
using _3DashUtils.ModuleSystem;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;

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

    public void OnGUI()
    {
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
        _3DashUtils.moduleList.Do((p) => p.OnUnityGUI());
    }

    void OnDestroy()
    {
        Harmony.UnpatchSelf();
        bundle.Unload(true);
    }
}
