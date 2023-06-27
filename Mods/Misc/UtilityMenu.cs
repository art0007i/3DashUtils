using _3DashUtils.ModuleSystem;
using HarmonyLib;
using UnityEngine;

namespace _3DashUtils.Mods.Misc;

public class UtilityMenu : ToggleModule, IKeybindModule
{
    public override string CategoryName => "Misc";

    protected override bool Default => true;

    protected override KeyCode DefaultKey => KeyCode.Tab;

    public override string Description => "Toggles the menu you are currently looking at.";

    public override void Update()
    {
        if (_3DashUtils.currentKeybindEditing == null)
        {
            if (KeyBind == KeyCode.None)
            {
                _3DashUtils.menuOpenFallback = true;
                _3DashUtils.EditKey(new(DefaultKey, (key) => KeyBind = key, "Open Menu"));
                return;
            }
            Extensions.CollectKeyBindInfos().Do((bind) =>
            {
                if (Input.GetKeyDown(bind.KeyBind))
                {
                    bind.KeyCallback();
                }
            });
        }
        else if (_3DashUtils.currentKeybindEditing.editingFinished)
        {
            _3DashUtils.currentKeybindEditing = null;
            _3DashUtils.conflicts = null;
            _3DashUtils.menuOpenFallback = false;
        }
    }
}
