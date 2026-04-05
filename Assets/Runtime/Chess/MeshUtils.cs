using UnityEngine;

namespace Runtime.Chess
{
    public static class MeshUtils
    {
        public static Mesh CreateCubeMesh()
        {
            var mesh = new Mesh();

            var vertices = new Vector3[]
            {
                new Vector3(0, 0, 0),
                new Vector3(1, 0, 0),
                new Vector3(1, 0, 1),
                new Vector3(0, 0, 1),
                new Vector3(0, 1, 0),
                new Vector3(1, 1, 0),
                new Vector3(1, 1, 1),
                new Vector3(0, 1, 1)
            };

            var triangles = new int[]
            {
                0, 1, 2, 0, 2, 3, // Bottom
                4, 6, 5, 4, 7, 6, // Top
                0, 5, 1, 0, 4, 5, // Front
                1, 6, 2, 1, 5, 6, // Right
                2, 7, 3, 2, 6, 7, // Back
                3, 4, 0, 3, 7, 4  // Left
            };
            
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.RecalculateNormals();

            return mesh;
        }
    }
}