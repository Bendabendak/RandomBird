using UnityEngine;

namespace SadJam
{
    public static class CustomMeshes
    {
        public static Mesh Cone2D(Vector2 size, int quality)
        {
            Mesh mesh = new();

            float actualAngle = 90.0f - size.x;

            float segmentAngle = size.x * 2 / quality;

            Vector3[] verts = new Vector3[quality * 3];
            Vector3[] normals = new Vector3[quality * 3];
            int[] triangles = new int[quality * 3];
            Vector2[] uvs = new Vector2[quality * 3];

            for (int i = 0; i < verts.Length; i++)
            {
                verts[i] = new Vector3(0, 0, 0);
                normals[i] = Vector3.up;
            }

            float a = actualAngle;

            for (int i = 1; i < verts.Length; i += 3)
            {
                verts[i] = new Vector3(Mathf.Cos(Mathf.Deg2Rad * a) * size.y, 0, Mathf.Sin(Mathf.Deg2Rad * a) * size.y);

                a += segmentAngle;

                verts[i + 1] = new Vector3(Mathf.Cos(Mathf.Deg2Rad * a) * size.y, 0, Mathf.Sin(Mathf.Deg2Rad * a) * size.y);
            }

            for (int i = 0; i < triangles.Length; i += 3)
            {
                triangles[i] = 0;
                triangles[i + 1] = i + 2;
                triangles[i + 2] = i + 1;
            }

            mesh.vertices = verts;
            mesh.normals = normals;
            mesh.triangles = triangles;
            mesh.RecalculateBounds();

            for (int i = 0; i < uvs.Length; i++)
            {
                uvs[i] = new Vector2((verts[i].x + mesh.bounds.size.x / 2) / mesh.bounds.size.x, (verts[i].z + mesh.bounds.size.z / 2) / mesh.bounds.size.z);
            }

            mesh.uv = uvs;

            return mesh;
        } 
    }
}
