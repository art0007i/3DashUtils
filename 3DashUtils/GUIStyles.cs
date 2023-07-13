using UnityEngine;

namespace _3DashUtils;

public class GUIStyles
{
    private static GUIStyle tooltip;
    public static GUIStyle Tooltip
    {
        get
        {
            if (tooltip == null)
            {
                tooltip = new GUIStyle(GUI.skin.button);
                tooltip.fontSize = 16;
            }
            return tooltip;
        }
    }
    private static GUIStyle configButton;
    public static GUIStyle ConfigButton
    {
        get
        {
            if (configButton == null)
            {
                configButton = new GUIStyle(GUI.skin.button);
                configButton.alignment = TextAnchor.MiddleCenter;
            }
            return configButton;
        }
    }
    private static GUIStyle configLayout;
    public static GUIStyle ConfigLayout
    {
        get
        {
            if (configLayout == null)
            {
                // keeping this for when I want to re-style the ui maybe?
                configLayout = new GUIStyle(GUI.skin.box);
            }
            return configLayout;
        }
    }
}
