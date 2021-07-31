using System.Collections.Generic;
using UnityEngine;

namespace Aspekt.Universe.Planets
{
    [ExecuteInEditMode]
    public class Planet : MonoBehaviour, IMultiSettingsParent
    {
        public PlanetShapeSettings shapeSettings;
        public PlanetDensitySettings densitySettings;

        private bool settingsChanged;

        private PlanetGenerator generator;
        private Transform planetModel;

        private void Generate()
        {
            generator ??= new PlanetGenerator();
            
            DestroyOriginalParent();
            
            planetModel = generator.Generate(shapeSettings, densitySettings);
            planetModel.SetParent(transform);
            planetModel.position = transform.position;
        }

        public void OnSettingsChanged()
        {
            settingsChanged = true;
        }

        private void Update()
        {
            if (!Application.isPlaying)
            {
                if (settingsChanged)
                {
                    settingsChanged = false;
                    Generate();
                }
            }
        }

        private void OnValidate()
        {
            var allSettings = new List<SettingsComponent>
            {
                shapeSettings,
                densitySettings,
            };

            foreach (var s in allSettings)
            {
                if (s != null)
                {
                    s.OnSettingsChanged -= OnSettingsChanged;
                    s.OnSettingsChanged += OnSettingsChanged;
                }
            }
            
            OnSettingsChanged();
        }
        
        private void DestroyOriginalParent()
        {
            if (planetModel == null)
            {
                planetModel = transform.Find("PlanetParent");
                if (planetModel == null) return;
            }
            
            if (Application.isPlaying)
            {
                Destroy(planetModel.gameObject);
            }
            else
            {
                DestroyImmediate(planetModel.gameObject);
            }
        }
    }
}