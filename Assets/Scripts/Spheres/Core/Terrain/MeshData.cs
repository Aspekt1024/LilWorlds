using UnityEngine;

namespace Aspekt.Spheres
{
    public class MeshData
    {
        public Vector3[] Vertices { get; }
        public int[] Triangles { get; }
        public Vector2[] UVs { get; }
        public Vector3[] Normals { get; private set; }
        
        private int triangleIndex;

        // For calculating normals, we take the surrounding triangles and average the normals between them
        private readonly Vector3[] outOfMeshVertices;
        private readonly int[] outOfMeshTriangles;
        private int outOfMeshTriangleIndex;
        
        public MeshData(int width, int height, int skipInterval)
        {
            var meshWidth = (width - 1) / skipInterval + 1;
            var meshHeight = (height - 1) / skipInterval + 1;
            
            var numVertices = meshWidth * meshHeight;
            var numTriangles = (meshWidth - 1) * (meshHeight - 1) * 2;
            
            Vertices = new Vector3[numVertices];
            Triangles = new int[numTriangles * 3];
            UVs = new Vector2[numVertices];
            
            outOfMeshVertices = new Vector3[meshWidth * 4 - 4];
            outOfMeshTriangles = new int[24 * (meshWidth - 2)];
        }
        
        public void AddVertex(Vector3 pos, Vector2 uv, int index)
        {
            if (index < 0)
            {
                outOfMeshVertices[-index - 1] = pos;
            }
            else
            {
                Vertices[index] = pos;
                UVs[index] = uv;
            }
        }

        public void AddTriangle(int a, int b, int c)
        {
            if (a < 0 || b < 0 || c < 0)
            {
                outOfMeshTriangles[outOfMeshTriangleIndex] = a;
                outOfMeshTriangles[outOfMeshTriangleIndex + 1] = b;
                outOfMeshTriangles[outOfMeshTriangleIndex + 2] = c;
                outOfMeshTriangleIndex += 3;
            }
            else
            {
                Triangles[triangleIndex] = a;
                Triangles[triangleIndex + 1] = b;
                Triangles[triangleIndex + 2] = c;
                triangleIndex += 3;
            }
        }

        public void Finalise(bool useTriplanarNormals)
        {
            Normals = CalculateNormals(useTriplanarNormals);
        }

        private Vector3[] CalculateNormals(bool useTriplanarNormals)
        {
            if (!useTriplanarNormals) return new Vector3[0];
            
            var vertexNormals = new Vector3[Vertices.Length];
            var triCount = Triangles.Length / 3f;
            for (int i = 0; i < triCount; i++)
            {
                var normalTriangleIndex = i * 3;
                var vertexIndexA = Triangles[normalTriangleIndex];
                var vertexIndexB = Triangles[normalTriangleIndex + 1];
                var vertexIndexC = Triangles[normalTriangleIndex + 2];

                var triNormal = SurfaceNormalFromIndices(vertexIndexA, vertexIndexB, vertexIndexC);
                vertexNormals[vertexIndexA] += triNormal;
                vertexNormals[vertexIndexB] += triNormal;
                vertexNormals[vertexIndexC] += triNormal;
            }
            
            var borderTriCount = outOfMeshTriangles.Length / 3f;
            for (int i = 0; i < borderTriCount; i++)
            {
                var normalTriangleIndex = i * 3;
                var vertexIndexA = outOfMeshTriangles[normalTriangleIndex];
                var vertexIndexB = outOfMeshTriangles[normalTriangleIndex + 1];
                var vertexIndexC = outOfMeshTriangles[normalTriangleIndex + 2];
            
                var triNormal = SurfaceNormalFromIndices(vertexIndexA, vertexIndexB, vertexIndexC);
                if (vertexIndexA >= 0) vertexNormals[vertexIndexA] += triNormal;
                if (vertexIndexB >= 0) vertexNormals[vertexIndexB] += triNormal;
                if (vertexIndexC >= 0) vertexNormals[vertexIndexC] += triNormal;
            }

            for (int i = 0; i < vertexNormals.Length; i++)
            {
                vertexNormals[i].Normalize();
            }

            return vertexNormals;
        }

        private Vector3 SurfaceNormalFromIndices(int indexA, int indexB, int indexC)
        {
            var pointA = indexA < 0 ? outOfMeshVertices[-indexA - 1] : Vertices[indexA];
            var pointB = indexB < 0 ? outOfMeshVertices[-indexB - 1] : Vertices[indexB];
            var pointC = indexC < 0 ? outOfMeshVertices[-indexC - 1] : Vertices[indexC];

            var sideAB = pointB - pointA;
            var sideAC = pointC - pointA;
            
            return Vector3.Cross(sideAB, sideAC).normalized;
        }
    }
}