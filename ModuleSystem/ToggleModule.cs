using _3DashUtils.ModuleSystem.Config;
using BepInEx.Configuration;
using System.Collections.Generic;
using UnityEngine;

namespace _3DashUtils.ModuleSystem
{
    public abstract class ToggleModule : ModuleBase, IConfigurableModule, IKeybindModule
    {
        public List<IConfigOption> ConfigOptions { get; private set; } = new();
        public List<KeyBindInfo> KeyBinds { get; private set; } = new();

        private bool settingsOpen;

        public KeyCode KeyBind { get => KeyBindConfigEntry.KeyBind; set => KeyBindConfigEntry.KeyBind = value; }

        protected ConfigEntry<bool> ConfigEntry { get; private set; }
        protected KeyBindInfo KeyBindConfigEntry { get; private set; }

        public bool Enabled { get => ConfigEntry.Value; set { ConfigEntry.Value = value; } }
        protected abstract bool Default { get; }
        protected virtual KeyCode DefaultKey { get => KeyCode.None; }

        public virtual string Description => "";


        public ToggleModule()
        {
            ConfigEntry = _3DashUtils.ConfigFile.Bind(CategoryName, this.GetType().Name, Default, Description);
            KeyBindConfigEntry = new(this.GetType().Name,()=> Enabled = !Enabled, "Keybind for toggling the " + this.GetType().Name + " Module", DefaultKey);
            KeyBinds.Add(KeyBindConfigEntry);
        }

        public override void Start()
        {
            // since mods can do things here I guess it's good to call it
            OnToggle();
        }

        public virtual void OnToggle() { }

        public override void OnGUI()
        {
            GUILayout.BeginHorizontal();
            {
                string tip = null;
                if (Description != null)
                {
                    tip = this.GenerateTooltip(Description);
                }
                var editKeys = Extensions.EditingKeybinds();
                if (editKeys)
                {
                    var text = $"{ModuleName}: <b>" + KeyBind.ToString() + "</b>";
                    if(GUILayout.Button(new GUIContent(text, tip), GUILayout.ExpandWidth(true)))
                    {
                        _3DashUtils.currentKeybindEditing = new(KeyBind, (key) => KeyBind = key, ModuleName);
                    }
                }
                else
                {
                    var text = $"{ModuleName}: " + Extensions.GetEnabledText(Enabled);
                    if (GUILayout.Button(new GUIContent(text, tip), GUILayout.ExpandWidth(true)))
                    {
                        Enabled = !Enabled;
                        OnToggle();
                    }
                }
                if (ConfigOptions.Count > 0)
                {
                    var content = new GUIContent(settingsOpen ? "˃" : "˅", tip);
                    var width = GUIStyles.ConfigButton.CalcHeight(content, 1f);
                    if (GUILayout.Button(content, GUIStyles.ConfigButton, GUILayout.Width(width)))
                    {
                        settingsOpen = !settingsOpen;
                    }
                }
            }
            GUILayout.EndHorizontal();
            // no need to check list size here, because you cannot open the settings
            if (settingsOpen)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(10f);
                GUILayout.BeginVertical(GUIStyles.ConfigLayout);
                this.BuildConfigGUI();
                GUILayout.EndVertical();
                GUILayout.EndHorizontal();
                GUILayout.Space(10f);
            }
        }
    }
}
