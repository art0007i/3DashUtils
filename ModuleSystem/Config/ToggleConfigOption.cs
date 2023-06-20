using UnityEngine;

namespace _3DashUtils.ModuleSystem.Config;

public class ToggleConfigOption : ConfigOptionBase<bool>
{
    /// <summary>
    /// Create a new toggle config option.
    /// </summary>
    public ToggleConfigOption(IMenuModule module, string name, bool defaultValue, string description) : base(module, name, defaultValue, description)
    {
    }

    public override void OnGUI()
    {
        Value = GUILayout.Toggle(Value, "");
    }
}
