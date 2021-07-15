using System;
using UnityEngine;

namespace Aspekt.Spheres
{
    [CreateAssetMenu(menuName = "Aspekt.Spheres/Sphere Shape Settings", fileName = "SphereShapeSettings")]
    public class SphereShapeSettings : ScriptableObject
    {
        public float radius;
        [Range(1, 100)]
        public int resolution;

        public ComputeShader heightShader;

        [Serializable]
        public struct Modifiers
        {
            public float frequency;
            public float offset;
            public float strength;
        }
        public Modifiers modifiers;
        
        public event Action OnSettingsChanged;

        private void OnValidate()
        {
            radius = Mathf.Max(radius, 0.1f);
            resolution = Mathf.Clamp(resolution, 0, 100);
            
            OnSettingsChanged?.Invoke();
        }
    }
}