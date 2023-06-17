using BepInEx.Configuration;
using UnityEngine;

namespace _3DashUtils.ModuleSystem
{
    public abstract class ToggleModule : ModuleBase
    {
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
            var text = $"{ModuleName}: " + ModuleUtils.GetEnabledText(Enabled.Value);
            if (GUILayout.Button(new GUIContent(text, $"<b>{ModuleName}</b>: {Tooltip}")))
            {
                Enabled.Value = !Enabled.Value;
                OnToggle();
            }
        }
    }
}
