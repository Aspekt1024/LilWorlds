using UnityEditor;
using UnityEngine;

namespace Aspekt.Spheres
{
    [CustomEditor(typeof(Sphere))]
    public class SphereEditor : Editor
    {
        private Sphere sphere;

        private Editor shapeEditor;
        private Editor shadingEditor;

        private bool shapeFoldout;
        private bool shadingFoldout;

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

            DrawSettingsEditor(sphere.shapeSettings, ref shapeFoldout, ref shapeEditor);
            DrawSettingsEditor(sphere.shadingSettings, ref shadingFoldout, ref shadingEditor);
            SaveState();
        }
        
        private void Regenerate()
        {
            sphere.OnShapeSettingsChanged();
            sphere.OnShadingSettingsChanged();
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
            shapeFoldout = EditorPrefs.GetBool(nameof(shapeFoldout), false);
            shadingFoldout = EditorPrefs.GetBool(nameof(shadingFoldout), false);
            sphere = (Sphere) target;
        }
        
        private void SaveState()
        {
            EditorPrefs.SetBool(nameof(shapeFoldout), shapeFoldout);
            EditorPrefs.SetBool(nameof(shadingFoldout), shadingFoldout);
        }
    }
}