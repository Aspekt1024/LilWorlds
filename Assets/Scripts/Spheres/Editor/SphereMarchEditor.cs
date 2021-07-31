using System;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Aspekt.Spheres
{
    [CustomEditor(typeof(SphereMarch))]
    public class SphereMarchEditor : Editor
    {
        private SphereMarch sphere;
        
        private Editor densityEditor;
        private Editor shapeEditor;
        private Editor shadingEditor;

        private bool sphereMarchDensityFoldout;
        private bool sphereMarchShapeFoldout;
        private bool sphereMarchShadingFoldout;

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

            DrawSettingsEditor(sphere.densitySettings, ref sphereMarchDensityFoldout, ref densityEditor);
            DrawSettingsEditor(sphere.shapeSettings, ref sphereMarchShapeFoldout, ref shapeEditor);
            DrawSettingsEditor(sphere.shadingSettings, ref sphereMarchShadingFoldout, ref shadingEditor);
            SaveState();
        }

        private void Regenerate()
        {
            sphere.OnDensitySettingsChanged();
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
            sphereMarchDensityFoldout = EditorPrefs.GetBool(nameof(sphereMarchDensityFoldout), false);
            sphereMarchShapeFoldout = EditorPrefs.GetBool(nameof(sphereMarchShapeFoldout), false);
            sphereMarchShadingFoldout = EditorPrefs.GetBool(nameof(sphereMarchShadingFoldout), false);
            sphere = (SphereMarch) target;
        }
        
        private void SaveState()
        {
            EditorPrefs.SetBool(nameof(sphereMarchDensityFoldout), sphereMarchDensityFoldout);
            EditorPrefs.SetBool(nameof(sphereMarchShapeFoldout), sphereMarchShapeFoldout);
            EditorPrefs.SetBool(nameof(sphereMarchShadingFoldout), sphereMarchShadingFoldout);
        }
    }
}