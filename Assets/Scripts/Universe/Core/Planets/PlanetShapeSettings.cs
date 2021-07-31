using System;
using UnityEngine;

namespace Aspekt.Universe.Planets
{
    [CreateAssetMenu(menuName = "Aspekt.Universe/Planet Shape Settings", fileName = "PlanetShapeSettings")]
    public class PlanetShapeSettings : SettingsComponent
    {
        [Range(0.5f, 200f)]
        public float radius;

        protected override void OnValidate()
        {
            radius = Mathf.Max(radius, 0.5f);
            
            base.OnValidate();
        }
    }
}