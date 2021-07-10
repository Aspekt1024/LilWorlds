using System;
using UnityEngine;

namespace Aspekt.Spheres
{
    [CreateAssetMenu(menuName = "Aspekt.Spheres/Sphere Shading Settings", fileName = "SphereShadingSettings")]
    public class SphereShadingSettings : ScriptableObject
    {
        public Material material;
        
        public event Action OnSettingsChanged;

        private void OnValidate()
        {
            OnSettingsChanged?.Invoke();
        }
    }
}