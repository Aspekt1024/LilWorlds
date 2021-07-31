using System;
using UnityEngine;

namespace Aspekt.Spheres
{
    [ExecuteInEditMode]
    public class Planet : MonoBehaviour
    {
        public PlanetShapeSettings shapeSettings;
        public PlanetShadingSettings shadingSettings;

        [Header("Debug")]
        public bool debugMode;
        
        private bool shapeSettingsChanged;
        private bool shadingSettingsChanged;

        private PlanetMesh planetMesh;

        [ContextMenu("Generate")]
        public void Generate()
        {
            planetMesh ??= new PlanetMesh(transform);
            planetMesh.Generate(shapeSettings, shadingSettings);
        }

        public void OnShapeSettingsChanged()
        {
            shapeSettingsChanged = true;
        }

        public void OnShadingSettingsChanged()
        {
            shadingSettingsChanged = true;
        }

        private void Update()
        {
            if (!Application.isPlaying)
            {
                if (shapeSettingsChanged || shadingSettingsChanged)
                {
                    shapeSettingsChanged = false;
                    shadingSettingsChanged = false;
                    Generate();
                }
            }
        }

        private void OnDrawGizmos()
        {
            if (debugMode)
            {
                planetMesh.DrawGizmos();
            }
        }

        private void OnValidate ()
        {
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