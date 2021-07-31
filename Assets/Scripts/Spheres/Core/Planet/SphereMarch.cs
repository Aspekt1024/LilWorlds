using System;
using UnityEngine;

namespace Aspekt.Spheres
{
    [ExecuteInEditMode]
    public class SphereMarch : MonoBehaviour
    {
        public SphereDensitySettings densitySettings;
        public TerrainMarchShapeSettings shapeSettings;
        public TerrainMarchShadingSettings shadingSettings;

        private bool densitySettingsChanged;
        private bool shapeSettingsChanged;
        private bool shadingSettingsChanged;

        private PlanetMarchMesh planetMesh;

        [ContextMenu("Generate")]
        public void Generate()
        {
            planetMesh ??= new PlanetMarchMesh(transform);
            planetMesh.Generate(densitySettings, shapeSettings, shadingSettings);
        }

        public void OnDensitySettingsChanged()
        {
            densitySettingsChanged = true;
        }

        public void OnShapeSettingsChanged()
        {
            shapeSettingsChanged = true;
        }

        public void OnShadingSettingsChanged()
        {
            shadingSettingsChanged = true;
        }

        private void Awake()
        {
            Generate();
        }

        private void Update()
        {
            if (!Application.isPlaying)
            {
                if (densitySettingsChanged || shapeSettingsChanged || shadingSettingsChanged)
                {
                    densitySettingsChanged = false;
                    shapeSettingsChanged = false;
                    shadingSettingsChanged = false;
                    Generate();
                }
            }
        }

        private void OnValidate ()
        {
            if (densitySettings != null)
            {
                densitySettings.OnSettingsChanged -= OnDensitySettingsChanged;
                densitySettings.OnSettingsChanged += OnDensitySettingsChanged;
            }
            if (shapeSettings != null)
            {
                shapeSettings.OnSettingsChanged -= OnShapeSettingsChanged;
                shapeSettings.OnSettingsChanged += OnShapeSettingsChanged;
            }
            if (shadingSettings != null)
            {
                shadingSettings.OnSettingsChanged -= OnShadingSettingsChanged;
                shadingSettings.OnSettingsChanged += OnShadingSettingsChanged;
            }

            OnShapeSettingsChanged();
        }
    }
}