using UnityEngine;

namespace _Strategy.Runtime.Util
{
    public static class MeshUtils
    {
        public static Mesh CreateCubeMesh()
        {
            var mesh = new Mesh();

            // Create separate vertices for each face to avoid shared vertex normal issues
            var vertices = new Vector3[]
            {
                // Bottom face (4 vertices) - looking up from below
                new Vector3(0, 0, 0), new Vector3(1, 0, 0), new Vector3(1, 0, 1), new Vector3(0, 0, 1),
                // Top face (4 vertices) - looking down from above
                new Vector3(0, 1, 0), new Vector3(0, 1, 1), new Vector3(1, 1, 1), new Vector3(1, 1, 0),
                // Front face (4 vertices) - looking at -Z face
                new Vector3(0, 0, 0), new Vector3(0, 1, 0), new Vector3(1, 1, 0), new Vector3(1, 0, 0),
                // Right face (4 vertices) - looking at +X face  
                new Vector3(1, 0, 0), new Vector3(1, 1, 0), new Vector3(1, 1, 1), new Vector3(1, 0, 1),
                // Back face (4 vertices) - looking at +Z face
                new Vector3(1, 0, 1), new Vector3(1, 1, 1), new Vector3(0, 1, 1), new Vector3(0, 0, 1),
                // Left face (4 vertices) - looking at -X face
                new Vector3(0, 0, 1), new Vector3(0, 1, 1), new Vector3(0, 1, 0), new Vector3(0, 0, 0)
            };

            var triangles = new int[]
            {
                // Bottom face (Y = 0)
                0, 1, 2, 0, 2, 3,
                // Top face (Y = 1)
                4, 5, 6, 4, 6, 7,
                // Front face (Z = 0)
                8, 9, 10, 8, 10, 11,
                // Right face (X = 1)
                12, 13, 14, 12, 14, 15,
                // Back face (Z = 1)
                16, 17, 18, 16, 18, 19,
                // Left face (X = 0)
                20, 21, 22, 20, 22, 23
            };
            
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.RecalculateNormals();

            return mesh;
        }
    }
}