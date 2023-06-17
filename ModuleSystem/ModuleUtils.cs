using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3DashUtils.ModuleSystem
{
    public class ModuleUtils
    {
        public static string GetEnabledText(bool enabled)
        {
            return enabled ? "<color=green>ON</color>" : "<color=red>OFF</color>";
        }
    }
}
