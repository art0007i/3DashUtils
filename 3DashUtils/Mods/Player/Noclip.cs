using _3DashUtils.Mods.Status;
using _3DashUtils.ModuleSystem;
using BepInEx;
using HarmonyLib;
using System;
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
            GameObject.FindGameObjectsWithTag("Hazard").Do(go =>
            {
                go.GetComponentsInChildren<Collider>().Do(coll => coll.isTrigger = true);
            });
        }
    }
    public override void OnToggle()
    {
        if (Enabled == false)
        {
            GameObject.FindGameObjectsWithTag("Hazard").Do(go =>
            {
                
                go.GetComponentsInChildren<Collider>().Do(coll => coll.isTrigger = false);
            });
            
        }
    }

    public bool IsPlayerSafe()
    {
        if(!Enabled) return true;
        PlayerScript player = UnityEngine.Object.FindObjectOfType<PlayerScript>();
        if (player)
        {
            BoxCollider collider1 = player.GetComponent<BoxCollider>();
            LayerMask layerMask = LayerMask.NameToLayer("Hazard");

            Collider[] hitColliders = Physics.OverlapBox(player.transform.position + collider1.center, Vector3.Scale(player.transform.lossyScale, collider1.size / 2), player.transform.rotation, layerMask);

            foreach (Collider collider in hitColliders)
            {
                if (collider.tag == "Wall" || collider.tag == "Hazard") return false;
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
