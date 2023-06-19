using _3DashUtils.ModuleSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3DashUtils.Mods.Player;

public class PracticeCheckpointFix : ToggleModule
{
    public override string CategoryName => "Player";

    protected override bool Default => true;

    public override string Description => "Fixes practice checkpoint input buffering and fixes placing checkpoints while dead.";
}
