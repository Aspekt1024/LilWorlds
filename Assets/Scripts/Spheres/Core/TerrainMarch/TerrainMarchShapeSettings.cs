using System;
using UnityEngine;

namespace Aspekt.Spheres
{
    [CreateAssetMenu(menuName = "Aspekt.TerrainMarch/Shape Settings", fileName = "TerrainShapeSettings")]
    public class TerrainMarchShapeSettings : ScriptableObject
    {
        public ComputeShader marchingCubesCompute;
        
        [Range(-50f, 50f)]
        public float surface;

        [Range(0f, 1f)]
        public float floorLevel;
        
        public event Action OnSettingsChanged;

        private void OnValidate()
        {
            OnSettingsChanged?.Invoke();
        }
    }
}