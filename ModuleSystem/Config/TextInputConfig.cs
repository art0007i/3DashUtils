using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace _3DashUtils.ModuleSystem.Config;

class TextInputConfig<T> : ConfigOptionBase<T>
{
    /// <summary>
    /// Create a new text input config option.
    /// </summary>
    public TextInputConfig(IMenuModule module, string name, T defaultValue, string description, TryParse<T> parseTextFunction) : base(module, name, defaultValue, description)
    {
        lastText = Value.ToString();
        textTryParser = parseTextFunction;
    }

    internal string lastText;

    internal TryParse<T> textTryParser;

    public override void OnGUI()
    {
        var newText = GUILayout.TextField(lastText);
        if (textTryParser(newText, out var i))
        {
            Value = i;
            lastText = newText;
        }
    }
}


public delegate bool TryParse<T>(string text, out T parse); 