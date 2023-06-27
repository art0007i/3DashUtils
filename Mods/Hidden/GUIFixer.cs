using _3DashUtils.ModuleSystem;
using HarmonyLib;
using UnityEngine;

namespace _3DashUtils.Mods.Hidden;

public class GUIFixer : ModuleBase
{
    public override string CategoryName => "Hidden";
}

[HarmonyPatch(typeof(FlatEditor), "NullifyIfOverUI")]
class FlatEditorPatch
{
    public static void Postfix(ref bool ___overUI)
    {
        ___overUI |= GUIUtility.hotControl != 0;
    }
}
