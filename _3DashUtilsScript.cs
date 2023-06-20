using _3DashUtils.Mods.Hidden;
using HarmonyLib;
using System;
using UnityEngine;

namespace _3DashUtils;

public class _3DashUtilsScript : MonoBehaviour
{
	private string lastTooltipContent;
	private float lastTooltipTime;
    private void Awake()
	{
        _3DashUtils.moduleList.Do((p) => p.Awake());
    }

    private void Start()
	{
		_3DashUtils.moduleList.Do((p) => p.Start());
	}

	public void Update()
    {
        _3DashUtils.moduleList.Do((p) => p.Update());
	}

	public void OnGUI()
    {
        if (MenuHandler.menuOpen.Value)
        {
            foreach (var key in _3DashUtils.moduleCategories.Keys)
			{
				if(key == "Hidden")
				{
					continue;
				}

				var cat = _3DashUtils.moduleCategories[key];
			
				cat.windowRect = GUILayout.Window(cat.internalWindowID, cat.windowRect, (windowID) =>
				{
					cat.modules.Sort((mod1,mod2) => Math.Sign(mod2.Priority-mod1.Priority));
					foreach (var gui in cat.modules)
					{
						gui.OnGUI();
                    }
                    if (!string.IsNullOrWhiteSpace(GUI.tooltip)){
						lastTooltipContent = GUI.tooltip;
						lastTooltipTime = Time.realtimeSinceStartup;
                    }
                    GUI.DragWindow();
                }, key);
            }

			if (!string.IsNullOrWhiteSpace(lastTooltipContent) && Time.realtimeSinceStartup - lastTooltipTime < 0.5)
			{
                var content = new GUIContent(lastTooltipContent);
                var size = GUIStyles.Tooltip.CalcSize(content);
                var rect = new Rect(20, Screen.height - 60 - size.y, size.x + 20, size.y + 2);
                GUI.Box(rect, content, GUIStyles.Tooltip);
            }
        }
        _3DashUtils.moduleList.Do((p) => p.OnUnityGUI());
    }
}
