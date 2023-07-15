using Popcron;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace _3DashUtils.Popcron
{
    public class SphereDrawer : Drawer
    {
        public override int Draw(ref Vector3[] buffer, params object[] values)
        {
            Vector3 center = (Vector3)values[0];
            float radius = (float)values[1];
            int horizontalDivisions = (int)values[2];
            int verticalDivisions = (int)values[3];
            for (int lat = 0; lat < verticalDivisions; lat++)
            {
                float theta1 = Mathf.PI * ((float)lat / verticalDivisions);
                float theta2 = Mathf.PI * ((float)(lat + 1) / verticalDivisions);

                for (int lon = 0; lon < horizontalDivisions; lon++)
                {
                    float phi1 = 2 * Mathf.PI * ((float)lon / horizontalDivisions);
                    float phi2 = 2 * Mathf.PI * ((float)(lon + 1) / horizontalDivisions);

                    // Calculate vertex positions for the triangles
                    Vector3 p1 = CalculateSpherePoint(center, radius, theta1, phi1);
                    Vector3 p2 = CalculateSpherePoint(center, radius, theta1, phi2);
                    Vector3 p3 = CalculateSpherePoint(center, radius, theta2, phi1);
                    Vector3 p4 = CalculateSpherePoint(center, radius, theta2, phi2);

                    var idx = ((horizontalDivisions * lat) + lon) * 6;

                    buffer[idx] = p1;
                    buffer[idx + 1] = p2;
                    buffer[idx + 2] = p3;
                    buffer[idx + 3] = p2;
                    buffer[idx + 4] = p4;
                    buffer[idx + 5] = p3;
                }
            }
            // 2 triangles, 3 verts each
            return horizontalDivisions * verticalDivisions * 2 * 3;
        }
        Vector3 CalculateSpherePoint(Vector3 center, float radius, float theta, float phi)
        {
            float x = center.x + radius * Mathf.Sin(theta) * Mathf.Cos(phi);
            float y = center.y + radius * Mathf.Cos(theta);
            float z = center.z + radius * Mathf.Sin(theta) * Mathf.Sin(phi);

            return new Vector3(x, y, z);
        }
    }
}
