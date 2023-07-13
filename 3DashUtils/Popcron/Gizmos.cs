using _3DashUtils.Popcron;
using System;
using UnityEngine;

namespace Popcron;

public class Gizmos
{
    private static string _prefsKey = null;

    private static int? _bufferSize = null;

    private static bool? _enabled = null;

    private static float? _dashGap = null;

    private static bool? _cull = null;

    private static int? _pass = null;

    private static Vector3? _offset = null;

    private static Vector3[] buffer = new Vector3[BufferSize];

    public static Func<Camera, bool> CameraFilter = (Camera cam) => false;

    private static string PrefsKey
    {
        get
        {
            if (string.IsNullOrEmpty(_prefsKey))
            {
                _prefsKey = SystemInfo.deviceUniqueIdentifier + "." + Application.companyName + "." + Application.productName + ".Popcron.Gizmos";
            }
            return _prefsKey;
        }
    }

    public static int BufferSize
    {
        get
        {
            if (!_bufferSize.HasValue)
            {
                _bufferSize = PlayerPrefs.GetInt(PrefsKey + ".BufferSize", 4096);
            }
            return _bufferSize.Value;
        }
        set
        {
            value = Mathf.Clamp(value, 0, int.MaxValue);
            if (_bufferSize != value)
            {
                _bufferSize = value;
                PlayerPrefs.SetInt(PrefsKey + ".BufferSize", value);
                buffer = new Vector3[value];
            }
        }
    }

    public static bool Enabled
    {
        get
        {
            if (!_enabled.HasValue)
            {
                _enabled = PlayerPrefs.GetInt(PrefsKey + ".Enabled", 1) == 1;
            }
            return _enabled.Value;
        }
        set
        {
            if (_enabled != value)
            {
                _enabled = value;
                PlayerPrefs.SetInt(PrefsKey + ".Enabled", value ? 1 : 0);
            }
        }
    }

    public static float DashGap
    {
        get
        {
            if (!_dashGap.HasValue)
            {
                _dashGap = PlayerPrefs.GetFloat(PrefsKey + ".DashGap", 0.1f);
            }
            return _dashGap.Value;
        }
        set
        {
            if (_dashGap != value)
            {
                _dashGap = value;
                PlayerPrefs.SetFloat(PrefsKey + ".DashGap", value);
            }
        }
    }

    [Obsolete("This property is obsolete. Use FrustumCulling instead.", false)]
    public static bool Cull
    {
        get
        {
            return FrustumCulling;
        }
        set
        {
            FrustumCulling = value;
        }
    }

    [Obsolete("This property is obsolete. Subscribe to CameraFilter predicate instead and return true for your custom camera.", false)]
    public static Camera Camera
    {
        get
        {
            return null;
        }
        set
        {
        }
    }

    public static bool FrustumCulling
    {
        get
        {
            if (!_cull.HasValue)
            {
                _cull = PlayerPrefs.GetInt(PrefsKey + ".FrustumCulling", 1) == 1;
            }
            return _cull.Value;
        }
        set
        {
            if (_cull != value)
            {
                _cull = value;
                PlayerPrefs.SetInt(PrefsKey + ".FrustumCulling", value ? 1 : 0);
            }
        }
    }

    public static Material Material
    {
        get
        {
            return GizmosInstance.Material;
        }
        set
        {
            GizmosInstance.Material = value;
        }
    }

    public static int Pass
    {
        get
        {
            if (!_pass.HasValue)
            {
                _pass = PlayerPrefs.GetInt(PrefsKey + ".Pass", 0);
            }
            return _pass.Value;
        }
        set
        {
            if (_pass != value)
            {
                _pass = value;
                PlayerPrefs.SetInt(PrefsKey + ".Pass", value);
            }
        }
    }

    public static Vector3 Offset
    {
        get
        {
            if (!_offset.HasValue)
            {
                string @string = PlayerPrefs.GetString(PrefsKey + ".Offset", 0 + "," + 0 + "," + 0);
                int num = @string.IndexOf(",");
                int num2 = @string.LastIndexOf(",");
                if (num + num2 <= 0)
                {
                    return Vector3.zero;
                }
                string[] array = @string.Split(new char[1] { ","[0] });
                _offset = new Vector3(float.Parse(array[0]), float.Parse(array[1]), float.Parse(array[2]));
            }
            return _offset.Value;
        }
        set
        {
            if (_offset != value)
            {
                _offset = value;
                PlayerPrefs.SetString(PrefsKey + ".Offset", value.x + "," + value.y + "," + value.y);
            }
        }
    }

    public static void Draw<T>(Color? color, bool dashed, params object[] args) where T : Drawer
    {
        if (Enabled)
        {
            Drawer drawer = Drawer.Get<T>();
            if (drawer != null)
            {
                int num = drawer.Draw(ref buffer, args);
                Vector3[] array = new Vector3[num];
                Array.Copy(buffer, array, num);
                GizmosInstance.Submit(array, color, dashed);
            }
        }
    }

    public static void Lines(Vector3[] lines, Color? color = null, bool dashed = false)
    {
        if (Enabled)
        {
            GizmosInstance.Submit(lines, color, dashed);
        }
    }

    public static void Line(Vector3 a, Vector3 b, Color? color = null, bool dashed = false)
    {
        Draw<LineDrawer>(color, dashed, new object[2] { a, b });
    }

    public static void Square(Vector2 position, Vector2 size, Color? color = null, bool dashed = false)
    {
        Square(position, Quaternion.identity, size, color, dashed);
    }

    public static void Square(Vector2 position, float diameter, Color? color = null, bool dashed = false)
    {
        Square(position, Quaternion.identity, Vector2.one * diameter, color, dashed);
    }

    public static void Square(Vector2 position, Quaternion rotation, Vector2 size, Color? color = null, bool dashed = false)
    {
        Draw<SquareDrawer>(color, dashed, new object[3] { position, rotation, size });
    }

    public static void Cube(Vector3 position, Quaternion rotation, Vector3 size, Color? color = null, bool dashed = false)
    {
        Draw<CubeDrawer>(color, dashed, new object[3] { position, rotation, size });
    }

    public static void Rect(Rect rect, Camera camera, Color? color = null, bool dashed = false)
    {
        rect.y = (float)Screen.height - rect.y;
        Vector2 vector = camera.ScreenToWorldPoint(new Vector2(rect.x, rect.y - rect.height));
        Draw<SquareDrawer>(color, dashed, new object[3]
        {
            vector + rect.size * 0.5f,
            Quaternion.identity,
            rect.size
        });
    }

    public static void Bounds(Bounds bounds, Color? color = null, bool dashed = false)
    {
        Draw<CubeDrawer>(color, dashed, new object[3]
        {
            bounds.center,
            Quaternion.identity,
            bounds.size
        });
    }

    public static void Cone(Vector3 position, Quaternion rotation, float length, float angle, Color? color = null, bool dashed = false, int pointsCount = 16)
    {
        float num = Mathf.Tan(angle * 0.5f * (Mathf.PI / 180f)) * length;
        Vector3 vector = rotation * Vector3.forward;
        Vector3 vector2 = position + vector * length;
        float num2 = 0f;
        Draw<PolygonDrawer>(color, dashed, new object[5] { vector2, pointsCount, num, num2, rotation });
        for (int i = 0; i < 4; i++)
        {
            float f = (float)i * 90f * (Mathf.PI / 180f);
            Vector3 vector3 = rotation * new Vector3(Mathf.Cos(f), Mathf.Sin(f)) * num;
            Line(position, position + vector3 + vector * length, color, dashed);
        }
    }

    public static void Sphere(Vector3 position, float radius, Color? color = null, bool dashed = false, int pointsCount = 16)
    {
        Draw<SphereDrawer>(color, dashed, position, radius, pointsCount, pointsCount);
        /*
		float num = 0f;
		Draw<PolygonDrawer>(color, dashed, new object[5]
		{
			position,
			pointsCount,
			radius,
			num,
			Quaternion.Euler(0f, 0f, 0f)
		});
		Draw<PolygonDrawer>(color, dashed, new object[5]
		{
			position,
			pointsCount,
			radius,
			num,
			Quaternion.Euler(90f, 0f, 0f)
		});
		Draw<PolygonDrawer>(color, dashed, new object[5]
		{
			position,
			pointsCount,
			radius,
			num,
			Quaternion.Euler(0f, 90f, 90f)
		});
		*/
    }

    public static void Circle(Vector3 position, float radius, Camera camera, Color? color = null, bool dashed = false, int pointsCount = 16)
    {
        float num = 0f;
        Quaternion quaternion = Quaternion.LookRotation(position - camera.transform.position);
        Draw<PolygonDrawer>(color, dashed, new object[5] { position, pointsCount, radius, num, quaternion });
    }

    public static void Circle(Vector3 position, float radius, Quaternion rotation, Color? color = null, bool dashed = false, int pointsCount = 16)
    {
        float num = 0f;
        Draw<PolygonDrawer>(color, dashed, new object[5] { position, pointsCount, radius, num, rotation });
    }
}
