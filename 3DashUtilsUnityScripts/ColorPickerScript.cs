using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace _3DashUtils.UnityScripts
{
    public class ColorPickerScript : MonoBehaviour
    {
        public enum ColorType
        {
            Static,
            Rainbow,
            Gradient, // Gradient fading back and forth / looping, I would need to program a gradient editor and rn I don't care. the color picker itself was annoying enough :C
        }

        public ColorType currentColorType = ColorType.Static;
        public GameObject[] typePanels = new GameObject[3];

        public RectTransform staticColorGrid;
        private RectTransform knobVisual;
        public Slider hueSlider;
        private Vector3[] cornerCache = new Vector3[4];
        private Material gradient2Dmaterial;
        public Image previewImage;
        public Color pickedColor;
        public Slider[] sliders = new Slider[3];
        public TMP_InputField hexCode;
        private Material[] sliderMats = new Material[3];


        public TMP_InputField rainbowSpeedInput;
        private float lastParsedSpeed = 0;
        public Image rainbowColor;
        public Slider brightnessSlider;

        public void ChangeColorType(int type)
        {
            foreach (var panel in typePanels)
            {
                panel?.SetActive(false);
            }
            typePanels[type]?.SetActive(true);
            currentColorType = (ColorType)type;
        }

        public void OnDragging(BaseEventData eventData)
        {
            if (eventData is PointerEventData ed)
            {
                if (!ed.pointerCurrentRaycast.isValid) return;
                var screenPoint = ed.pointerCurrentRaycast.worldPosition;
                staticColorGrid.GetWorldCorners(cornerCache);
                var target = remap(cornerCache[0], cornerCache[2], Vector2.zero, Vector2.one, screenPoint);
                target = new Vector2(Mathf.Clamp01(target.x), Mathf.Clamp01(target.y));
                knobVisual.anchorMax = target;
                knobVisual.anchorMin = knobVisual.anchorMax;
                UpdatePickedColor();
            }
        }

        Vector2 remap(Vector2 InMin, Vector2 InMax, Vector2 OutMin, Vector2 OutMax, Vector2 Value)
        {
            return OutMin + (Value - InMin) * (OutMax - OutMin) / (InMax - InMin);
        }

        public void UpdatePickedColor()
        {
            pickedColor = Color.HSVToRGB(hueSlider.value, knobVisual.anchorMax.x, knobVisual.anchorMax.y);
            gradient2Dmaterial.SetColor("_Color", Color.HSVToRGB(hueSlider.value, 1, 1));
            previewImage.color = pickedColor;
            sliders[0].value = pickedColor.r;
            sliderMats[0].SetColor("_ColorFrom", new Color(0, pickedColor.g, pickedColor.b));
            sliderMats[0].SetColor("_ColorTo", new Color(1, pickedColor.g, pickedColor.b));
            sliders[1].value = pickedColor.g;
            sliderMats[1].SetColor("_ColorFrom", new Color(pickedColor.r, 0, pickedColor.b));
            sliderMats[1].SetColor("_ColorTo", new Color(pickedColor.r, 1, pickedColor.b));
            sliders[2].value = pickedColor.b;
            sliderMats[2].SetColor("_ColorFrom", new Color(pickedColor.r, pickedColor.g, 0));
            sliderMats[2].SetColor("_ColorTo", new Color(pickedColor.r, pickedColor.g, 1));

            var hex = ((byte)(pickedColor.r * 255)).ToString("X2") +
                ((byte)(pickedColor.g * 255)).ToString("X2") +
                ((byte)(pickedColor.b * 255)).ToString("X2");
            hexCode.text = hex;
        }

        public void SetFromColor(Color col)
        {
            pickedColor = col;
            Color.RGBToHSV(col, out var h, out var s, out var v);
            knobVisual.anchorMax = new Vector2(s, v);
            knobVisual.anchorMin = knobVisual.anchorMax;
            hueSlider.value = h;
            UpdatePickedColor();
        }

        // Start is called before the first frame update
        void Start()
        {
            knobVisual = (RectTransform)staticColorGrid.GetChild(0);
            var evt = staticColorGrid.GetComponent<EventTrigger>();
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.Drag;
            entry.callback.AddListener(OnDragging);
            evt.triggers.Add(entry);
            var gridImg = staticColorGrid.GetComponent<Image>();
            gradient2Dmaterial = new Material(gridImg.material);
            gridImg.material = gradient2Dmaterial;
            var entry2 = new Slider.SliderEvent();
            entry2.AddListener((f) => UpdatePickedColor());
            hueSlider.onValueChanged = entry2;


            EventTrigger.Entry entry3 = new EventTrigger.Entry();
            entry3.eventID = EventTriggerType.Drag;
            entry3.callback.AddListener((f) =>
            {
                var col = new Color(sliders[0].value, sliders[1].value, sliders[2].value);
                SetFromColor(col);
            });
            for (var i = 0; i < 3; i++)
            {
                var compSlider = sliders[i];
                var evtTrigger = compSlider.GetComponent<EventTrigger>();
                evtTrigger.triggers.Add(entry3);
                var img = compSlider.transform.Find("Background").GetComponent<Image>();
                img.material = new Material(img.material);
                sliderMats[i] = img.material;

            }

            var entry4 = new TMP_InputField.SubmitEvent();
            entry4.AddListener((str) =>
            {
                str = str?.Trim();
                if (str == null || str.Length < 6)
                {
                    UpdatePickedColor();
                    return;
                }
                var hexStr = new string(str.Where((c) =>
                    ((c >= '0' && c <= '9') ||
                     (c >= 'a' && c <= 'f') ||
                     (c >= 'A' && c <= 'F'))).ToArray());
                if (hexStr.Length < 6)
                {
                    UpdatePickedColor();
                    return;
                }
                var r = (float)int.Parse(hexStr.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
                var g = (float)int.Parse(hexStr.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
                var b = (float)int.Parse(hexStr.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
                SetFromColor(new Color(r / 255f, g / 255f, b / 255f));
            });
            hexCode.onEndEdit = entry4;

            SetFromColor(pickedColor);
        }

        // Update is called once per frame
        void Update()
        {
            lastParsedSpeed = float.TryParse(rainbowSpeedInput.text, out var spd) ? spd : 0;
            if (lastParsedSpeed == 0)
            {
                rainbowColor.color = Color.red;
            }
            else
            {
                Color.RGBToHSV(rainbowColor.color, out var h, out var s, out var v);
                rainbowColor.color = Color.HSVToRGB(Mathf.Repeat(h + lastParsedSpeed / 10 * Time.deltaTime, 1), 1, brightnessSlider.value);
            }
        }
    }
}
