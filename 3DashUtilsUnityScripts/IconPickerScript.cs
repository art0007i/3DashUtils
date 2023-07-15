using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


#if UNITY_EDITOR
using UnityEditor;
#endif

namespace _3DashUtils.UnityScripts
{
    [RequireComponent(typeof(FlexibleGridLayout))]
    public class IconPickerScript : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            Rebuild();
        }
        public void Rebuild()
        {
            var gos = Resources.LoadAll<GameObject>("Cubes");
            var iconPrefab = Resources.Load<GameObject>("iconPrefab");
            foreach (var go in gos)
            {
                var instance = GameObject.Instantiate(iconPrefab, transform);
                var icon = GameObject.Instantiate(go, instance.transform.GetChild(0));
                icon.layer = LayerMask.NameToLayer("UI");
                foreach (var mesh in icon.GetComponentsInChildren<Renderer>())
                {
                    mesh.gameObject.layer = icon.layer;
                }
                
            }
            /*
            var grid = GetComponent<FlexibleGridLayout>();
            var orig = transform.GetChild(0);
            for (var i = 0; i < (grid.rows * grid.columns) - 1; i++)
            {
                GameObject.Instantiate(orig, orig.parent);
            }
            */
        }

        // Update is called once per frame
        void Update()
        {

        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(IconPickerScript))]
    class PickerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var t = (IconPickerScript)target;
            if (GUILayout.Button("Sync Settings"))
            {
                for (var i = t.transform.childCount - 1; i > 0; i--)
                {
                    Destroy(t.transform.GetChild(i).gameObject);
                }
                t.Rebuild();
            }
        }
    }
#endif
}
