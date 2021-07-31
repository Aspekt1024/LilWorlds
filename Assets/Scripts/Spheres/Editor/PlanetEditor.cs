using UnityEditor;
using UnityEngine;

namespace Aspekt.Spheres
{
    [CustomEditor(typeof(Planet))]
    public class PlanetEditor : Editor
    {
        private Planet planet;
        
        private Editor shapeEditor;
        private Editor shadingEditor;

        private bool planetShapeFoldout;
        private bool planetShadingFoldout;

        public override void OnInspectorGUI()
        {
            using (var check = new EditorGUI.ChangeCheckScope())
            {
                DrawDefaultInspector();
                if (check.changed)
                {
                    Regenerate();
                }
            }

            if (GUILayout.Button("Generate"))
            {
                Regenerate();
            }

            DrawSettingsEditor(planet.shapeSettings, ref planetShapeFoldout, ref shapeEditor);
            DrawSettingsEditor(planet.shadingSettings, ref planetShadingFoldout, ref shadingEditor);
            SaveState();
        }
        
        private void Regenerate()
        {
            planet.OnShapeSettingsChanged();
            planet.OnShadingSettingsChanged();
            EditorApplication.QueuePlayerLoopUpdate();
        }
        
        private void DrawSettingsEditor(Object settings, ref bool foldout, ref Editor editor)
        {
            if (settings == null) return;
            foldout = EditorGUILayout.InspectorTitlebar(foldout, settings);
            if (foldout)
            {
                CreateCachedEditor(settings, null, ref editor);
                editor.OnInspectorGUI();
            }
        }

        private void OnEnable()
        {
            planetShapeFoldout = EditorPrefs.GetBool(nameof(planetShapeFoldout), false);
            planetShadingFoldout = EditorPrefs.GetBool(nameof(planetShadingFoldout), false);
            planet = (Planet) target;
        }
        
        private void SaveState()
        {
            EditorPrefs.SetBool(nameof(planetShapeFoldout), planetShapeFoldout);
            EditorPrefs.SetBool(nameof(planetShadingFoldout), planetShadingFoldout);
        }
    }
}