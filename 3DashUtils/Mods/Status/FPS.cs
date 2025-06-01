using System.Security.Permissions;
using _3DashUtils.ModuleSystem;
using _3DashUtils.ModuleSystem.Config;
using UnityEngine;

namespace _3DashUtils.Mods.Status;
public class FPS : TemplateLabel
{
    public override string CategoryName => "Status";

    public override string ModuleName => "FPS";

    protected override bool Default => false;

    public override string text
    {
        get
        {
            float f = 1f / Time.unscaledDeltaTime;
            return $"FPS: {Mathf.Round(f)}";
        }
    }
}

