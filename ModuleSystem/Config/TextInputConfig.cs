using System;
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

    private bool Dummycheck(T t)
    {
        return true;
    }

    /// <summary>
    /// Create a new text input config option.
    /// Only works on supported types.<br/>
    /// Will throw <see cref="ArgumentException"/> if <typeparamref name="T"/> is not one of:
    /// <list type="bullet">
    /// <item><see cref="int"/></item>
    /// <item><see cref="float"/></item>
    /// <item><see cref="double"/></item>
    /// </list>
    /// </summary>
    /// <exception cref="ArgumentException">Thrown whenever the passed parameter <typeparamref name="T"/> is not one of the supported ones.</exception>
    public TextInputConfig(IMenuModule module, string name, T defaultValue, string description, Func<T,bool> parsedValueCheck = null) : base(module, name, defaultValue, description)
    {
        if(!CommonTextParsers.Parsers.TryGetValue(typeof(T), out var func))
        {
            throw new ArgumentException("The passed type parameter T was not one of the supported types.");
        }
        var parser = Extensions.CastDelegate<TryParseWithCheck<T>>(func);
        var check = parsedValueCheck ?? Dummycheck;


        lastText = Value.ToString();
        textTryParser = (string text, out T parse) => parser(text, out parse, check);
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
public delegate bool TryParseWithCheck<T>(string text, out T parse, Func<T,bool> check); 