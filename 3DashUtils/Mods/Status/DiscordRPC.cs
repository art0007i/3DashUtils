using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _3DashUtils.ModuleSystem;

namespace _3DashUtils.Mods.Status;

internal class DiscordRPC : ToggleModule
{
    public override string CategoryName => "Status";

    protected override bool Default => false;

}
