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

    public override string Description => "Toggles fullscreen mode.";

    public int ResolutionX => resolutionXconfig.Value;
    public ConfigOptionBase<int> resolutionXconfig;
    public int ResolutionY => resolutionYconfig.Value;
    public ConfigOptionBase<int> resolutionYconfig;

    public Fullscreen()
    {
        resolutionXconfig = new TextInputConfig<int>(this, "Width", 1280, "The width of the non-fullscreen window.", (i)=>i>0);
        resolutionYconfig = new TextInputConfig<int>(this, "Height", 720, "The height of the non-fullscreen window.", (i)=>i>0);
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
    }

    public override void Update()
    {
        base.Update();
        Enabled = Screen.fullScreen;
        if (!Enabled)
        {
            Resolution res = default;
            res.width = ResolutionX;
            res.height = ResolutionY;
            Screen.SetResolution(res.width, res.height, false);
        }
    }
}
