using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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

	internal static ManualLogSource Log;

	internal readonly Harmony Harmony;

	internal readonly Assembly Assembly;

	public static ConfigFile ConfigFile = new ConfigFile(Path.Combine(Paths.ConfigPath, MODNAME + ".cfg"), saveOnInit: true);



    private static GameObject Load;
    internal static Material CustomMaterial;
    internal static Material RedMaterial;

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

	public _3DashUtils()
    {
        Log = base.Logger;
        Log.LogDebug("Plugin Constructing...");
        Harmony = new Harmony(GUID);
		Assembly = Assembly.GetExecutingAssembly();
		Path.GetDirectoryName(Assembly.Location);
	}

	public void Start()
    {
		Log.LogDebug("Plugin Starting...");

        var bundle = AssetBundle.LoadFromMemory(Properties.Resources.shaderbundle);
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
				mcat.windowRect = new Rect(20 + (i*200), 20, 100, 100);
                moduleCategories.Add(modObj.CategoryName,mcat);
                i++;
			}
        }
        Load = new GameObject();
		Load.name = "3DashUtils Manager";
		Load.AddComponent<_3DashUtilsScript>();
		DontDestroyOnLoad(Load);
		Harmony.PatchAll(Assembly);

	}
}
