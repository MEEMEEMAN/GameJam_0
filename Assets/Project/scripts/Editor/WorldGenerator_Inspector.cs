using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Game
{
    [CustomEditor(typeof(WorldGenerator))]
    public class WorldGenerator_Inspector : Editor
    {
        WorldGenerator gen;

        private void OnEnable()
        {
            gen = (WorldGenerator)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();


        }
    }

}