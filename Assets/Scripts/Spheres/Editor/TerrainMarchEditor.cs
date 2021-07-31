using UnityEditor;
using UnityEngine;

namespace Aspekt.Spheres
{
    [CustomEditor(typeof(TerrainMarch))]
    public class TerrainMarchEditor : Editor
    {
        private TerrainMarch terrain;
        
        private Editor densityEditor;
        private Editor shapeEditor;
        private Editor shadingEditor;

        private bool terrainMarchDensityFoldout;
        private bool terrainMarchShapeFoldout;
        private bool terrainMarchShadingFoldout;

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

            DrawSettingsEditor(terrain.densitySettings, ref terrainMarchDensityFoldout, ref densityEditor);
            DrawSettingsEditor(terrain.shapeSettings, ref terrainMarchShapeFoldout, ref shapeEditor);
            DrawSettingsEditor(terrain.shadingSettings, ref terrainMarchShadingFoldout, ref shadingEditor);
            SaveState();
        }
        
        private void Regenerate()
        {
            terrain.OnDensitySettingsChanged();
            terrain.OnShapeSettingsChanged();
            terrain.OnShadingSettingsChanged();
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
            terrainMarchDensityFoldout = EditorPrefs.GetBool(nameof(terrainMarchDensityFoldout), false);
            terrainMarchShapeFoldout = EditorPrefs.GetBool(nameof(terrainMarchShapeFoldout), false);
            terrainMarchShadingFoldout = EditorPrefs.GetBool(nameof(terrainMarchShadingFoldout), false);
            terrain = (TerrainMarch) target;
        }
        
        private void SaveState()
        {
            EditorPrefs.SetBool(nameof(terrainMarchDensityFoldout), terrainMarchDensityFoldout);
            EditorPrefs.SetBool(nameof(terrainMarchShapeFoldout), terrainMarchShapeFoldout);
            EditorPrefs.SetBool(nameof(terrainMarchShadingFoldout), terrainMarchShadingFoldout);
        }
    }
}