using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace _3DashUtils.ModuleSystem;

public abstract class TextEditorModule<T> : ToggleModule
{
    public abstract ConfigEntry<T> Value { get; }

    internal string lastText;
    public abstract bool TryParseText(string text, out T parse);

    public override void OnGUI()
    {
        GUILayout.BeginHorizontal();
        {
            lastText = GUILayout.TextField(lastText, GUILayout.Width(50));
            if(TryParseText(lastText, out var i))
            {
                Value.Value = i;
            }
            // button
            base.OnGUI();
        }
        GUILayout.EndHorizontal();
    }
}
