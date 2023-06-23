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
    public static KeyBindInfo menuOpenKeyBind;
    public override string CategoryName => "Hidden";

    public List<KeyBindInfo> KeyBinds { get; private set; } = new();

    public MenuHandler()
    {
        menuOpenKeyBind = new("MenuOpenKey", () => menuOpen.Value = !menuOpen.Value, "The key that opens and closes the utility menu.", KeyCode.Tab);
        KeyBinds.Add(menuOpenKeyBind);
    }

    public override void Update()
    {
        if (_3DashUtils.currentKeybindEditing == null)
        {
            if (MenuHandler.menuOpenKeyBind.KeyBind == KeyCode.None)
            {
                _3DashUtils.menuOpenFallback = true;
                _3DashUtils.EditKey(new(KeyCode.None, (key) => MenuHandler.menuOpenKeyBind.KeyBind = key, "Open Menu"));
                return;
            }
            Extensions.CollectKeyBindInfos().Do((bind) =>
            {
                if (Input.GetKeyDown(bind.KeyBind))
                {
                    bind.KeyCallback();
                }
            });
        }
        if (_3DashUtils.currentKeybindEditing.editingFinished)
        {
            _3DashUtils.currentKeybindEditing = null;
            _3DashUtils.conflicts = null;
            _3DashUtils.menuOpenFallback = false;
        }
    }
}
