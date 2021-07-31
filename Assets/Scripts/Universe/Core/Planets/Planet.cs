using System.Collections.Generic;
using UnityEngine;

namespace Aspekt.Universe.Planets
{
    [ExecuteInEditMode]
    public class Planet : MonoBehaviour, IMultiSettingsParent
    {
        public PlanetShapeSettings shapeSettings;

        private bool settingsChanged;

        private void Generate()
        {
            
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
                    Debug.Log("change action");
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
    }
}