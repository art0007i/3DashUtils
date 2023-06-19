using _3DashUtils.ModuleSystem.Config;
using BepInEx.Configuration;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;

namespace _3DashUtils.ModuleSystem
{
    public abstract class ToggleModule : ModuleBase, IConfigurableModule
    {
        List<IConfigOption> IConfigurableModule.ConfigOptions => ConfigOptions;
        private List<IConfigOption> ConfigOptions = new();

        private bool settingsOpen;

        public KeyCode KeyBind { get => KeyBindConfigEntry.Value; set { KeyBindConfigEntry.Value = value; } }

        protected ConfigEntry<bool> ConfigEntry { get; private set; }
        protected ConfigEntry<KeyCode> KeyBindConfigEntry { get; private set; }

        public bool Enabled { get => ConfigEntry.Value; set { ConfigEntry.Value = value; } }
        protected abstract bool Default { get; }
        protected virtual KeyCode DefaultKey { get => KeyCode.None; }

        public virtual string Description => null;


        public ToggleModule()
        {
            ConfigEntry = _3DashUtils.ConfigFile.Bind(CategoryName, this.GetType().Name, Default, Description);
            KeyBindConfigEntry = _3DashUtils.ConfigFile.Bind("Keybinds", this.GetType().Name, DefaultKey, "Keybind for toggling the " + this.GetType().Name + " Module");
        }

        public override void Start()
        {
            // since mods can do things here I guess it's good to call it
            OnToggle();
        }

        public virtual void OnToggle() { }


        private static GUIStyle configButtonStyle;
        private static GUIStyle configLayoutStyle;
        public override void OnGUI()
        {
            GUILayout.BeginHorizontal();
            {
                if(configButtonStyle == null)
                {
                    configButtonStyle = new GUIStyle(GUI.skin.button);
                    configButtonStyle.alignment = TextAnchor.MiddleCenter;
                }
                if (configLayoutStyle == null)
                {
                    // keeping this for when I want to re-style the ui maybe?
                    configLayoutStyle = new GUIStyle(GUI.skin.box);
                }


                var text = $"{ModuleName}: " + Extensions.GetEnabledText(Enabled);
                string tip = null;
                if (Description != null)
                {
                    tip = this.GenerateTooltip(Description);
                }
                if (GUILayout.Button(new GUIContent(text, tip), GUILayout.ExpandWidth(true)))
                {
                    Enabled = !Enabled;
                    OnToggle();
                }
                if(ConfigOptions.Count > 0)
                {
                    var content = new GUIContent(settingsOpen ? "˃" : "˅", tip);
                    var width = configButtonStyle.CalcHeight(content, 1f);
                    if (GUILayout.Button(content, configButtonStyle, GUILayout.Width(width)))
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
                //optionStyle.normal.background = Texture2D.whiteTexture;
                GUILayout.BeginVertical(configLayoutStyle);
                {
                    foreach (var configOp in ConfigOptions)
                    {

                        GUILayout.BeginHorizontal(new GUIContent("", Extensions.GenerateTooltip(configOp)), GUIStyle.none, GUILayout.ExpandHeight(false));
                        GUILayout.Label(configOp.Name, GUILayout.ExpandWidth(true));
                        //GUILayout.BeginHorizontal(GUILayout.Width(50f));
                        configOp.OnGUI();
                        //GUILayout.EndHorizontal();
                        GUILayout.EndHorizontal();
                    }
                }
                GUILayout.EndVertical();
                GUILayout.EndHorizontal();
                GUILayout.Space(10f);
            }
        }
    }
}
