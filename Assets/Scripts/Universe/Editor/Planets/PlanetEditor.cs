using UnityEditor;
using UnityEngine;

namespace Aspekt.Universe.Planets
{
    [CustomEditor(typeof(Planet))]
    public class PlanetEditor : MultiSettingsEditor<Planet>
    {
        private Planet planet;
        
        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("Generate"))
            {
                planet.OnSettingsChanged();
                EditorApplication.QueuePlayerLoopUpdate();
            }
            
            DrawEditors();
        }

        private void OnEnable()
        {
            planet = (Planet) target;
            
            SetupEditors(
                planet,
                planet.shapeSettings,
                planet.densitySettings
            );
        }
    }
}