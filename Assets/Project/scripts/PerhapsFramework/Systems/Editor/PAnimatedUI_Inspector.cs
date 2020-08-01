using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Perhaps
{
    [CustomEditor(typeof(PAnimatedElement))]
    public class PAnimatedUI_Inspector : Editor
    {
        PAnimatedElement element;

        private void OnEnable()
        {
            element = (PAnimatedElement)target;
        }

        public override void OnInspectorGUI()
        {

            if(GUILayout.Button("Set position as start"))
            {
                element.startPosition = element.WriteCurrentPosition();
            }

            if(GUILayout.Button("Set position as end"))
            {
                element.endPosition = element.WriteCurrentPosition();
            }

            if(GUILayout.Button("Transiton opposite instant"))
            {
                element.TransitionOpposite(true);
            }

            if (Application.isPlaying && GUILayout.Button("Transition opposite animated"))
            {
                element.TransitionOpposite();
            }

            base.OnInspectorGUI();


        }
    }

}