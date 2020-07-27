using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Perhaps.Editor;

namespace Perhaps
{
    [CustomEditor(typeof(NavGrid2D))]
    public class GridDataEditor : UnityEditor.Editor
    {
        NavGrid2D grid;

        private void OnEnable()
        {
            if(target != null)
            {
                grid = (NavGrid2D)target;
            }
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (grid.data == null)
            {
                var style = new GUIStyle();
                style.normal.textColor = Color.red;

                GUILayout.Label("Please add a NavGrid2DData object!", style);
                return;
            }

            grid.data.fillColor = EditorGUILayout.ColorField("Fill Color", grid.data.fillColor);
            grid.data.outlineColor = EditorGUILayout.ColorField("Outline Color", grid.data.outlineColor);
            if (GUILayout.Button("Bake Grid"))
            {
                grid.BakeGrid();
                EditorUtility.SetDirty(grid.data);
            }

            GUILayout.Label($"{grid.data.grid.Length} Nodes In total.");
        }

        void OnSceneGUI()
        {
            if (grid.data == null)
                return;

            Rect r = RectExt.ResizeRect(grid.data.gridRect, Handles.CubeHandleCap, grid.data.outlineColor, grid.data.fillColor, HandleUtility.GetHandleSize(Vector3.zero) * 0.1f, 0.1f);
            grid.data.gridRect = r;
        }
    }
}