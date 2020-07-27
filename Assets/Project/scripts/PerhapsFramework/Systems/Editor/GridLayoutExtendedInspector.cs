using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Perhaps;

[CustomEditor(typeof(GridLayoutExtended))]
public class GridLayoutExtendedInspector : Editor
{
    GridLayoutExtended grid;

    private void OnEnable()
    {
        grid = (GridLayoutExtended)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Build Grid"))
        {
            grid.Build();
        }
    }
}