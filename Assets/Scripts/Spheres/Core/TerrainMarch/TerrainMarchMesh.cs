using System.Collections.Generic;
using UnityEngine;

namespace Aspekt.Spheres
{
    public class TerrainMarchMesh
    {
        private readonly Transform parent;
        
        private int size;
        private float surface;
        private float spacing;
        
        public TerrainMarchMesh(Transform parent)
        {
            this.parent = parent;
        }

        public void Generate(TerrainDensitySettings densitySettings, TerrainMarchShapeSettings shapeSettings, TerrainMarchShadingSettings shadingSettings)
        {
            
            // DENSITY
            var pointsBuffer = densitySettings.Generate();

            // MARCHING CUBES
            size = densitySettings.NumVoxels;
            surface = shapeSettings.surface;
            spacing = densitySettings.Spacing;
            
            var marchCompute = shapeSettings.marchingCubesCompute;
            marchCompute.SetInt("numPointsPerAxis", size + 1);
            marchCompute.SetFloat("isoLevel", surface);

            var marchKernelIndex = marchCompute.FindKernel("March");

            marchCompute.SetBuffer(marchKernelIndex, "points", pointsBuffer);

            int numVoxels = size * size * size;
            int maxTriangleCount = numVoxels * 5;
            const int stride = (sizeof(float) * 3 + sizeof(int) * 2) * 3;
            var triBuffer = new ComputeBuffer(maxTriangleCount, stride, ComputeBufferType.Append);
            triBuffer.SetCounterValue(0);
            marchCompute.SetBuffer(marchKernelIndex, "triangles", triBuffer);

            const int threadGroupSize = 8;
            int numThreadsPerAxis = Mathf.CeilToInt (size / (float) threadGroupSize);
            marchCompute.Dispatch(marchKernelIndex, numThreadsPerAxis, numThreadsPerAxis, numThreadsPerAxis);

            var triCountBuffer = new ComputeBuffer(1, sizeof(int), ComputeBufferType.Raw);
            ComputeBuffer.CopyCount(triBuffer, triCountBuffer, 0);
            int[] triCountArray = {0};
            triCountBuffer.GetData(triCountArray);
            int numTris = triCountArray[0];

            var tris = new Triangle[numTris];
            triBuffer.GetData(tris, 0, 0, numTris);
            
            triCountBuffer.Release();
            triBuffer.Release();
            pointsBuffer.Release();

            // MESH GENERATION
            var terrainTf = parent.Find("TerrainMesh");
            if (terrainTf != null)
            {
                if (Application.isPlaying)
                {
                    Object.Destroy(terrainTf.gameObject);
                }
                else
                {
                    Object.DestroyImmediate(terrainTf.gameObject);
                }
            }
            terrainTf = new GameObject("TerrainMesh").transform;
            terrainTf.SetParent(parent);
            terrainTf.localPosition = Vector3.zero;

            densitySettings.SetupMaterial(shadingSettings.material);
            GenerateMesh(terrainTf, tris, shadingSettings.material);
        }

        private struct VertexData
        {
            public int edgeIndex;
            public int marchIndex; 
            public Vector3 pos;
        }
        
        private struct Triangle
        {
            public VertexData vertexA;
            public VertexData vertexB;
            public VertexData vertexC;

            public VertexData this[int i]
            {
                get
                {
                    return i switch
                    {
                        0 => vertexA,
                        1 => vertexB,
                        2 => vertexC,
                    };
                }
            }
        }

        private void GenerateMesh(Transform terrainTf, Triangle[] triangles, Material material)
        {
            DetermineTriangles(triangles);
            
            int maxVertsPerMesh = 30000;
            var numVerts = verts.Count;
            int numMeshes = verts.Count / maxVertsPerMesh + 1;

            material.SetFloat("_MinHeight", -size * 0.5f * spacing);
            material.SetFloat("_MaxHeight", size * 0.5f * spacing);
            material.SetFloat("_Size", size);
            
            ConstructSubMesh(terrainTf, verts, triIndices, material);
        }

        int vertexIndex = 0;
        private Dictionary<Vector2Int, int> vertexIndexMap = new Dictionary<Vector2Int, int>();
        private List<Vector3> verts = new List<Vector3>();
        private List<int> triIndices = new List<int>();

        private void DetermineTriangles(Triangle[] triangles)
        {
            vertexIndexMap.Clear();
            verts.Clear();
            triIndices.Clear();
            vertexIndex = 0;
            
            foreach (var triangle in triangles)
            {
                AddVertex(triangle.vertexA);
                AddVertex(triangle.vertexB);
                AddVertex(triangle.vertexC);
            }
        }

        private void AddVertex(VertexData vert)
        {
            var id = new Vector2Int(vert.edgeIndex, vert.marchIndex);
            if (vertexIndexMap.ContainsKey(id))
            {
                var sharedVertexIndex = vertexIndexMap[id];
                triIndices.Add(sharedVertexIndex);
            }
            else
            {
                vertexIndexMap.Add(id, vertexIndex);
                triIndices.Add(vertexIndex);
                verts.Add(vert.pos);
                vertexIndex++;
            }
        }

        private void ConstructSubMesh(Transform terrainTf, List<Vector3> verts, List<int> indices, Material material)
        {
            Mesh mesh = new Mesh();
            mesh.SetVertices(verts);
            mesh.SetTriangles(indices, 0);
            mesh.RecalculateBounds();
            mesh.Optimize();
            mesh.RecalculateNormals();

            GameObject go = new GameObject("Mesh");
            go.transform.parent = terrainTf;
            go.AddComponent<MeshFilter>();
            go.AddComponent<MeshRenderer>();
            go.GetComponent<Renderer>().material = material;
            go.GetComponent<MeshFilter>().mesh = mesh;
            go.transform.localPosition = Vector3.zero;// new Vector3(-size / 2f, -size / 2f, -size / 2f);
        }
    }
}