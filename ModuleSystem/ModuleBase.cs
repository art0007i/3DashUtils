using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3DashUtils.ModuleSystem;

public abstract class ModuleBase : IMenuModule
{
    public abstract string CategoryName { get; }

    public virtual string ModuleName { get { return GetType().Name; } }

    public virtual float Priority => 0;

    public virtual void Awake()
    {
    }

    public virtual void OnGUI()
    {
    }

    public virtual void Start()
    {
    }

    public virtual void Update()
    {
    }
}
