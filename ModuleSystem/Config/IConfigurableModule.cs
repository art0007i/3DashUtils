using System.Collections.Generic;
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
        
            foreach (var configOp in module.ConfigOptions)
            {

                GUILayout.BeginHorizontal(new GUIContent("", Extensions.GenerateTooltip(configOp)), GUIStyle.none, GUILayout.ExpandHeight(false));
                GUILayout.Label(configOp.Name, GUILayout.ExpandWidth(true));
                //GUILayout.BeginHorizontal(GUILayout.Width(50f));
                configOp.OnGUI();
                //GUILayout.EndHorizontal();
                GUILayout.EndHorizontal();
            }
        
    }
}
