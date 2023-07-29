using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static _3DashUtils.UnityScripts.ColorPickerScript;

namespace _3DashUtils.UnityScripts
{
    [RequireComponent(typeof(FlexibleGridLayout))]
    public class IconPickerScript : MonoBehaviour
    {
        public List<GameObject> availableIcons = new();
        public GameObject selectedIconRoot;
        public GameObject iconPrefab;
        public string currentIcon;
        public List<ColorPickerScript> pickerScripts;

        // Start is called before the first frame update
        void Start()
        {
            Time.timeScale = 1;
            RenderSettings.ambientLight = new Color(0.5f,0.5f,0.5f);
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
            Rebuild();

            foreach (var script in pickerScripts)
            {
                StartPickerScript(script);
            }
        }
        public void Rebuild()
        {
            foreach (var go in availableIcons)
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
            var cubePrefabExists = availableIcons.Any((go) => go.name == currentIcon);
            if (!cubePrefabExists)
            {
                // do nothing in case invalid input was provided
                return;
            }
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
            var cubePrefab = availableIcons.FirstOrDefault((go) => go.name == currentIcon);
            var cube = GameObject.Instantiate(cubePrefab, selectedIconRoot.transform);
            RenderInsideUI(cube);
        }

        public void BackButtonPressed()
        {
            SceneManager.LoadScene("Menu");
        }

        void Update()
        {
            foreach (var script in pickerScripts)
            {
                UpdatePickerScript(script);
            }
        }

        public void UpdatePickerScript(ColorPickerScript s)
        {
            s.LastParsedSpeed = float.TryParse(s.rainbowSpeedInput.text, out var spd) ? spd : 0;
            if (s.LastParsedSpeed == 0)
            {
                s.rainbowColor.color = Color.red;
            }
            else
            {
                var rcol = ColorPickerScript.GetRainbowColor(s.isColor2, s.LastParsedSpeed, s.brightnessSlider.value);
                s.rainbowColor.color = rcol;
            }
            if (s.currentColorType == ColorType.Rainbow)
            {
                s.materialToChange?.SetColor("_Color", s.rainbowColor.color);
            }

        }
        public void StartPickerScript(ColorPickerScript s)
        {

            s.KnobVisual = (RectTransform)s.staticColorGrid.GetChild(0);
            var evt = s.staticColorGrid.GetComponent<EventTrigger>();
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.Drag;
            entry.callback.AddListener(s.OnDragging);
            evt.triggers.Add(entry);
            var gridImg = s.staticColorGrid.GetComponent<Image>();
            s.Gradient2DMaterial = new Material(gridImg.material);
            gridImg.material = s.Gradient2DMaterial;
            var entry2 = new Slider.SliderEvent();
            entry2.AddListener((f) => s.UpdatePickedColor());
            s.hueSlider.onValueChanged = entry2;


            EventTrigger.Entry entry3 = new EventTrigger.Entry();
            entry3.eventID = EventTriggerType.Drag;
            entry3.callback.AddListener((f) =>
            {
                var col = new Color(s.sliders[0].value, s.sliders[1].value, s.sliders[2].value);
                s.SetFromColor(col);
            });
            for (var i = 0; i < 3; i++)
            {
                var compSlider = s.sliders[i];
                var evtTrigger = compSlider.GetComponent<EventTrigger>();
                evtTrigger.triggers.Add(entry3);
                var img = compSlider.transform.Find("Background").GetComponent<Image>();
                img.material = new Material(img.material);
                s.SliderMats[i] = img.material;
            }

            var entry4 = new TMP_InputField.SubmitEvent();
            entry4.AddListener((str) =>
            {
                str = str?.Trim();
                if (str == null || str.Length < 6)
                {
                    s.UpdatePickedColor();
                    return;
                }
                var hexStr = new string(str.Where((c) =>
                    ((c >= '0' && c <= '9') ||
                     (c >= 'a' && c <= 'f') ||
                     (c >= 'A' && c <= 'F'))).ToArray());
                if (hexStr.Length < 6)
                {
                    s.UpdatePickedColor();
                    return;
                }
                var r = (float)int.Parse(hexStr.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
                var g = (float)int.Parse(hexStr.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
                var b = (float)int.Parse(hexStr.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
                s.SetFromColor(new Color(r / 255f, g / 255f, b / 255f));
            });
            s.hexCode.onEndEdit = entry4;

            s.SetFromColor(s.pickedColor);
        }
    }
}
