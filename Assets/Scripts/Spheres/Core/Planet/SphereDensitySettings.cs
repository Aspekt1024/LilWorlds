using System;
using UnityEngine;

namespace Aspekt.Spheres
{
    public abstract class SphereDensitySettings : ScriptableObject
    {
        public ComputeShader densityCompute;
        
        [Range(10, 200)]
        public int NumVoxels = 30;
        
        [Range(0.1f, 2f)]
        public float Spacing;
        
        public ComputeBuffer Generate()
        {
            var numPoints = (NumVoxels + 1) * (NumVoxels + 1) * (NumVoxels + 1);
            var pointsBuffer = new ComputeBuffer(numPoints, sizeof(float) * 4);

            return GenerateDensity(pointsBuffer);
        }

        protected abstract ComputeBuffer GenerateDensity(ComputeBuffer pointsBuffer);

        public abstract void SetupMaterial(Material material);
        
        public event Action OnSettingsChanged;

        private void OnValidate()
        {
            OnSettingsChanged?.Invoke();
        }
    }
}