using _3DashUtils.ModuleSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace _3DashUtils;

public static class Extensions
{
    /// <summary>
    /// Generates a red OFF text or green ON text depending on the passed value.
    /// </summary>
    public static string GetEnabledText(bool enabled)
    {
        return enabled ? "<color=green>ON</color>" : "<color=red>OFF</color>";
    }

    /// <summary>
    /// Generates a string that has the module name followed by the provided tooltip.
    /// </summary>
    public static string GenerateTooltip(this IMenuModule mod, string tip)
    {
        return $"<b>{mod.ModuleName}</b>: {tip}";
    }

    /// <summary>
    /// Returns true if the game is currently paused.
    /// <br/>
    /// Also returns false if the reflection somehow failed.
    /// </summary>
    public static bool GetPauseState()
    {
        FieldInfo field = typeof(PauseMenuManager).GetField("paused", BindingFlags.Static | BindingFlags.Public);
        if (field != null)
        {
            return (bool)field.GetValue(null);
        }
        return false;
    }
    /// <summary>
    /// Returns true whenever any cheating modules are enabled.
    /// </summary>
    public static bool CheatsEnabled()
    {
        return _3DashUtils.moduleList.Any((m) => m.IsCheat);
    }

    public static T GetModule<T>() where T : IMenuModule
    {
        return (T)_3DashUtils.moduleList.Find((v) => { return typeof(T).IsAssignableFrom(v.GetType()); });
    }
}
