using System.Collections.Generic;
using UnityEngine;

namespace Aspekt.Spheres
{
    public class SphereMesh
    {
        public Vector3[] Vertices;
        public int[] Triangles;
        
        private FixedSizeList<Vector3> vertices;
        private FixedSizeList<int> triangles;
        private int numDivisions;
        private int numVertsPerFace;

        private readonly Transform parent;
        
        private static readonly Vector3[] BaseVertices = { Vector3.up, Vector3.left, Vector3.back, Vector3.right, Vector3.forward, Vector3.down };
        
        // Indices of the vertex pairs that make up each of the initial 12 edges
        private static readonly int[] EdgeVertexPairs = { 0, 1, 0, 2, 0, 3, 0, 4, 1, 2, 2, 3, 3, 4, 4, 1, 5, 1, 5, 2, 5, 3, 5, 4 };

        // Indices of the edge triplets that make up the initial 8 faces
        private static readonly int[] FaceEdgeTriplets = { 0, 1, 4, 1, 2, 5, 2, 3, 6, 3, 0, 7, 8, 9, 4, 9, 10, 5, 10, 11, 6, 11, 8, 7 };

        public SphereMesh(Transform parent)
        {
            this.parent = parent;
        }

        public void GenerateSphere(SphereShapeSettings shapeSettings, SphereShadingSettings shadingSettings)
        {
            GenerateVertsAndTris(shapeSettings.resolution);

            var mesh = GetMesh();
            mesh.SetVertices(Vertices);
            mesh.SetTriangles(Triangles, 0, true);
            mesh.RecalculateNormals();

            var meshRenderer = parent.GetComponentInChildren<MeshRenderer>();
            meshRenderer.sharedMaterial = shadingSettings.material;
        }

        private void GenerateVertsAndTris(int resolution)
        {
            numDivisions = Mathf.Max(0, resolution);
            numVertsPerFace = ((numDivisions + 3) * (numDivisions + 3) - (numDivisions + 3)) / 2;
            var numVerts = numVertsPerFace * 8 - (numDivisions + 2) * 12 + 6;
            int numTrisPerFace = (numDivisions + 1) * (numDivisions + 1);
            
            vertices = new FixedSizeList<Vector3>(numVerts);
            triangles = new FixedSizeList<int>(numTrisPerFace * 24);

            vertices.AddRange(BaseVertices);

            var edges = new List<int[]>(EdgeVertexPairs.Length / 2);
            for (int i = 0; i < EdgeVertexPairs.Length; i += 2)
            {
                var startVertex = vertices.Items[EdgeVertexPairs[i]];
                var endVertex = vertices.Items[EdgeVertexPairs[i + 1]];

                var edgeVertexIndices = new int[numDivisions + 2];
                edgeVertexIndices[0] = EdgeVertexPairs[i];
                edgeVertexIndices[edgeVertexIndices.Length - 1] = EdgeVertexPairs[i + 1];
                
                for (int divisionIndex = 0; divisionIndex < numDivisions; divisionIndex++)
                {
                    var percent = (float)(divisionIndex + 1) / (numDivisions + 1);
                    edgeVertexIndices[divisionIndex + 1] = vertices.NextIndex;
                    vertices.Add(Vector3.Slerp(startVertex, endVertex, percent));
                }
                
                edges.Add(edgeVertexIndices);
            }
            
            for (int i = 0; i < FaceEdgeTriplets.Length; i += 3)
            {
                var faceIndex = i / 3;
                var reverse = faceIndex >= 4;
                CreateFace(edges[FaceEdgeTriplets[i]], edges[FaceEdgeTriplets[i + 1]], edges[FaceEdgeTriplets[i + 2]], reverse);
            }

            Vertices = vertices.Items;
            Triangles = triangles.Items;
        }

        private void CreateFace(int[] sideA, int[] sideB, int[] bottom, bool reverse)
        {
            var numPointsInEdge = sideA.Length;
            var vertexMap = new FixedSizeList<int>(numVertsPerFace);
            vertexMap.Add(sideA[0]);

            for (int i = 1; i < numPointsInEdge - 1; i++)
            {
                var sideAVertex = vertices.Items[sideA[i]];
                var sideBVertex = vertices.Items[sideB[i]];
                
                vertexMap.Add(sideA[i]);

                var numInnerPoints = i - 1;
                for (int j = 0; j < numInnerPoints; j++)
                {
                    var t = (j + 1f) / (numInnerPoints + 1f);
                    vertexMap.Add(vertices.NextIndex);
                    vertices.Add(Vector3.Slerp(sideAVertex, sideBVertex, t));
                }
                
                vertexMap.Add(sideB[i]);
            }

            for (int i = 0; i < numPointsInEdge; i++)
            {
                vertexMap.Add(bottom[i]);
            }
            
            // Triangulate
            var numRows = numDivisions + 1;
            for (int row = 0; row < numRows; row++)
            {
                // vertices down left edge follow quadratic sequence: 0, 1, 3, 6, 10, 15...
                // the nth term can be calculated with: (n^2 - n)/2
                var topVertex = ((row + 1) * (row + 1) - (row + 1)) / 2;
                var bottomVertex = ((row + 2) * (row + 2) - (row + 2)) / 2;

                var numTrianglesInRow = 2 * row + 1;
                for (int column = 0; column < numTrianglesInRow; column++)
                {
                    var v0 = topVertex;
                    
                    int v1, v2;
                    if (column % 2 == 0)
                    {
                        v1 = bottomVertex + 1;
                        v2 = bottomVertex;
                        topVertex++;
                        bottomVertex++;
                    }
                    else
                    {
                        v1 = bottomVertex;
                        v2 = topVertex - 1;
                    }
                    
                    triangles.Add(vertexMap.Items[v0]);
                    triangles.Add(vertexMap.Items[reverse ? v2 : v1]);
                    triangles.Add(vertexMap.Items[reverse ? v1: v2]);
                }
            }
        }

        private Mesh GetMesh()
        {
            var meshFilter = parent.GetComponentInChildren<MeshFilter>();
            if (meshFilter != null)
            {
                if (meshFilter.sharedMesh.vertices.Length != Vertices.Length)
                {
                    meshFilter.sharedMesh = new Mesh();
                }
                return meshFilter.sharedMesh;
            }
            
            var gameObject = new GameObject("SphereMesh");
            gameObject.transform.SetParent(parent);
            gameObject.transform.localPosition = Vector3.zero;
            gameObject.transform.localRotation = Quaternion.identity;

            meshFilter = gameObject.AddComponent<MeshFilter>();
            gameObject.AddComponent<MeshRenderer>();

            meshFilter.sharedMesh = new Mesh();

            return meshFilter.sharedMesh;
        }
    }
}