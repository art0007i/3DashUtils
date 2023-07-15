using System;
using UnityEngine;

namespace _3DashUtils.ModuleSystem.Config;

public class SliderConfig<T> : ConfigOptionBase<T>
{
    /// <summary>
    /// Creates a slider config option. <br/>
    /// Throws <see cref="InvalidCastException"/> whenever parameter <typeparamref name="T"/> is not convertible to a float.
    /// </summary>
    /// <exception cref="InvalidCastException">Thrown whenever parameter <typeparamref name="T"/> is not convertible to a float.</exception>
    /// <param name="decimalPlaces">This is used to render the current value of the slider.</param>
    public SliderConfig(IMenuModule module, string name, T defaultValue, string description, T min, T max, byte decimalPlaces = 2) : base(module, name, defaultValue, description)
    {
        float test = ToFloat(defaultValue);
        T test2 = FromFloat(test);

        Min = min;
        Max = max;
        DecimalPlaces = decimalPlaces;
    }
    public byte DecimalPlaces { get; private set; }
    public T Min { get; private set; }
    public T Max { get; private set; }

    public float ToFloat(T val) => Convert.ToSingle(val);
    public T FromFloat(float val) => (T)Convert.ChangeType(val, typeof(T));

    public override void OnGUI()
    {
        GUILayout.Label(ToFloat(Value).ToString("n" + DecimalPlaces), GUILayout.Width(25));
        Value = FromFloat(GUILayout.HorizontalSlider(ToFloat(Value), ToFloat(Min), ToFloat(Max), GUILayout.ExpandWidth(true)));
    }
}
