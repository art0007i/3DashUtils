using _3DashUtils.ModuleSystem;
using HarmonyLib;
using System.Linq;
using UnityEngine;

namespace _3DashUtils.Mods.Player;

public class DieScript : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Hazard"))
        {
            PlayerScript player = FindObjectOfType<PlayerScript>();
            if (player != null)
            {
                player.Die(false);
            }
            else
            {
                player = FindObjectOfType<PlayerScriptEditor>();
                if (player != null)
                {
                    player.Die(false);
                }
            }
        }
    }
}

public class Noclip : ToggleModule
{
    public override string CategoryName => "Player";

    public override string Description => "Prevents the player from dying.";

    public override bool IsCheat => Enabled;

    protected override bool Default => false;

    public override void Update()
    {
        if (Enabled)
        {
            GameObject.FindGameObjectsWithTag("Hazard").SelectMany(go => go.GetComponentsInChildren<Collider>()).Do(coll => coll.isTrigger = true);
        }
    }
    public override void OnToggle()
    {
        if (Enabled == false)
        {
            GameObject.FindGameObjectsWithTag("Hazard").SelectMany(go => go.GetComponentsInChildren<Collider>()).Do(coll => coll.isTrigger = false);
        }
    }

    public bool IsPlayerSafe()
    {
        if(!Enabled) return true;
        PlayerScript player = Object.FindObjectOfType<PlayerScript>();
        if (player)
        {
            BoxCollider collider1 = player.GetComponent<BoxCollider>();
            LayerMask layerMask = LayerMask.NameToLayer("Hazard");

            Collider[] hitColliders = Physics.OverlapBox(player.transform.position + collider1.center, Vector3.Scale(player.transform.lossyScale, collider1.size / 2), player.transform.rotation, layerMask);

            foreach (Collider collider in hitColliders)
            {
                if (collider.tag == "Hazard") return false;
                if (collider.tag == "Wall" && !player.isHedron)
                {
                    float num = 0.25f;
                    num = collider.GetComponent<BoxCollider>().size.y * collider.transform.localScale.y / 2f;

                    Vector3 localPosition = player.transform.localPosition;
                    Vector3 vector = player.transform.parent.transform.InverseTransformPoint(collider.transform.position);
                    float y = localPosition.y;
                    float y2 = vector.y;
                    if (!(y > y2 + num) && !(y < y2 - num))
                    {
                        return false;
                    }
                }
            }

            if (player.transform.localPosition.y > 35f)
            {
                return false;
            }

            if (player.transform.localPosition.y < -15f)
            {
                return false;
            }

            if (player.isHedron && Mathf.Abs(player.transform.localPosition.x) > 15f)
            {
                return false;
            }
        }

        return true;
    }
}

[HarmonyPatch(typeof(PlayerScript), nameof(PlayerScript.Die))]
public class NoClipPatch
{
    public static bool Prefix(bool deathOverride)
    {
        return deathOverride || !Extensions.Enabled<Noclip>();
    }
}

[HarmonyPatch(typeof(PlayerScript), "Awake")]
public class AwakePatch
{
    public static void Postfix(PlayerScript __instance)
    {
        __instance.gameObject.AddComponent<DieScript>();
    }
}

[HarmonyPatch(typeof(PlayerScriptEditor), "Awake")]
public class AwakePatchEditor
{
    public static void Postfix(PlayerScriptEditor __instance)
    {
        __instance.gameObject.AddComponent<DieScript>();
    }
}
