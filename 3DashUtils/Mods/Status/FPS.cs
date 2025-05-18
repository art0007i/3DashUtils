using System.Security.Permissions;
using _3DashUtils.ModuleSystem;
using _3DashUtils.ModuleSystem.Config;
using UnityEngine;

namespace _3DashUtils.Mods.Status;
public class FPS : TemplateLabel
{
    public override string CategoryName => "Status";

    public override string ModuleName => "FPS";

    public override string Description => "Shows an FPS label in the top left corner";

    protected override bool Default => false;

    public override string text { get; set; } = "";

    public override void UpdateText()
    {
        float f = 1f / Time.unscaledDeltaTime;
        text = $"FPS: {Mathf.Round(f)}";
    }
}

