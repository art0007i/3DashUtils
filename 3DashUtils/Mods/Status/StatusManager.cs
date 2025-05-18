using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _3DashUtils.ModuleSystem;
using _3DashUtils.ModuleSystem.Config;
using UnityEngine;

namespace _3DashUtils.Mods.Status;
public class StatusManager : ToggleModule
{
    public override string CategoryName => "Status";
    public override string ModuleName => "Visible";
    public override float Priority => 1;
    protected override bool Default => true;

    public static ConfigOptionBase<string> colorHex;
    public static string ColorHex => colorHex.Value;

    public static ConfigOptionBase<int> fontSizeValue;
    public static int FontSizeValue => fontSizeValue.Value;

    public static ConfigOptionBase<int> spacingValue;
    public static int SpacingValue => spacingValue.Value;

    public static ConfigOptionBase<PositionEnum> selectedPosition;
    public static PositionEnum SelectedPosition => selectedPosition.Value;

    public StatusManager()
    {
        colorHex = new TextInputConfig<string>(this, "Color Hex Code", "#FFFFFF", "The hexadecimal color code for the status text.", (v) =>
        {
            if (!v.StartsWith("#")) return false;
            if (v.Length == 7)
            {
                for (int i = 1; i < v.Length; i++)
                {
                    char c = v[i];
                    if (!"0123456789ABCDEFabcdef".Contains(c)) return false;
                }
                return true;
            }

            return false;
        });

        fontSizeValue = new TextInputConfig<int>(this, "Font Size", 30, "The font size for the status text.", (v) => v > 0);

        spacingValue = new TextInputConfig<int>(this, "Spacing", 5, "The spacing for the status text.", (v) => v > 0);

        selectedPosition = new EnumSelectorConfigOption<PositionEnum>(this, "Position", PositionEnum.TopLeft, "");
        ((EnumSelectorConfigOption<PositionEnum>)selectedPosition).Changed = PositionChanged;
         
    }
    
    private void PositionChanged(PositionEnum selected_position)
    {
        position = selected_position;
        repositionLabels();
    }

    public override void Update()
    {
        base.Update();
        Color color1 = ColorUtility.TryParseHtmlString(ColorHex, out Color parsedColor) ? parsedColor : Color.white;
        if(color != color1)
        {
            color = color1;
            repositionLabels();
        }

        if(fontSize != FontSizeValue)
        {
            fontSize = FontSizeValue;
            repositionLabels();
        }
         
        if(spacing != SpacingValue)
        {
            spacing = SpacingValue;
            repositionLabels();
        }
    }

    private int spacing = 5;
    private List<TemplateLabel> labels = new List<TemplateLabel>();

    private Color color = Color.white;
    public Color Color
    {
        get { return color; }
        set { color = value; }
    }

    public enum PositionEnum
    {
        TopLeft,
        TopRight,
        BottomLeft,
        BottomRight
    }

    private PositionEnum position;
    public PositionEnum Position
    {
        get { return position; }
        set { position = value; }
    }

    private int fontSize = 30;
    public int FontSize
    {
        get { return fontSize; }
        set { fontSize = value; }
    }

    public void addLabel(TemplateLabel label)
    {
        labels.Add(label);
        repositionLabels();
    }

    public void removeLabel(TemplateLabel label)
    {
        labels.Remove(label);
        repositionLabels();
    }

    private void repositionLabels()
    {
        for (int i = 0; i < labels.Count; i++)
        {
            TemplateLabel item = labels[i];
            if(position == PositionEnum.TopLeft)
            {
                int y = fontSize * i + spacing * (i + 1);
                item.reposition(spacing, y, fontSize, spacing, color, Enabled);
            }

            if (position == PositionEnum.TopRight)
            {
                int y = fontSize * i + spacing * (i + 1);
                item.reposition(Screen.width - fontSize - item.getWidth(), y, fontSize, spacing, color, Enabled);
            }

            if (position == PositionEnum.BottomLeft)
            {
                int y = Screen.height - fontSize * (i + 1) - spacing * (i + 1);
                item.reposition(spacing, y, fontSize, spacing, color, Enabled);
            }

            if (position == PositionEnum.BottomRight)
            {
                int y = Screen.height - fontSize * (i + 1) - spacing * (i + 1);
                item.reposition(Screen.width - fontSize - item.getWidth(), y, fontSize, spacing, color, Enabled);
            }
        }
    }

    public override void OnToggle()
    {
        base.OnToggle();
        if(Enabled)
        {
            repositionLabels();
        } else
        {
            repositionLabels();
        }
    }
}
