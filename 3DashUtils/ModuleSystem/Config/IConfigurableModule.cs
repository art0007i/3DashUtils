using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace _3DashUtils.ModuleSystem.Config;

public interface IConfigurableModule
{
    public List<IConfigOption> ConfigOptions { get; }
}

public static class ConfigurableModuleExtensions
{
    public static void BuildConfigGUI(this IConfigurableModule module)
    {
        var style = GUI.skin.label;
        var maxWidth = module.ConfigOptions.Select((o) => style.CalcSize(new GUIContent(o.Name)).x).Max();

        foreach (var configOp in module.ConfigOptions)
        {
            GUILayout.BeginHorizontal(new GUIContent("", Extensions.GenerateTooltip(configOp)), GUIStyle.none, GUILayout.ExpandHeight(false));
            GUILayout.Label(configOp.Name, GUILayout.Width(maxWidth));
            //GUILayout.BeginHorizontal(GUILayout.Width(50f));
            configOp.OnGUI();
            //GUILayout.EndHorizontal();
            GUILayout.EndHorizontal();
        }

    }
}
