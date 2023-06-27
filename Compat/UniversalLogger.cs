using System.Linq;
using HarmonyLib;
#if BEPINEX
using BepInEx;
using BepInEx.Logging;
#elif MELON
using MelonLoader;
#endif

namespace _3DashUtils.Compat;

public class UniversalLogger
{

#if BEPINEX
    public UniversalLogger(BaseUnityPlugin mod)
    {
        // its internal so i dont care
        Log = (ManualLogSource)Traverse.Create(mod).Property("Logger").GetValue();
    }
#elif MELON
    public UniversalLogger(MelonMod mod)
    {
        Log = mod.LoggerInstance;
    }
#endif

#if BEPINEX
    internal static ManualLogSource Log;
#elif MELON
    internal MelonLogger.Instance Log;
#endif

    public void Dbg(params object[] obj)
    {
        var str = StringifyParams(obj);
#if BEPINEX
        Log.LogDebug(str);
#elif MELON
#if DEBUG
        Log.Msg(StringifyParams(obj));
#endif
#endif
    }
    public void Msg(params object[] obj)
    {
        var str = StringifyParams(obj);
#if BEPINEX
        Log.LogMessage(str);
#elif MELON
        Log.Msg(str);
#endif
    }
    public void Warn(params object[] obj)
    {
        var str = StringifyParams(obj);
#if BEPINEX
        Log.LogWarning(str);
#elif MELON
        Log.Warning(StringifyParams(obj));
#endif
    }
    public void Error(params object[] obj)
    {
        var str = StringifyParams(obj);
#if BEPINEX
        Log.LogError(str);
#elif MELON
        Log.Error(StringifyParams(obj));
#endif
    }

    private string StringifyParams(params object[] obj)
    {
        if(obj == null)
        {
            return "";
        }
        return string.Join(" ", obj.AsQueryable().Select((a) => a.ToString()));
    }
}
