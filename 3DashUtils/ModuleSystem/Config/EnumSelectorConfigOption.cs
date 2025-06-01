using System;
using System.ComponentModel;
using UnityEngine;

namespace _3DashUtils.ModuleSystem.Config;

class EnumSelectorConfigOption<T> : ConfigOptionBase<T>
{
    T selectedEnum;
    
    public EnumSelectorConfigOption(IMenuModule module, string name, T defaultValue, string description)
        : base(module, name, defaultValue, description)
    {
        selectedEnum = defaultValue;
    }

    public Action<T>? Changed;
    string[] names = Enum.GetNames(typeof(T));
    int selectedIndex = 0;
    T selectedEnumIndex;

    public void Start()
    {
        updateSelectedIndex();
    }

    public void updateSelectedIndex()
    {
        selectedEnumIndex = (T)Enum.Parse(typeof(T), names[selectedIndex]);
        Changed?.Invoke(selectedEnumIndex);

    }

    public override void OnGUI()
    {
        if(GUILayout.Button("<"))
        {
            if (selectedIndex - 1 > 0)
            {
                selectedIndex--;
            }
            else
            {
                selectedIndex = names.Length - 1;
            }
            updateSelectedIndex();
        }

        GUILayout.Label(selectedEnumIndex.ToString(), GUILayout.Width(75));
        if (GUILayout.Button(">"))
        {
            if (selectedIndex + 1 < names.Length)
            {
                selectedIndex++;
            }
            else
            {
                selectedIndex = 0;
            }
            updateSelectedIndex();
        }
    }
}
