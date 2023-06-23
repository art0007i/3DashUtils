using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace _3DashUtils.ModuleSystem;

public class KeyBindInfo
{
    public Action KeyCallback;
    public string Name;
    public string Description;
    public ConfigEntry<KeyCode> keyBindOption;
    public KeyCode KeyBind { get => keyBindOption.Value; set { keyBindOption.Value = value; } }

    public KeyBindInfo(string name, Action keyCallback, string description = "", KeyCode defaultKey = KeyCode.None)
    {
        KeyCallback = keyCallback;
        Name = name;
        Description = description;
        keyBindOption = _3DashUtils.ConfigFile.Bind<KeyCode>("Keybinds", name, KeyCode.None, description);
    }
}
