using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3DashUtils.ModuleSystem.Config;

public interface IConfigurableModule
{
    List<IConfigOption> ConfigOptions { get; }
}
