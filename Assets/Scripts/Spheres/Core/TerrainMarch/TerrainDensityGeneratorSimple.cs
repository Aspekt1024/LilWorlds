using System;
using UnityEngine;

namespace Aspekt.Spheres
{
    [CreateAssetMenu(menuName = "Aspekt.TerrainMarch/Simple Terrain Density", fileName = "SimpleTerrainDensity")]
    public class TerrainDensityGeneratorSimple : TerrainDensitySettings
    {

        public Vector3 centre;
        public Vector3 offset;
        
        [Range(1,10)]
        public int numOctaves = 4;
        
        public float lacunarity = 2;
        public float persistence = .5f;
        
        [Range(0.1f, 10f)]
        public float frequency = 1;
        
        [Range(0f, 3f)]
        public float noiseStrength = 1;

        public bool enableTerracing;
        
        [Range(0f, 10)]
        public float terrace;
        
        [Range(-45f, 45f)]
        public float octaveRotation;

        public enum OctaveRotateMode
        {
            LowRange = 0,
            MidRange = 10,
            HighRange = 20,
            LowHighRange = 30,
        }
        public OctaveRotateMode octaveRotateMode;
        
        public float floorOffset = 1;
        
        [Range(0f, 1f)]
        public float floorStrength = 1;
        
        protected override ComputeBuffer GenerateDensity (ComputeBuffer pointsBuffer)
        {
            var densityKernel = densityCompute.FindKernel("Density");
            densityCompute.SetBuffer (densityKernel, "points", pointsBuffer);

            var numPointsPerAxis = NumVoxels + 1;
            densityCompute.SetInt ("numPointsPerAxis", numPointsPerAxis);
            densityCompute.SetFloat ("spacing", Spacing);

            densityCompute.SetVector ("centre", new Vector4(centre.x, centre.y, centre.z, 0f));
            densityCompute.SetVector ("offset", new Vector4(offset.x, offset.y, offset.z, 0f));
            densityCompute.SetVector("worldSize", new Vector4(NumVoxels, NumVoxels, NumVoxels, 0f));
            
            densityCompute.SetInt ("octaves", Mathf.Max (1, numOctaves));
            densityCompute.SetFloat ("lacunarity", lacunarity);
            densityCompute.SetFloat ("persistence", persistence);
            densityCompute.SetFloat ("frequency", frequency);
            densityCompute.SetFloat ("noiseWeight", noiseStrength);
            densityCompute.SetBool("enableTerracing", enableTerracing);
            densityCompute.SetFloat ("terrace", terrace);
            densityCompute.SetFloat ("octaveRotation", octaveRotation);
            
            densityCompute.SetFloat ("floorOffset", floorOffset);
            densityCompute.SetFloat ("floorStrength", floorStrength);

            var rotMode = octaveRotateMode switch
            {
                OctaveRotateMode.LowRange => 1 << 0 | 1 << 1,
                OctaveRotateMode.MidRange => 1 << 3 | 1 << 4 | 1 << 5,
                OctaveRotateMode.HighRange => 1 << 7 | 1 << 8,
                OctaveRotateMode.LowHighRange => 1 << 0 | 1 << 1 | 1 << 7 | 1 << 8,
                _ => 0
            };
            densityCompute.SetInt("octaveRotateMode", rotMode);

            const int threadGroupSize = 8;
            int numThreadsPerAxis = Mathf.CeilToInt (numPointsPerAxis / (float) threadGroupSize);
            densityCompute.Dispatch (densityKernel, numThreadsPerAxis, numThreadsPerAxis, numThreadsPerAxis);

            ReleaseBuffers();

            return pointsBuffer;
        }

        public override void SetupMaterial(Material material)
        {
            material.SetFloat("_FloorLevel", floorOffset);
        }

        private void ReleaseBuffers()
        {
        }
        
    }
}