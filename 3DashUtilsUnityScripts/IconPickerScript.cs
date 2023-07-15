using System.Collections.Generic;
using System.Linq;
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
        private List<string> availableIcons;
        public GameObject selectedIconRoot;
        public string currentIcon;

        // Start is called before the first frame update
        void Start()
        {
            Rebuild();
        }
        public void Rebuild()
        {
            var gos = Resources.LoadAll<GameObject>("Cubes");
            availableIcons = gos.Select((g) => g.name).ToList();
            currentIcon = availableIcons[0];
            var iconPrefab = Resources.Load<GameObject>("iconPrefab");
            foreach (var go in gos)
            {
                var instance = GameObject.Instantiate(iconPrefab, transform);
                var btn = instance.GetComponent<Button>();
                var evt = new Button.ButtonClickedEvent();
                var cachedName = go.name;
                evt.AddListener(() => { UpdateCurrentIcon(cachedName); });
                evt.AddListener(() => Debug.Log("clicked " + cachedName));
                btn.onClick = evt;
                var icon = GameObject.Instantiate(go, instance.transform.GetChild(0));
                RenderInsideUI(icon);
            }
            LoadCurrentIcon();
            /*
            var grid = GetComponent<FlexibleGridLayout>();
            var orig = transform.GetChild(0);
            for (var i = 0; i < (grid.rows * grid.columns) - 1; i++)
            {
                GameObject.Instantiate(orig, orig.parent);
            }
            */
        }

        private static void RenderInsideUI(GameObject go)
        {
            go.layer = LayerMask.NameToLayer("UI");
            foreach (var mesh in go.GetComponentsInChildren<Renderer>())
            {
                mesh.gameObject.layer = go.layer;
            }
        }

        public void UpdateCurrentIcon(string name)
        {
            currentIcon = name;
            LoadCurrentIcon();
        }
        public void LoadCurrentIcon()
        {
            // just in case delete ALL children
            for (int i = selectedIconRoot.transform.childCount - 1; i >= 0; i--)
            {
                Destroy(selectedIconRoot.transform.GetChild(i).gameObject);
            }

            var cubePrefab = Resources.Load<GameObject>("Cubes/" + currentIcon);
            var cube = GameObject.Instantiate(cubePrefab, selectedIconRoot.transform);
            RenderInsideUI(cube);
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
