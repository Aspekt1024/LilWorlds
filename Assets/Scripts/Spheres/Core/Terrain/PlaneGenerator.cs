using Aspekt.Noise;
using UnityEngine;

namespace Aspekt.Spheres
{
    public class PlaneGenerator
    {
        private readonly Transform parent;
        
        public PlaneGenerator(Transform parent)
        {
            this.parent = parent;
        }
        
        public void Generate(Plane.DebugSettings debugSettings, TerrainShapeSettings shapeSettings, PerlinNoiseSettings noiseSettings, TerrainColorSettings colorSettings)
        {
            var resolution = TerrainResolutionUtil.GetResolutionSize(debugSettings.resolution);
            var skipInterval = TerrainResolutionUtil.GetSkipInterval(debugSettings.lod);
            
            var heightMap = NoiseUtil.GeneratePerlinNoise(resolution + 1, resolution + 1, Vector2.zero, noiseSettings);
            var meshData = CalculateMeshData(heightMap, shapeSettings, skipInterval, debugSettings.useTriplanarNormals);
            
            var mesh = GetMesh(meshData);
            mesh.SetVertices(meshData.Vertices);
            mesh.SetTriangles(meshData.Triangles, 0, true);
            mesh.uv = meshData.UVs;

            if (debugSettings.useTriplanarNormals)
            {
                mesh.normals = meshData.Normals;
            }
            else
            {
                mesh.RecalculateNormals();
            }
            
            GenerateColor(heightMap, debugSettings, colorSettings);
        }

        private MeshData CalculateMeshData(float[,] heightMap, TerrainShapeSettings settings, int skipInterval, bool useTriplanarNormals)
        {
            var width = heightMap.GetLength(0);
            var height = heightMap.GetLength(1);
            
            var meshData = new MeshData(width, height, skipInterval);
            var bottomLeft = new Vector2(-settings.size / 2, -settings.size / 2);

            var meshWidth = (width - 1) / skipInterval + 1;
            var meshHeight = (height - 1) / skipInterval + 1;
            for (int x = 0; x < width; x += skipInterval)
            {
                for (int y = 0; y < height; y += skipInterval)
                {
                    var percent = new Vector2((float) x / (width - 1), (float) y / (height - 1));
                    var vertexHeight = heightMap[x, y] * settings.heightMultiplier * settings.heightCurve.Evaluate(heightMap[x, y]);
                    var vertexPos = new Vector3(bottomLeft.x + percent.x * settings.size, vertexHeight, bottomLeft.y + percent.y * settings.size);
                    
                    var index = x / skipInterval * meshHeight + y / skipInterval;
                    meshData.AddVertex(vertexPos, percent, index);

                    if (x < width - skipInterval && y < height - skipInterval)
                    {
                        meshData.AddTriangle(index, index + meshWidth + 1, index + meshWidth);
                        meshData.AddTriangle(index, index + 1, index + meshWidth + 1);
                    }
                }
            }
            
            meshData.Finalise(useTriplanarNormals);
            return meshData;
        }
        

        private void GenerateColor(float[,] heightMap, Plane.DebugSettings debugSettings, TerrainColorSettings colorSettings)
        {
            var meshRenderer = parent.GetComponentInChildren<MeshRenderer>();
            if (meshRenderer.sharedMaterial != null && meshRenderer.sharedMaterial.shader.name != "Standard") return;
            var mat = new Material(Shader.Find("Standard"));
            mat.mainTexture = TextureGenerator.GenerateTexture(heightMap, colorSettings, debugSettings.colourWeight);
            meshRenderer.sharedMaterial = mat;
        }

        private Mesh GetMesh(MeshData meshData)
        {
            var meshFilter = parent.GetComponentInChildren<MeshFilter>();
            if (meshFilter == null)
            {
                var go = new GameObject("Plane");
                go.transform.SetParent(parent);
                go.transform.localPosition = Vector3.zero;
                go.transform.localRotation = Quaternion.identity;
                
                go.AddComponent<MeshRenderer>();
                meshFilter = go.AddComponent<MeshFilter>();
                meshFilter.sharedMesh = new Mesh();
            }

            if (meshFilter.sharedMesh.vertices.Length != meshData.Vertices.Length)
            {
                meshFilter.sharedMesh = new Mesh();
            }

            return meshFilter.sharedMesh;
        }

    }
}