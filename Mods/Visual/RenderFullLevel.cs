using _3DashUtils.ModuleSystem;
using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace _3DashUtils.Mods.Visual;

internal class RenderFullLevel : ToggleModule
{
    public static ConfigEntry<bool> option = _3DashUtils.ConfigFile.Bind("Visual", "RenderFullLevel", false);
    public override string CategoryName => "Visual";
    public override string ModuleName => "Render Full Level";
    public override ConfigEntry<bool> Enabled => option;
    public override string Tooltip => "Renders the entire level at the same time.";

    public override void Update()
    {
        GameObject gameObject = GameObject.Find("WorldGenerator");
        if (!(gameObject == null))
        {
            gameObject.GetComponent<WorldGenerator>().renderDistance = Enabled.Value ? float.PositiveInfinity : 14f;
        }
        GameObject gameObject2 = GameObject.Find("WorldGeneratorEditor");
        if (!(gameObject2 == null))
        {
            gameObject2.GetComponent<WorldGeneratorEditor>().renderDistance = Enabled.Value ? float.PositiveInfinity : 14f;
        }
    }
}
