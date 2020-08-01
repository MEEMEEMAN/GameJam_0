using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Assertions;
using System;
using System.IO;
using System.Linq;

namespace Perhaps
{
    [CustomEditor(typeof(PUIManager))]
    public class PUIManager_Inspector : Editor
    {
        PUIManager manager;

        private void OnEnable()
        {
            manager = (PUIManager)target;
        }

        string[] presets;
        int selectedPresetIndex = 0;
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Save current as new preset"))
            {
                Debug.Log("Creating new preset...");

                if (manager.presets == null)
                    manager.presets = new List<PUIPreset>();

                PUIPreset preset = PUIPreset.SavePreset(manager);
                manager.presets.Add(preset);

                preset.presetName = $"#{manager.presets.Count} Preset";
                Debug.Log($"Created preset \"{preset.presetName}\"!");
            }

            if (manager.presets == null)
                return;

            PerhapsEditorUtil.DrawHorizontalLine(Color.gray, 1);

            if (GUILayout.Button("Override selected"))
            {
                PUIPreset overridingPreset = PUIPreset.SavePreset(manager);

                PUIPreset overridenPreset = manager.presets[selectedPresetIndex];
                overridingPreset.presetName = overridenPreset.presetName;

                manager.presets[selectedPresetIndex] = overridingPreset;
                Debug.Log($"Overriding Preset \"{overridingPreset.presetName}\"...");
            }

            
            if (presets == null || presets.Length != manager.presets.Count)
                presets = manager.presets.Select(x => x.presetName).ToArray();

            selectedPresetIndex = EditorGUILayout.Popup("Selected Preset", selectedPresetIndex, presets);
            PerhapsEditorUtil.DrawHorizontalLine(Color.gray, 1);

            GUIStyle style = EditorStyles.boldLabel;
            GUILayout.Label("Apply Preset", style);
            for (int i = 0; i < manager.presets.Count; i++)
            {
                if (GUILayout.Button(manager.presets[i].presetName))
                {
                    Debug.Log($"Applying \"{manager.presets[i].presetName}\"...");
                    manager.ApplyPreset(manager.presets[i]);
                }
            }

        }
    }

}