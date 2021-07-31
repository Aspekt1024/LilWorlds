using System;
using UnityEngine;

namespace Aspekt.Spheres
{
    [CreateAssetMenu(menuName = "Aspekt/Planets/Shading Settings", fileName = "PlanetShadingSettings")]
    public class PlanetShadingSettings : ScriptableObject
    {
        public Material material;

        public event Action OnSettingsChanged;

        private void OnValidate()
        {
            OnSettingsChanged?.Invoke();
        }
    }
}