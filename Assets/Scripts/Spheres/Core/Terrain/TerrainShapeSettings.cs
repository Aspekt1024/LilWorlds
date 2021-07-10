using UnityEngine;

namespace Aspekt.Spheres
{
    [CreateAssetMenu(menuName = "Aspekt Terrain/Terrain Shape Settings", fileName = "TerrainShapeSettings")]
    public class TerrainShapeSettings : ScriptableObject
    {
        public float size;
        [Range(0f, 2f)]
        public float heightMultiplier;
        public AnimationCurve heightCurve;
    }
}