using System;
using UnityEngine;

namespace Aspekt.Spheres
{
    [CreateAssetMenu(menuName = "Aspekt.Spheres/Sphere Shape Settings", fileName = "SphereShapeSettings")]
    public class SphereShapeSettings : ScriptableObject
    {
        public float radius;
        public int resolution;
        
        public event Action OnSettingsChanged;

        private void OnValidate()
        {
            radius = Mathf.Max(radius, 0.1f);
            resolution = Mathf.Clamp(resolution, 0, 50);
            
            OnSettingsChanged?.Invoke();
        }
    }
}