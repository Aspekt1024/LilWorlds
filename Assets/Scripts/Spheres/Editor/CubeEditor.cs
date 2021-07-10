using System;
using UnityEditor;
using UnityEngine;

namespace Aspekt.Spheres
{
    [CustomEditor(typeof(Cube))]
    public class CubeEditor : Editor
    {
        private Cube cube;
        
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            if (GUILayout.Button("Generate"))
            {
                
            }
        }

        private void OnEnable()
        {
            cube = (Cube) target;
        }
    }
}