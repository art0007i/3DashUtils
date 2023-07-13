#if BEPINEX
using BepInEx.Configuration;
#elif MELON
using MelonLoader;
using UnityEngine;
#endif

namespace _3DashUtils.Compat;

public class ConfigWrapper<T>
{
    public T Value { get => entry.Value; set => entry.Value = value; }
#if BEPINEX
    private ConfigEntry<T> entry;
#elif MELON
    MelonPreferences_Entry<T> entry;
#endif

    public ConfigWrapper(string category, string name, T defaultValue, string description = "")
    {
        // I like BepInEx config api
#if BEPINEX
        entry = _3DashUtils.ConfigFile.Bind(category, name, defaultValue, description);
#elif MELON
        var cat = MelonPreferences.GetCategory(category);
        if (cat == null)
        {
            cat = MelonPreferences.CreateCategory(category);
        }
        entry = cat.CreateEntry(name.JoinPascalCase(), defaultValue, name, description);
        // idk melon default value wack
        if(entry.Value is KeyCode c && name == "UtilityMenu" && c == KeyCode.None)
        {
            entry.Value = (T)(object)KeyCode.Tab;
        }
#endif
    }
}
