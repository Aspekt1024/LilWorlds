using System;
using UnityEngine;

namespace Aspekt.Spheres
{
    [CreateAssetMenu(menuName = "Aspekt Terrain/Terrain Color Settings", fileName = "TerrainColorSettings")]
    public class TerrainColorSettings : ScriptableObject
    {
        public Gradient grad;
        public FilterMode textureFilterMode;
    }
}