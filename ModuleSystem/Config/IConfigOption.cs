using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3DashUtils.ModuleSystem.Config;
public interface IConfigOption
{
    /// <summary>
    /// The module this config option belongs to.
    /// </summary>
    public abstract IMenuModule Module { get; }
    /// <summary>
    /// The name of the config option.
    /// </summary>
    public abstract string Name { get; }
    /// <summary>
    /// The description of the config option.
    /// </summary>
    public abstract string Description { get; }
    /// <summary>
    /// Used for BepInEx config. Usually you should just pass whatever the module's category is.
    /// </summary>
    public abstract string Category { get; }
    /// <summary>
    /// Function used to make the config option work.
    /// This should only create a small control such as a slider or checkbox, and use it to change the config value.
    /// </summary>
    public abstract void OnGUI();
}