using BepInEx.Configuration;
using System;

namespace _3DashUtils.ModuleSystem.Config;

public abstract class ConfigOptionBase<T> : IConfigOption
{
    private ConfigEntry<T> entry;
    /// <summary>
    /// Create a new config option.
    /// </summary>
    public ConfigOptionBase(IMenuModule module, string name, T defaultValue, string description)
    {
        Module = module;
        Category = module.CategoryName;
        Name = name;
        Description = description;
        try
        {
            entry = _3DashUtils.ConfigFile.Bind(Category, module.ModuleName.JoinPascalCase() + Name, defaultValue, Description);
        }
        catch (Exception e)
        {
            throw new Exception("Failed to bind config. This is most likely caused by a duplicate key name. (This is a re-throw; check InnerException for original exception)", e);
        }
        if (module is IConfigurableModule c)
        {
            c.ConfigOptions.Add(this);
        }
    }

    public T Value { get => entry.Value; set { entry.Value = value; } }

    public string Name { get; private set; }

    public string Description { get; private set; }

    public string Category { get; private set; }

    public IMenuModule Module { get; private set; }

    public abstract void OnGUI();
}