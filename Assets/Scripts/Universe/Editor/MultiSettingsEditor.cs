using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Aspekt
{
    public class MultiSettingsEditor<T> : Editor where T : IMultiSettingsParent
    {
        private struct SettingsEditor
        {
            public Editor editor;
            public SettingsComponent settings;
            public bool foldout;
        }
        private readonly List<SettingsEditor> settingsEditors = new List<SettingsEditor>();

        private T parent;

        protected void DrawEditors()
        {
            using (var check = new EditorGUI.ChangeCheckScope())
            {
                DrawDefaultInspector();
                if (check.changed)
                {
                    parent.OnSettingsChanged();
                    EditorApplication.QueuePlayerLoopUpdate();
                }
            }
            
            for (int i = 0; i < settingsEditors.Count; i++)
            {
                settingsEditors[i] = DrawSettingsEditor(settingsEditors[i]);
                EditorPrefs.SetBool(GetFoldoutPropName(settingsEditors[i].settings), settingsEditors[i].foldout);
            }
        }
        
        protected void SetupEditors(T parent, params SettingsComponent[] settingsComponents)
        {
            this.parent = parent;
            
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