using _3DashUtils.ModuleSystem;
using BepInEx.Configuration;
using UnityEngine;

namespace _3DashUtils.Mods.Hidden;

public class MenuHandler : ModuleBase
{
    public static ConfigEntry<bool> menuOpen = _3DashUtils.ConfigFile.Bind("Main", "MenuOpen", true);
    public static ConfigEntry<KeyCode> menuOpenKey = _3DashUtils.ConfigFile.Bind("Main", "MenuOpenKey", KeyCode.Tab);
    public override string CategoryName => "Hidden";

    public override void Update()
    {
        if (Input.GetKeyDown(menuOpenKey.Value))
        {
            menuOpen.Value = !menuOpen.Value;
        }
    }
}
