using _3DashUtils.ModuleSystem;
using _3DashUtils.ModuleSystem.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;

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
    /// Generates a string that has the module name followed by the provided tooltip.
    /// </summary>
    public static string GenerateTooltip(this IConfigOption conf)
    {
        return $"<b>{conf.Module.ModuleName} - {conf.Name}</b>: {conf.Description}";
    }

    /// <summary>
    /// The key bind which when pressed will activate key bind editing mode.
    /// I'm so sorry for the variable name.
    /// </summary>
    public static KeyCode keyBindEditKeyBind = KeyCode.LeftShift;

    /// <summary>
    /// Returns true if the user is in keybind editing mode.
    /// Use this function to add behavior to allow editing binds.
    /// </summary>
    /// <returns></returns>
    public static bool EditingKeybinds()
    {
        // will be used for config system.
        return _3DashUtils.currentKeybindEditing is KeyBindEditInfo || Input.GetKey(keyBindEditKeyBind);
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
    /// <summary>
    /// Gets the instance for a given module. Useful when you need it inside of a static context.
    /// </summary>
    public static T GetModule<T>() where T : IMenuModule
    {
        return (T)_3DashUtils.moduleList.First((v) => { return typeof(T).IsAssignableFrom(v.GetType()); });
    }

    /// <summary>
    /// Checks if a toggle module is enabled. Useful when you need it inside of a static context.
    /// </summary>
    public static bool Enabled<T>() where T : ToggleModule
    {
        return GetModule<T>().Enabled;
    } 

    /// <summary>
    /// Converts a PascalCase string to insert spaces between the capital letters.
    /// <br/>
    /// <example>
    /// For example:
    /// <code>
    /// SplitCamelCase("IBMMakeStuffAndSellIt");
    /// </code>
    /// results in <c>IBM Make Stuff And Sell It</c>
    /// </example>
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static string SplitPascalCase(this string str)
    {
        return Regex.Replace(
            Regex.Replace(
                str,
                @"(\P{Ll})(\P{Ll}\p{Ll})",
                "$1 $2"
            ),
            @"(\p{Ll})(\P{Ll})",
            "$1 $2"
        );
    }

    /// <summary>
    /// Converts a string with spaces to pascal case. Also removes all special characters.
    /// <br/>
    /// <example>
    /// For example:
    /// <code>
    /// JoinPascalCase("epic.gaming moment!");
    /// </code>
    /// results in <c>EpicGamingMoment</c>
    /// </example>
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static string JoinPascalCase(this string str)
    {
        str = str.Trim();
        var output = "";
        bool space = false;
        for (int i = 0; i < str.Length; i++)
        {
            char c = str[i];
            if (char.IsWhiteSpace(c) || !char.IsLetterOrDigit(c))
            {
                space = true;
                continue;
            }
            if (space || i == 0)
            {
                // invariant??? in what case is the output of ToUpper different to ToUpperInvariant
                // either way I'm using the invariant one because this should be culture independant
                output += char.ToUpperInvariant(c);
                space = false;
            }
            else
            {
                output += c;
            }
        }
        return output;
    }

    public static string GetPluginDataPath()
    {
        return Path.GetDirectoryName(typeof(_3DashUtils).Assembly.Location);
    }
    /// <summary>
    /// Finds a GameObject with name <paramref name="name"/> and gets the component of type <typeparamref name="T"/> from it.
    /// </summary>
    public static T Find<T>(string name) where T : Component
    {
        return GameObject.Find(name).GetComponent<T>();
    }
    /// <summary>
    /// Finds a GameObject with name same as class <typeparamref name="T"/> and gets the component of type <typeparamref name="T"/> from it.
    /// </summary>
    public static T FindSameName<T>() where T : Component
    {
        var name = typeof(T).Name;
        return GameObject.Find(name).GetComponent<T>();
    }
    /// <summary>
    /// Finds a GameObject with tag <paramref name="tag"/> and gets the component of type <typeparamref name="T"/> from it.
    /// </summary>
    public static T FindByTag<T>(string tag) where T : Component
    {
        return GameObject.FindGameObjectWithTag(tag).GetComponent<T>();
    }

    public static T CastDelegate<T>(Delegate d) where T : Delegate
    {
        return (T)d.Method.CreateDelegate(typeof(T));
    }

    /// <summary>
    /// Creates a new rect of size <paramref name="size"/> in the middle of the game window.
    /// </summary>
    public static Rect GetMiddleOfScreenRect(Vector2 size)
    {
        Vector2 screnSize = new(Screen.width, Screen.height);
        var screenPos = (screnSize - size) / 2;
        return new Rect(screenPos, size);
    }

    /// <summary>
    /// Returns an IEnumerable that contains KeyBindInfos of every single module.
    /// </summary>
    public static IEnumerable<KeyBindInfo> CollectKeyBindInfos()
    {
        return _3DashUtils.moduleList.OfType<IKeybindModule>().SelectMany((k) => k.KeyBinds);
    }
}
