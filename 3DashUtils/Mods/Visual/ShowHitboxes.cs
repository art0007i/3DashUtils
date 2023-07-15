using HarmonyLib;
using System.Linq;
using Gizmos = Popcron.Gizmos;
using UnityEngine;
using _3DashUtils.ModuleSystem;
using UnityEngine.SceneManagement;
using _3DashUtils.ModuleSystem.Config;

namespace _3DashUtils.Mods.Visual;

public class ShowHitboxes : ToggleModule
{
    public override string CategoryName => "Visual";

    public override string ModuleName => "Show Hitboxes";

    public override string Description => "Shows hitboxes of all interactable objects.";

    protected override bool Default => false;


    internal static Material CustomMaterial;
    internal static Material RedMaterial;
    public static float Opacity => opactiyOption.Value;
    private static ConfigOptionBase<float> opactiyOption;

    public ShowHitboxes()
    {
        var niceMaterial = _3DashUtils.UtilsAssetsBundle.LoadAsset<Material>("Utils/VColMat.mat");
        niceMaterial.SetFloat("_Alpha", .69f);
        var redMat = new Material(niceMaterial);
        redMat.SetColor("_Color", Color.red);
        CustomMaterial = niceMaterial;
        RedMaterial = redMat;

        opactiyOption = new SliderConfig<float>(this, "Opacity", 0.75f, "Controls how opaque the hitboxes will be (0 means invisible).", 0, 1);
    }

    public override void Start()
    {
        Gizmos.Enabled = true;
        Gizmos.FrustumCulling = true;
        Gizmos.CameraFilter += (c) =>
        {
            // main cameras only by default. but this game has no main cams ?
            return true;
        };
    }

    public static void RenderHitboxes()
    {
        CustomMaterial.SetFloat("_Alpha", Opacity);
        RedMaterial.SetFloat("_Alpha", Opacity);

        Gizmos.Material = CustomMaterial;

        GameObject.FindGameObjectsWithTag("Player").SelectMany(p => p.GetComponents<PlayerScript>())
            .Do(player =>
            {
                var col = Color.green;
                if (player.isWave)
                    player.gameObject.GetComponents<Collider>().Where(c => c != player.CubeCollider)
                    .Do(c => RenderCollider(c, col));
                else
                {
                    RenderCollider(player.CubeCollider, col);
                }
            });

        GameObject.FindGameObjectsWithTag("Wall").SelectMany(p => p.GetComponents<Collider>())
            .Do(coll => RenderCollider(coll, Color.blue));

        GameObject.FindGameObjectsWithTag("Hazard").SelectMany(p => p.GetComponents<Collider>())
            .Do(coll => RenderCollider(coll, Color.red));

        GameObject.FindGameObjectsWithTag("Orb").SelectMany(p => p.GetComponents<Collider>())
            .Do(coll => RenderCollider(coll, new Color(1f, 0.5f, 0f)));

        // this works until they rename the pads and portals
        SceneManager.GetActiveScene().GetRootGameObjects()
            .Where(go => go.name.Contains("Pad") || go.name.Contains("Portal"))
            .SelectMany(go => go.GetComponentsInChildren<Collider>())
            .Do((coll) => RenderCollider(coll, new Color(1, 0.5f, 0)));

    }

    public override void Update()
    {
        if (!Enabled) return;
        RenderHitboxes();
    }

    public static void RenderCollider(Collider c, Color col)
    {
        col.a = 0.5f;
        if (c is BoxCollider b)
        {
            var size = Vector3.Scale(b.size, c.transform.lossyScale);
            var pos = c.transform.TransformPoint(b.center);
            Gizmos.Cube(pos, c.transform.rotation, size, col);
        }
        else if (c is SphereCollider s)
        {
            var pos = c.transform.TransformPoint(s.center);
            Gizmos.Sphere(pos, s.radius * GetAverageScale(c.transform), col, false, 6);
        }
        else if (c is MeshCollider m)
        {
            var render = m.gameObject.GetComponent<MeshRenderer>();
            if (render != null && !render.sharedMaterials.Contains(RedMaterial))
            {
                render.sharedMaterials = render.sharedMaterials.AddToArray(RedMaterial);
            }
        }
    }

    public static float GetAverageScale(Transform t)
    {
        var scl = t.lossyScale;
        var avgUniformScale = (scl.x + scl.y + scl.z) / 3f;
        return avgUniformScale;
    }
}
