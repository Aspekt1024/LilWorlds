using System;
using UnityEngine;

namespace Aspekt.Universe.Planets
{
    [CreateAssetMenu(menuName = "Aspekt.Universe/Planet Shape Settings", fileName = "PlanetShapeSettings")]
    public class PlanetShapeSettings : SettingsComponent
    {
        public float radius;

        private void OnValidate()
        {
            radius = Mathf.Max(radius, 0.5f);
        }
    }
}