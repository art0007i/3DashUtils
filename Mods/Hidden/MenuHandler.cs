using _3DashUtils.ModuleSystem;
using _3DashUtils.ModuleSystem.Config;
using BepInEx.Configuration;
using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace _3DashUtils.Mods.Hidden;

public class MenuHandler : ModuleBase, IKeybindModule
{
    public static ConfigEntry<bool> menuOpen = _3DashUtils.ConfigFile.Bind("Main", "MenuOpen", true);
    public override string CategoryName => "Hidden";

    public List<KeyBindInfo> KeyBinds { get; private set; } = new();

    public MenuHandler()
    {
        KeyBinds.Add(new("MenuOpenKey", () => menuOpen.Value = !menuOpen.Value, "The key that opens and closes the utility menu.", KeyCode.Tab));
    }

    public override void Update()
    {
        if(_3DashUtils.currentKeybindEditing is KeyBindEditInfo i && i.editingFinished)
        {
            _3DashUtils.currentKeybindEditing = null;
            return;
        }
        _3DashUtils.moduleList.OfType<IKeybindModule>().SelectMany((k) => k.KeyBinds).Do((bind) =>
        {
            if (Input.GetKeyDown(bind.KeyBind))
            {
                bind.KeyCallback();
            }
        });
    }
}
