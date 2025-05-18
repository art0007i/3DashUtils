using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _3DashUtils.ModuleSystem;
using UnityEngine;

namespace _3DashUtils.Mods.Status;
public abstract class TemplateLabel : ToggleModule
{
    private int x;
    private int y;
    private int fontSize;
    private int spacing;
    private Color color;
    private GUIStyle style;
    private bool visible = false;
    public virtual string text { get; set; }

    public int getWidth()
    {
        UpdateText();
        style = new GUIStyle();
        style.fontSize = fontSize;
        style.normal.textColor = color;
        //GUI.Label(new Rect(x, y, 500, fontSize + spacing), text, style);
        return (int)style.CalcSize(new GUIContent(text)).x;
    }

    public void reposition(int _x, int _y, int _fontSize, int _spacing, Color _color, bool _visible)
    {
        x = _x; y = _y; fontSize = _fontSize; spacing = _spacing; color = _color; visible = _visible;
    }

    public override void OnUnityGUI()
    {
        if (Enabled && visible)
        {
            UpdateText();
            style = new GUIStyle();
            style.fontSize = fontSize;
            style.normal.textColor = color;
            GUI.Label(new Rect(x, y, 500, fontSize + spacing), text, style);
        }
    }
    public virtual void UpdateText() { }

    public override void OnToggle()
    {
        base.OnToggle();
        if (Enabled)
        {
            if(Extensions.GetModule<StatusManager>().Enabled) visible = true;
            Extensions.GetModule<StatusManager>().addLabel(this);
        } else
        {
            visible = false;
            Extensions.GetModule<StatusManager>().removeLabel(this);
        }
    }
}

