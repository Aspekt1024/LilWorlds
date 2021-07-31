using UnityEngine;

namespace Aspekt.Universe.Planets
{
    [CreateAssetMenu(menuName = "Aspekt.Universe/Planet Density Settings", fileName = "PlanetDensitySettings")]
    public class PlanetDensitySettings : SettingsComponent
    {
        [Tooltip("The spacing between vertices")]
        [Range(0.05f, 1f)] public float spacing;
        
        public void Generate()
        {
            
        }
    }
}