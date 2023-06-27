using UnityEngine;

namespace Popcron;

public class CubeDrawer : Drawer
{
    public override int Draw(ref Vector3[] buffer, params object[] values)
    {
        Vector3 position = (Vector3)values[0];
        Quaternion rotation = (Quaternion)values[1];
        Vector3 size = (Vector3)values[2];
        size *= 0.5f;
        Vector3 v1 = new Vector3(position.x - size.x, position.y - size.y, position.z - size.z);
        Vector3 v2 = new Vector3(position.x + size.x, position.y - size.y, position.z - size.z);
        Vector3 v3 = new Vector3(position.x + size.x, position.y + size.y, position.z - size.z);
        Vector3 v4 = new Vector3(position.x - size.x, position.y + size.y, position.z - size.z);
        Vector3 v5 = new Vector3(position.x - size.x, position.y - size.y, position.z + size.z);
        Vector3 v6 = new Vector3(position.x + size.x, position.y - size.y, position.z + size.z);
        Vector3 v7 = new Vector3(position.x + size.x, position.y + size.y, position.z + size.z);
        Vector3 v8 = new Vector3(position.x - size.x, position.y + size.y, position.z + size.z);
        v1 = rotation * (v1 - position);
        v1 += position;
        v2 = rotation * (v2 - position);
        v2 += position;
        v3 = rotation * (v3 - position);
        v3 += position;
        v4 = rotation * (v4 - position);
        v4 += position;
        v5 = rotation * (v5 - position);
        v5 += position;
        v6 = rotation * (v6 - position);
        v6 += position;
        v7 = rotation * (v7 - position);
        v7 += position;
        v8 = rotation * (v8 - position);
        v8 += position;

        // this code required an mspaint drawing and lots of winding direction trial and error to write

        // zneg
        buffer[0] = v3;
        buffer[1] = v2;
        buffer[2] = v1;
        buffer[3] = v1;
        buffer[4] = v4;
        buffer[5] = v3;
        // yneg
        buffer[6] = v1;
        buffer[7] = v2;
        buffer[8] = v5;
        buffer[9] = v5;
        buffer[10] = v2;
        buffer[11] = v6;
        // zpos
        buffer[12] = v8;
        buffer[13] = v5;
        buffer[14] = v6;
        buffer[15] = v6;
        buffer[16] = v7;
        buffer[17] = v8;
        // ypos
        buffer[18] = v3;
        buffer[19] = v4;
        buffer[20] = v8;
        buffer[21] = v8;
        buffer[22] = v7;
        buffer[23] = v3;
        // xneg
        buffer[24] = v1;
        buffer[25] = v5;
        buffer[26] = v4;
        buffer[27] = v8;
        buffer[28] = v4;
        buffer[29] = v5;
        // xpos
        buffer[30] = v3;
        buffer[31] = v6;
        buffer[32] = v2;
        buffer[33] = v3;
        buffer[34] = v7;
        buffer[35] = v6;

        return 36;
        /*
		buffer[0] = v1;
		buffer[1] = v2;
		buffer[2] = v2;
		buffer[3] = v3;
		buffer[4] = v3;
		buffer[5] = v4;
		buffer[6] = v4;
		buffer[7] = v1;
		buffer[8] = v5;
		buffer[9] = v6;
		buffer[10] = v6;
		buffer[11] = v7;
		buffer[12] = v7;
		buffer[13] = v8;
		buffer[14] = v8;
		buffer[15] = v5;
		buffer[16] = v1;
		buffer[17] = v5;
		buffer[18] = v2;
		buffer[19] = v6;
		buffer[20] = v3;
		buffer[21] = v7;
		buffer[22] = v4;
		buffer[23] = v8;
		*/
        //return 24;
    }
}
