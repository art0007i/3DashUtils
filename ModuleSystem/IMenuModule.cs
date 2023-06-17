using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3DashUtils.ModuleSystem
{
    /// <summary>
    /// The interface class for modules. Usually you want to extend the <see cref="ModuleBase"/> when making your own module.
    /// </summary>
    public interface IMenuModule
    {
        /// <summary>
        /// Called whenever the equivalent unity function gets called.
        /// </summary>
        public abstract void Awake();
        /// <summary>
        /// Called whenever the equivalent unity function gets called.
        /// </summary>
        public abstract void Start();
        /// <summary>
        /// Called whenever the equivalent unity function gets called.
        /// </summary>
        public abstract void Update();
        /// <summary>
        /// Called whenever the equivalent unity function gets called.
        /// <br/>
        /// This function will be ran inside of a window along other modules with the same <see cref="CategoryName"/>.
        /// </summary>
        public abstract void OnGUI();
        /// <summary>
        /// The name of the category of this module.
        /// Each unique category will create a separate window, except for '<b>Hidden</b>' which is ignored.
        /// </summary>
        public abstract string CategoryName { get; }
        public abstract string ModuleName { get; }
        /// <summary>
        /// Higher numbers position the module higher in the list.
        /// </summary>
        public abstract float Priority { get; }
    }
}
