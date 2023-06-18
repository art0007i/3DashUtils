using BepInEx.Configuration;
using System.Reflection;
using UnityEngine;

namespace _3DashUtils.ModuleSystem
{
    public abstract class ToggleModule : ModuleBase
    {
        public virtual KeyCode KeyBind => KeyCode.None;

        public abstract ConfigEntry<bool> Enabled { get; }

        public virtual string Tooltip => null;

        public override void Start()
        {
            // since mods can do shit here I guess it's good
            OnToggle();
        }

        public virtual void OnToggle() { }

        public override void OnGUI()
        {
            var text = $"{ModuleName}: " + Extensions.GetEnabledText(Enabled.Value);
            string tip = null;
            if (Tooltip != null)
            {
                tip = this.GenerateTooltip(Tooltip);
            }
            if (GUILayout.Button(new GUIContent(text, tip)))
            {
                Enabled.Value = !Enabled.Value;
                OnToggle();
            }
        }
    }
}
