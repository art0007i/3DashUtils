using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace _3DashUtils.ModuleSystem;
/// <summary>
/// This is a module which allows you to define several buttons that have a click action and KeyBind
/// </summary>
public abstract class ButtonsModule : ModuleBase, IKeybindModule
{
    public List<KeyBindInfo> KeyBinds { get; protected set; } = new();

    protected abstract List<KeyBindButton> Buttons { get; }

    protected struct KeyBindButton
    {
        public string Name;
        public string Description;
        public KeyBindInfo KeyBind;

        public KeyBindButton(string name, string description, KeyBindInfo keyBind)
        {
            Name = name;
            Description = description;
            KeyBind = keyBind;
        }
    }

    public override void Awake()
    {
        base.Awake();
        Buttons.Do((sh) => KeyBinds.Add(sh.KeyBind));
    }

    private int btnIndex;

    protected void DrawKeybindButton()
    {
        var button = Buttons[btnIndex];
        var keys = Extensions.EditingKeybinds();
        var buttonLabel = button.Name + (keys ? $": <b>{button.KeyBind.KeyBind}</b>" : "");
        var content = new GUIContent(buttonLabel, button.Description);
        if (GUILayout.Button(content))
        {
            if (keys)
            {
                _3DashUtils.EditKey(new(KeyCode.None, (key) => button.KeyBind.KeyBind = key, button.Name + " Shortcut"));
            }
            else
            {
                button.KeyBind.KeyCallback();
            }
        }
        btnIndex++;
    }
    protected void DrawAllKeybindButtons()
    {
        for (int i = 0; i < Buttons.Count; ++i)
        {
            DrawKeybindButton();
        }
    }

    public override void OnGUI()
    {
        base.OnGUI();
        btnIndex = 0;
    }
}
