using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Aspekt
{
    public class MultiSettingsEditor : Editor
    {
        private struct SettingsEditor
        {
            public Editor editor;
            public SettingsComponent settings;
            public bool foldout;
        }
        private readonly List<SettingsEditor> settingsEditors = new List<SettingsEditor>();

        protected void DrawSettingsEditors()
        {
            for (int i = 0; i < settingsEditors.Count; i++)
            {
                settingsEditors[i] = DrawSettingsEditor(settingsEditors[i]);
                EditorPrefs.SetBool(GetFoldoutPropName(settingsEditors[i].settings), settingsEditors[i].foldout);
            }
        }
        
        protected void SetupSettingsEditors(params SettingsComponent[] settingsComponents)
        {
            settingsEditors.Clear();
            
            foreach (var settingsComponent in settingsComponents)
            {
                var settingsEditor = new SettingsEditor
                {
                    settings = settingsComponent,
                    foldout = EditorPrefs.GetBool(GetFoldoutPropName(settingsComponent)),
                };
                settingsEditors.Add(settingsEditor);
            }
        }
        
        private static SettingsEditor DrawSettingsEditor(SettingsEditor settingsEditor)
        {
            if (settingsEditor.settings == null) return settingsEditor;
            settingsEditor.foldout = EditorGUILayout.InspectorTitlebar(settingsEditor.foldout, settingsEditor.settings);
            if (settingsEditor.foldout)
            {
                CreateCachedEditor(settingsEditor.settings, null, ref settingsEditor.editor);
                settingsEditor.editor.OnInspectorGUI();
            }
            return settingsEditor;
        }

        private static string GetFoldoutPropName(Object obj)
        {
            return obj.GetType().Name + "Foldout";
        }
    }
}