using UnityEditor;
using UnityEngine;

namespace Aspekt.Spheres
{
    [CustomEditor(typeof(Plane))]
    public class PlaneEditor : Editor
    {
        private Plane plane;

        private bool shapeFoldout;
        private bool noiseFoldout;
        private bool colorFoldout;
        private Editor shapeEditor;
        private Editor noiseEditor;
        private Editor colorEditor;
        
        public override void OnInspectorGUI()
        {
            if (DrawDefaultInspector())
            {
                plane.OnSettingsChanged();
            }
            
            if (GUILayout.Button("Generate"))
            {
                plane.OnSettingsChanged();
            }

            if (DrawSettingsEditors())
            {
                plane.OnSettingsChanged();
            }
            
            SaveState();
        }

        private bool DrawSettingsEditors()
        {
            var changeDetected = DrawSettingsEditor(plane.shapeSettings, ref shapeFoldout, ref shapeEditor);
            changeDetected |= DrawSettingsEditor(plane.noiseSettings, ref noiseFoldout, ref noiseEditor);
            changeDetected |= DrawSettingsEditor(plane.colorSettings, ref colorFoldout, ref colorEditor);
            return changeDetected;
        }

        private void OnEnable()
        {
            plane = (Plane) target;
            shapeFoldout = EditorPrefs.GetBool(nameof(shapeFoldout));
            noiseFoldout = EditorPrefs.GetBool(nameof(noiseFoldout));
            colorFoldout = EditorPrefs.GetBool(nameof(colorFoldout));
        }
        
        private void SaveState()
        {
            EditorPrefs.SetBool(nameof(shapeFoldout), shapeFoldout);
            EditorPrefs.SetBool(nameof(noiseFoldout), noiseFoldout);
            EditorPrefs.SetBool(nameof(colorFoldout), colorFoldout);
        }
        
        private bool DrawSettingsEditor(Object settings, ref bool foldout, ref Editor editor)
        {
            if (settings == null) return false;
            foldout = EditorGUILayout.InspectorTitlebar(foldout, settings);
            if (foldout)
            {
                CreateCachedEditor(settings, null, ref editor);
                return editor.DrawDefaultInspector();
            }

            return false;
        }
    }
}