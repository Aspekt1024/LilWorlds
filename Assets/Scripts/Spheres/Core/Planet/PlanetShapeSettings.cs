using System;
using UnityEngine;

namespace Aspekt.Spheres
{
    [CreateAssetMenu(menuName = "Aspekt/Planets/Shape Settings", fileName = "PlanetShapeSettings")]
    public class PlanetShapeSettings : ScriptableObject
    {
        public ComputeShader generator;
        [Range(-0.999f, 0.999f)]
        public float surface;

        [Range(0f, 1f)]
        public float coreRadius;
        
        [Range(5, 100)]
        public int size;
        
        public event Action OnSettingsChanged;

        private void OnValidate()
        {
            OnSettingsChanged?.Invoke();
        }
    }
}