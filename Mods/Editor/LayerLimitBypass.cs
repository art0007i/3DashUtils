using _3DashUtils.ModuleSystem;
using BepInEx.Configuration;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3DashUtils.Mods.Editor;

internal class LayerLimitBypass : ToggleModule
{
    public static ConfigEntry<bool> option = _3DashUtils.ConfigFile.Bind("Editor", "LayerLimitBypass", false);
    public override string CategoryName => "Editor";

    public override string ModuleName => "Layer Limit Bypass";

    public override ConfigEntry<bool> Enabled => option;

    public override string Tooltip => "Bypasses the max layer limit of 5.";


}

[HarmonyPatch(typeof(FlatCamera), "Update")]
public class LayerLimitBypassPatch
{

}
