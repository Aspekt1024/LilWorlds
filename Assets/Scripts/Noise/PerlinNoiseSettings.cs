using UnityEngine;

namespace Aspekt.Noise
{
    
    [CreateAssetMenu(menuName = "Aspekt.Noise/Perlin", fileName = "Perlin Noise Settings")]
    public class PerlinNoiseSettings : ScriptableObject
    {
        public string seed;
        
        [Tooltip("When set to local, the noise map is normalised to the max value per individual noise map")]
        public NormalizeMode normalizeMode = NormalizeMode.Global;
            
        [Tooltip("Adjusts the noise scale.")]
        [Range(0.3f, 100f)]
        public float scale = 50;
        
        [Tooltip("The number of levels of detail the perlin noise will have")]
        [Range(1, 10)]
        public int octaves = 6;
        
        [Tooltip("How much each octave level contributes to the overall noise (adjusts amplitude)")]
        [Range(0f, 1f)]
        public float persistence = 0.6f;
        
        [Tooltip("How much detail is added or removed at each octave (adjusts frequency)")]
        [Range(1f, 10f)]
        public float lacunarity = 2f;

        [Tooltip("Adjusts the noise horizontal and vertical offset")]
        public Vector2 offset;

        private void OnValidate()
        {
            if (seed == null) seed = "";
        }
    }
}