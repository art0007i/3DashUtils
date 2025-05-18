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
        public bool isColor2;
        public ColorType currentColorType = ColorType.Static;
        public GameObject[] typePanels = new GameObject[3];

        public RectTransform staticColorGrid;
        private RectTransform knobVisual;
        public RectTransform KnobVisual { get => knobVisual; set => knobVisual = value; }
        public Slider hueSlider;
        private Vector3[] cornerCache = new Vector3[4];
        private Material gradient2Dmaterial;
        public Material Gradient2DMaterial { get => gradient2Dmaterial; set => gradient2Dmaterial = value; }
        public Image previewImage;
        public Color pickedColor;
        public Material materialToChange;
        public Slider[] sliders = new Slider[3];
        public TMP_InputField hexCode;
        private Material[] sliderMats = new Material[3];
        public Material[] SliderMats { get => sliderMats; set => sliderMats = value; }


        public TMP_InputField rainbowSpeedInput;
        private float lastParsedSpeed = 0;
        public float LastParsedSpeed { get => lastParsedSpeed; set => lastParsedSpeed = value; }
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
            if(currentColorType == ColorType.Static)
            {
                // reset after rainbow
                materialToChange?.SetColor("_Color", pickedColor);
            }
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

            materialToChange?.SetColor("_Color", pickedColor);

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

        public static Color GetRainbowColor(bool isColor2, float speed, float brightness)
        {
            var rainbowProgress = (speed / 10 * Time.timeSinceLevelLoad) + (isColor2 ? .5f : 0);
            return Color.HSVToRGB(Mathf.Repeat(rainbowProgress, 1), 1, brightness);
        }
    }
}
