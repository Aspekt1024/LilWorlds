using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Aspekt.Spheres
{
    public class PlanetMesh
    {
        private readonly Transform parent;
        private List<GameObject> meshes = new List<GameObject>();

        private int size;
        private float surface;
        private Vector3[] grid;
        private float[] noise;
        
        public PlanetMesh(Transform parent)
        {
            this.parent = parent;
        }

        public void Generate(PlanetShapeSettings shapeSettings, PlanetShadingSettings shadingSettings)
        {
            var mainKernel = shapeSettings.generator.FindKernel("CSMain");


            size = shapeSettings.size;
            surface = shapeSettings.surface;
            var length = (size + 1) * (size + 1) * (size + 1);
            
            shapeSettings.generator.SetFloat("size", size + 1);
            shapeSettings.generator.SetFloat("core", shapeSettings.coreRadius);

            grid = GenerateGrid(size);
            ComputeBuffer gridBuffer = null;
            ComputeHelper.CreateStructuredBuffer(ref gridBuffer, grid);
            shapeSettings.generator.SetBuffer(mainKernel, "grid", gridBuffer);

            ComputeBuffer noiseBuffer = null;
            ComputeHelper.CreateAndSetBuffer<float>(ref noiseBuffer, length, shapeSettings.generator, "noiseData", mainKernel);
            ComputeHelper.Run(shapeSettings.generator, noiseBuffer.count, kernelIndex: mainKernel);
                
            var noiseData = new float[noiseBuffer.count];
            noiseBuffer.GetData (noiseData);
            
            ComputeHelper.Release(noiseBuffer);
            ComputeHelper.Release(gridBuffer);

            var verts = new List<Vector3>();
            var indices = new List<int>();
            var mc = new MarchingCubes.MarchingCubesv2(shapeSettings.surface);
            mc.Generate(noiseData, size, size, size, verts, indices);

            var planetTf = parent.Find("PlanetMesh");
            if (planetTf == null)
            {
                planetTf = new GameObject("PlanetMesh").transform;
                planetTf.SetParent(parent);
                planetTf.localPosition = Vector3.zero;
            }
            
            GenerateMesh(planetTf, verts, indices, shapeSettings.size, shadingSettings.material);

            noise = noiseData.ToArray();
        }

        public void DrawGizmos()
        {
            if (!noise.Any() || !grid.Any()) return;
            
            if (noise.Length > 20000) return;

            for (int i = 0; i < noise.Length; i++)
            {
                var aboveSurface = noise[i] >= surface;
                Gizmos.color = aboveSurface ? Color.white : Color.black;
                Gizmos.DrawSphere(grid[i] * size - Vector3.one * (size * 0.5f), 0.1f);
            }
        }

        private Vector3[] GenerateGrid(int size)
        {
            var grid = new Vector3[(size + 1) * (size + 1) * (size + 1)];
            var idx = 0;
            for (int x = 0; x < size + 1; x++)
            {
                for (int y = 0; y < size + 1; y++)
                {
                    for (int z = 0; z < size + 1; z++)
                    {
                        grid[idx] = new Vector3(x, y, z) / size;// - Vector3.one * 0.5f;
                        idx++;
                    }
                }
            }

            return grid;
        }

        private void GenerateMesh(Transform planetTf, List<Vector3> verts, List<int> indices, int radius, Material material)
        {
            int maxVertsPerMesh = 30000;
            int numMeshes = verts.Count / maxVertsPerMesh + 1;

            foreach (Transform childTf in planetTf)
            {
                if (childTf == planetTf) continue;
                if (Application.isPlaying)
                {
                    Object.Destroy(childTf.gameObject);
                }
                else
                {
                    Object.DestroyImmediate(childTf.gameObject);
                }
            }
            meshes.Clear();

            for (int i = 0; i < numMeshes; i++)
            {

                List<Vector3> splitVerts = new List<Vector3>();
                List<int> splitIndices = new List<int>();

                for (int j = 0; j < maxVertsPerMesh; j++)
                {
                    int idx = i * maxVertsPerMesh + j;

                    if (idx < verts.Count)
                    {
                        splitVerts.Add(verts[idx]);
                        splitIndices.Add(j);
                    }
                }

                if (splitVerts.Count == 0) continue;

                Mesh mesh = new Mesh();
                mesh.SetVertices(splitVerts);
                mesh.SetTriangles(splitIndices, 0);
                mesh.RecalculateBounds();
                mesh.RecalculateNormals();

                GameObject go = new GameObject("Mesh");
                go.transform.parent = planetTf;
                go.AddComponent<MeshFilter>();
                go.AddComponent<MeshRenderer>();
                go.GetComponent<Renderer>().material = material;
                go.GetComponent<MeshFilter>().mesh = mesh;
                go.transform.localPosition = new Vector3(-radius / 2f, -radius / 2f, -radius / 2f);

                meshes.Add(go);
            }
        }
    }
}