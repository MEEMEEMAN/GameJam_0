using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Perhaps.Editor
{
    [CustomEditor(typeof(PerhapsUIMoverConfigurationManager))]
    public class UIMoverConfigInspector : UnityEditor.Editor
    {
        PerhapsUIMoverConfigurationManager configManager;

        private void OnEnable()
        {
            configManager = (PerhapsUIMoverConfigurationManager)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var configs = configManager.uiMoverConfigurations;

            GUIStyle boldLabel = EditorStyles.boldLabel;

            GUILayout.Label("Transition in editor", boldLabel);

            const int rowCount = 4;
            for (int i = 0; i < configs.Length; i++)
            {
                string buttonText = configs[i].configName != null && configs[i].configName.Length > 0 ? configs[i].configName : i.ToString();
                if(GUILayout.Button(buttonText))
                {
                    configManager.ArrangeUIConfiguration(configs[i], true /*instant*/);
                }
            }
        }
    }

}