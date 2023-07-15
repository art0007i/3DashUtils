using _3DashUtils.ModuleSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3DashUtils.Mods.Status;

internal class Status : ToggleModule
{
    public override string CategoryName => "Status";

    protected override bool Default => false;
}
