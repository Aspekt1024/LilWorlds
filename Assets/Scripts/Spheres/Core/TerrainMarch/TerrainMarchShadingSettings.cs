using System;
using UnityEngine;

namespace Aspekt.Spheres
{
    [CreateAssetMenu(menuName = "Aspekt.TerrainMarch/Shading Settings", fileName = "TerrainShadingSettings")]
    public class TerrainMarchShadingSettings : ScriptableObject
    {
        public Material material;

        public event Action OnSettingsChanged;

        private void OnValidate()
        {
            OnSettingsChanged?.Invoke();
        }
    }
}