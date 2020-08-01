using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Perhaps
{
    /// <summary>
    /// The PerhapsFramework's UI root node.
    /// </summary>
    [RequireComponent(typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster))]
    public class PUIManager : MonoBehaviour
    {
        #region Variables
        public static PUIManager instance { get; private set; }

        [SerializeField] Canvas canvas;
        [SerializeField] CanvasScaler scaler;
        [SerializeField] GraphicRaycaster raycaster;
        public  PUIElement[] elements;

        [Tooltip("Presets allow you to save a configuration of UI elements and load it on demand")]
        [Header("Presets")]
        public List<PUIPreset> presets;
        #endregion

        private void OnValidate()
        {
            if (canvas == null)
                canvas = GetComponent<Canvas>();

            if (scaler == null)
                scaler = GetComponent<CanvasScaler>();

            if (raycaster == null)
                raycaster = GetComponent<GraphicRaycaster>();

            Assert.IsTrue(FindObjectOfType<EventSystem>() != null, "No EventSystem found.");
            elements = GetComponentsInChildren<PUIElement>();
        }

        private void Awake()
        {
            if(instance == null)
            {
                instance = this;
            }
        }

        public void ApplyPreset(PUIPreset preset)
        {
            PUIPreset.ApplyPreset(this, preset);
        }
    }
    
}
