using System;
using UnityEngine;

namespace Aspekt.Spheres
{
    public class TerrainDensityGenerator
    {
        private const int threadGroupSize = 8;
        private readonly ComputeShader densityShader;

        private ComputeBuffer offsetsBuffer;

        public TerrainDensityGenerator(ComputeShader densityShader)
        {
            this.densityShader = densityShader;
        }

        [Serializable]
        public class Settings
        {
            public int numPointsPerAxis;
            public float boundsSize;
            public Vector3 worldBounds;
            public Vector3 centre;
            public Vector3 offset;
            public float spacing;
            
            public int seed;
            public int numOctaves = 4;
            public float lacunarity = 2;
            public float persistence = .5f;
            public float noiseScale = 1;
            public float noiseWeight = 1;
            public bool closeEdges;
            public float floorOffset = 1;
            public float weightMultiplier = 1;
            
            public float hardFloorHeight;
            public float hardFloorWeight;

            public Vector4 shaderParams;
        }
        
        public ComputeBuffer GenerateDensity (ComputeBuffer pointsBuffer, Settings settings) {

            // Noise parameters
            var prng = new System.Random (settings.seed);
            var offsets = new Vector3[settings.numOctaves];
            float offsetRange = 1000;
            
            for (int i = 0; i < settings.numOctaves; i++)
            {
                offsets[i] = new Vector3 ((float) prng.NextDouble () * 2 - 1, (float) prng.NextDouble () * 2 - 1, (float) prng.NextDouble () * 2 - 1) * offsetRange;
            }

            offsetsBuffer = new ComputeBuffer (offsets.Length, sizeof (float) * 3);
            offsetsBuffer.SetData (offsets);

            var centre = new Vector4(settings.centre.x, settings.centre.y, settings.centre.z, 0f);
            var offset = new Vector4(settings.offset.x, settings.offset.y, settings.offset.z, 0f);
            densityShader.SetVector ("centre", centre);
            densityShader.SetVector ("offset", offset);
            
            densityShader.SetInt ("octaves", Mathf.Max (1, settings.numOctaves));
            densityShader.SetFloat ("lacunarity", settings.lacunarity);
            densityShader.SetFloat ("persistence", settings.persistence);
            densityShader.SetFloat ("noiseScale", settings.noiseScale);
            densityShader.SetFloat ("noiseWeight", settings.noiseWeight);
            densityShader.SetBool ("closeEdges", settings.closeEdges);
            densityShader.SetBuffer (0, "offsets", offsetsBuffer);
            densityShader.SetFloat ("floorOffset", settings.floorOffset);
            densityShader.SetFloat ("weightMultiplier", settings.weightMultiplier);
            densityShader.SetFloat ("hardFloor", settings.hardFloorHeight);
            densityShader.SetFloat ("hardFloorWeight", settings.hardFloorWeight);
            densityShader.SetVector ("params", settings.shaderParams);

            int numThreadsPerAxis = Mathf.CeilToInt (settings.numPointsPerAxis / (float) threadGroupSize);
            // Points buffer is populated inside shader with pos (xyz) + density (w).
            densityShader.SetBuffer (0, "points", pointsBuffer);
            densityShader.SetInt ("numPointsPerAxis", settings.numPointsPerAxis);
            densityShader.SetFloat ("boundsSize", settings.boundsSize);
            densityShader.SetFloat ("spacing", settings.spacing);
            densityShader.SetVector("worldSize", settings.worldBounds);

            densityShader.Dispatch (0, numThreadsPerAxis, numThreadsPerAxis, numThreadsPerAxis);

            ReleaseBuffers();

            // Return voxel data buffer so it can be used to generate mesh
            return pointsBuffer;
        }

        private void ReleaseBuffers()
        {
            offsetsBuffer?.Release();
        }
        
    }
}