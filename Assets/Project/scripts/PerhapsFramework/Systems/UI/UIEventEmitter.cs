using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Perhaps
{
    public class UIEventEmitter : MonoBehaviour
    {
        public static event Action<string[]> OnSignalEmit;

        [SerializeField] UIButtonPerhaps btn;
        public string[] emmitedStrings;

        private void OnValidate()
        {
            if (btn == null)
                btn = GetComponent<UIButtonPerhaps>();
        }

        public void Start()
        {
            btn.OnStateChange += OnBtnStateChanged;
        }

        void OnBtnStateChanged(UIElementPerhaps btn)
        {
            if (btn.state.clicked)
            {
                Emit();
            }
        }

        public void Emit()
        {
            if (OnSignalEmit != null)
            {
                OnSignalEmit(emmitedStrings);
            }
        }
    }

}