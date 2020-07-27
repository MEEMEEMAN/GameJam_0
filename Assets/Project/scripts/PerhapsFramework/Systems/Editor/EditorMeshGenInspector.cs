using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Perhaps.Procedural;
using Perhaps;

[ExecuteInEditMode]
[CustomEditor(typeof(EditorMeshGenerator))]
public class EditorMeshGenInspector : Editor
{
    EditorMeshGenerator generator;
    private void OnEnable()
    {
        generator = (EditorMeshGenerator)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Generate"))
        {
            generator.GenerateMesh();
        }
    }
}