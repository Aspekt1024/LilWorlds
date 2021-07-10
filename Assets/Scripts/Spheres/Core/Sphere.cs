using UnityEngine;

namespace Aspekt.Spheres
{
    [ExecuteInEditMode]
    public class Sphere : MonoBehaviour
    {
        public SphereShapeSettings shapeSettings;
        public SphereShadingSettings shadingSettings;

        private SphereMesh sphereMesh;

        private bool shapeSettingsChanged;
        private bool shadingSettingsChanged;

        private void Awake()
        {
            sphereMesh = new SphereMesh(transform);
        }

        private void Update()
        {
            if (!Application.isPlaying)
            {
                if (shapeSettingsChanged || shadingSettingsChanged)
                {
                    shapeSettingsChanged = false;
                    shadingSettingsChanged = false;
                    sphereMesh ??= new SphereMesh(transform);
                    sphereMesh.GenerateSphere(shapeSettings, shadingSettings);
                }
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

        public void OnShapeSettingsChanged()
        {
            shapeSettingsChanged = true;
        }

        public void OnShadingSettingsChanged()
        {
            shadingSettingsChanged = true;
        }
    }
}