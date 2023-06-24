using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3DashUtils.ModuleSystem.Config;

public class CommonTextParsers
{
    public static Dictionary<Type, Delegate> Parsers = new()
    {
        {
            typeof(int),
            TryParseInt
        },
        {
            typeof(float),
            TryParseFloat
        },
        {
            typeof(double),
            TryParseDouble
        },
        {
            typeof(string),
            StringParser
        }
    };

    public static bool TryParseInt(string text, out int value, Func<int, bool> checkValue)
    {
        return int.TryParse(text, out value) && checkValue(value);
    }
    public static bool TryParseFloat(string text, out float value, Func<float, bool> checkValue)
    {
        return float.TryParse(text, out value) && checkValue(value);
    }
    public static bool TryParseDouble(string text, out double value, Func<double, bool> checkValue)
    {
        return double.TryParse(text, out value) && checkValue(value);
    }
    public static bool StringParser(string text, out string value, Func<string, bool> checkValue)
    {
        value = text;
        return checkValue(text);
    }
}
