using _3DashUtils.ModuleSystem;
using _3DashUtils.ModuleSystem.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace _3DashUtils.Mods.Misc;

public class Fullscreen : ToggleModule
{
    public override string CategoryName => "Misc";

    protected override bool Default => true;

    public override string Description => "Toggles fullscreen mode. Can always be toggled by Alt+Enter (default unity keybind).";

    public int ResolutionX => resolutionXconfig.Value;
    public ConfigOptionBase<int> resolutionXconfig;
    public int ResolutionY => resolutionYconfig.Value;
    public ConfigOptionBase<int> resolutionYconfig;

    public Fullscreen()
    {
        resolutionXconfig = new TextInputConfig<int>(this, "Width", 1280, "The width of the non-fullscreen window.", (i) => i > 0);
        resolutionYconfig = new TextInputConfig<int>(this, "Height", 720, "The height of the non-fullscreen window.", (i) => i > 0);
    }

    public override void Awake()
    {
        base.Awake();
        Enabled = Screen.fullScreen;
    }

    public override void OnToggle()
    {
        base.OnToggle();
        Screen.fullScreen = Enabled;
        Screen.SetResolution(Display.main.systemWidth, Display.main.systemHeight, Enabled);
    }

    public override void Update()
    {
        base.Update();
        Enabled = Screen.fullScreen;
        if (!Enabled)
        {
            var w = ResolutionX;
            var h = ResolutionY;
            if (Screen.width != w || Screen.height != h)
            {
                Screen.SetResolution(w, h, Enabled);
            }
        }
    }
}
