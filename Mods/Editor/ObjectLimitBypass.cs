using _3DashUtils.ModuleSystem;
using HarmonyLib;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using TMPro;

namespace _3DashUtils.Mods.Editor;

public class ObjectLimitBypass : ToggleModule
{
    public override string CategoryName => "Editor";

    public override string ModuleName => "Object Limit Bypass";

    public override string Description => "Increases the object limit to 2147483647.";

    protected override bool Default => true;
}

[HarmonyPatch(typeof(FlatEditor))]
public class ObjectLimitPatch
{
    [HarmonyPostfix]
    [HarmonyPatch("UpdateUI")]
    public static void UIPostfix(TextMeshProUGUI ___objectLimitText)
    {
        if (Extensions.Enabled<ObjectLimitBypass>())
        {
            ___objectLimitText.text = "";
        }
    }

    public static MethodInfo totalItemsFunc = AccessTools.Method(typeof(FlatEditor), "GetTotalItems");

    [HarmonyTranspiler]
    [HarmonyPatch("Update")]
    public static IEnumerable<CodeInstruction> UpdateTranspiler(IEnumerable<CodeInstruction> codes)
    {
        foreach (var code in codes)
        {
            if (code.Calls(totalItemsFunc))
            {
                yield return new(OpCodes.Call, typeof(ObjectLimitPatch).GetMethod(nameof(Inject)));   
            }
            else
            {
                yield return code;
            }
        }
    }

    public static int Inject(FlatEditor editor)
    {
        if(Extensions.Enabled<ObjectLimitBypass>())
            return -1;
        return (int)totalItemsFunc.Invoke(editor, null);
    }
}
