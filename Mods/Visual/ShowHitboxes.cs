using BepInEx.Configuration;
using HarmonyLib;
using System.Linq;
using Gizmos = Popcron.Gizmos;
using UnityEngine;
using Popcron;
using UnityEngine.PlayerLoop;
using _3DashUtils.ModuleSystem;

namespace _3DashUtils.Mods.Visual;

public class ShowHitboxes : ToggleModule
{
    public static ConfigEntry<bool> showHitboxes = _3DashUtils.ConfigFile.Bind("Visual", "ShowHitboxes", false);
    public override ConfigEntry<bool> Enabled => showHitboxes;

    public override string CategoryName => "Visual";

    public override string ModuleName => "Show Hitboxes";

    public override string Tooltip => "Shows hitboxes of all interactable objects.";

    public override void Start()
    {
        Gizmos.Enabled = true;
        Gizmos.CameraFilter += (c) =>
        {
            // main cameras only by default. but this game has no main cams xddd
            return true;
        };
    }

    public override void Update()
    {
        if (!Enabled.Value)
        {
            return;
        }
        Gizmos.Material = _3DashUtils.CustomMaterial;

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

        Object.FindObjectsOfType<PadScript>().SelectMany(p => p.gameObject.GetComponents<Collider>())
            .Do(coll => RenderCollider(coll, new Color(1f, 0.5f, 0f)));

        Object.FindObjectsOfType<PortalScript>().SelectMany(p => p.gameObject.GetComponents<Collider>())
            .Do(coll => RenderCollider(coll, new Color(1f, 0.5f, 0f)));
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
            if (render != null && !render.sharedMaterials.Contains(_3DashUtils.RedMaterial))
            {
                _3DashUtils.Log.LogMessage("converting " + m.gameObject.name);
                render.sharedMaterials = render.sharedMaterials.AddToArray(_3DashUtils.RedMaterial);
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
