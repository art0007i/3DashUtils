using _3DashUtils.ModuleSystem;
using System.Linq;
using UnityEngine.SceneManagement;

namespace _3DashUtils.Mods.Visual;

internal class HitboxesOnDeath : ToggleModule
{
    public override string CategoryName => "Visual";

    public override string ModuleName => "Hitboxes On Death";

    public override string Description => "Shows hitboxes of all interactable objects on player death.";

    protected override bool Default => false;

    public override void Update()
    {
        if (Extensions.Enabled<ShowHitboxes>() || !Enabled) return;
        var dead = SceneManager.GetActiveScene().GetRootGameObjects().Any(go => go.name.StartsWith("DeathPrefab"));
        if (dead)
        {
            ShowHitboxes.RenderHitboxes();
        }
    }
}

