using UnityEditor;
using UnityEngine;

namespace Aspekt.Universe.Planets
{
    [CustomEditor(typeof(Planet))]
    public class PlanetEditor : MultiSettingsEditor
    {
        private Planet planet;
        
        public override void OnInspectorGUI()
        {
            using (var check = new EditorGUI.ChangeCheckScope())
            {
                DrawDefaultInspector();
                if (check.changed)
                {
                    planet.OnSettingsChanged();
                    EditorApplication.QueuePlayerLoopUpdate();
                }
            }

            if (GUILayout.Button("Generate"))
            {
                planet.OnSettingsChanged();
                EditorApplication.QueuePlayerLoopUpdate();
            }
            
            DrawSettingsEditors();
        }

        private void OnEnable()
        {
            planet = (Planet) target;
            
            SetupSettingsEditors(planet.shapeSettings);
        }
    }
}