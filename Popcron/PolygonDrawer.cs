using System;
using UnityEngine;

namespace Popcron;

public class PolygonDrawer : Drawer
{
    public override int Draw(ref Vector3[] buffer, params object[] values)
    {
        // could make it double sided??? nahh

        Vector3 position = (Vector3)values[0];
        int points = (int)values[1];
        float radius = (float)values[2];
        float offset = (float)values[3];
        Quaternion rotation = (Quaternion)values[4];

        float step = 360f / points;
        offset *= Mathf.Deg2Rad;

        for (int i = 0; i < points; i++)
        {
            float cx = Mathf.Cos(Mathf.Deg2Rad * step * i + offset) * radius;
            float cy = Mathf.Sin(Mathf.Deg2Rad * step * i + offset) * radius;
            Vector3 current = new Vector3(cx, cy);

            float nx = Mathf.Cos(Mathf.Deg2Rad * step * (i + 1) + offset) * radius;
            float ny = Mathf.Sin(Mathf.Deg2Rad * step * (i + 1) + offset) * radius;
            Vector3 next = new Vector3(nx, ny);
            buffer[(i * 3) + 0] = position + (rotation * next);
            buffer[(i * 3) + 1] = position + (rotation * current);
            buffer[(i * 3) + 2] = position;
        }
        return points * 3;
    }
}
