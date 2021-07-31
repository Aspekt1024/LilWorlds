using UnityEngine;

namespace Aspekt.Universe.Planets
{
    [ExecuteInEditMode]
    public class Planet : MonoBehaviour
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
                    Debug.Log("Change action");
                    settingsChanged = false;
                    Generate();
                }
            }
        }

        private void OnValidate()
        {
            if (shapeSettings != null)
            {
                shapeSettings.OnSettingsChanged -= OnSettingsChanged;
                shapeSettings.OnSettingsChanged += OnSettingsChanged;
            }
            
            OnSettingsChanged();
        }
    }
}