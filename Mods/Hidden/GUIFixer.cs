using _3DashUtils.ModuleSystem;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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