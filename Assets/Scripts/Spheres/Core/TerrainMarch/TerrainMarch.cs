using UnityEngine;

namespace Aspekt.Spheres
{
    [ExecuteInEditMode]
    public class TerrainMarch : MonoBehaviour
    {
        public TerrainDensitySettings densitySettings;
        public TerrainMarchShapeSettings shapeSettings;
        public TerrainMarchShadingSettings shadingSettings;

        private bool densitySettingsChanged;
        private bool shapeSettingsChanged;
        private bool shadingSettingsChanged;

        private TerrainMarchMesh terrainMesh;

        [ContextMenu("Generate")]
        public void Generate()
        {
            terrainMesh ??= new TerrainMarchMesh(transform);
            terrainMesh.Generate(densitySettings, shapeSettings, shadingSettings);
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