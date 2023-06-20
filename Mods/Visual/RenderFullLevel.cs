using _3DashUtils.ModuleSystem;
using UnityEngine;

namespace _3DashUtils.Mods.Visual;

internal class RenderFullLevel : ToggleModule
{
    public override string CategoryName => "Visual";
    public override string ModuleName => "Render Full Level";
    public override string Description => "Renders the entire level at the same time.";
    protected override bool Default => false;

    public override void Update()
    {
        GameObject gameObject = GameObject.Find("WorldGenerator");
        if (!(gameObject == null))
        {
            gameObject.GetComponent<WorldGenerator>().renderDistance = Enabled ? float.PositiveInfinity : 14f;
        }
        GameObject gameObject2 = GameObject.Find("WorldGeneratorEditor");
        if (!(gameObject2 == null))
        {
            gameObject2.GetComponent<WorldGeneratorEditor>().renderDistance = Enabled ? float.PositiveInfinity : 14f;
        }
    }
}
