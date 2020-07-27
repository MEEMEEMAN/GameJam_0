using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Perhaps
{
    [CustomEditor(typeof(UIMoverPerhaps))]
    public class UIMoverInspector : UnityEditor.Editor
    {
        UIMoverPerhaps mover;

        void OnEnable()
        {
            mover = (UIMoverPerhaps)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if(GUILayout.Button("Write begin"))
            {
                mover.WriteBegin();
            }

            if (GUILayout.Button("Write end"))
            {
                mover.WriteEnd();
            }

            if (GUILayout.Button("Transition Opposite Instant"))
            {
                mover.TransitionOpposite(true);
            }

            if(Application.isPlaying)
            {
                if(GUILayout.Button("Transition Opposite Animated"))
                {
                    mover.TransitionOpposite();
                }
            }

        }
    }
}
