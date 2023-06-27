using System;
using UnityEngine;

namespace Popcron;

[Obsolete("THIS CLASS IS BROKEN, BECAUSE I CHANGED THE DRAWING MODE TO GL_TRIANGLES")]
public class LineDrawer : Drawer
{
    public override int Draw(ref Vector3[] buffer, params object[] args)
    {
        buffer[0] = (Vector3)args[0];
        buffer[1] = (Vector3)args[1];
        return 2;
    }
}
