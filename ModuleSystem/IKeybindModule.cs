using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3DashUtils.ModuleSystem;

public interface IKeybindModule
{
    public List<KeyBindInfo> KeyBinds { get; }
}
