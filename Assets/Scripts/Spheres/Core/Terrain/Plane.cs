using System;
using Aspekt.Noise;
using UnityEngine;

namespace Aspekt.Spheres
{
    public class Plane : MonoBehaviour
    {
        [Serializable]
        public struct DebugSettings
        {
            public bool useTriplanarNormals;
            [Range(0f, 1f)]
            public float colourWeight;
            
            public TerrainResolutionUtil.Resolutions resolution;
            public TerrainResolutionUtil.LOD lod;
        }

        public DebugSettings debugSettings;

        public TerrainShapeSettings shapeSettings;
        public PerlinNoiseSettings noiseSettings;
        public TerrainColorSettings colorSettings;

        private PlaneGenerator generator;

        public void OnSettingsChanged()
        {
            Generate();
        }

        private void Generate()
        {
            generator ??= new PlaneGenerator(transform);
            generator.Generate(debugSettings, shapeSettings, noiseSettings, colorSettings);
        }
    }
}