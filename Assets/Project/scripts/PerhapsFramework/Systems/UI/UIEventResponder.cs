using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Perhaps
{
    public class UIEventResponder : UIElementPerhaps
    {
        public event Action onSignalReceived;

        [Header("Setup")]
        public string[] listenedSignals;

        [Header("Quick(optional)")]
        [SerializeField] UIMoverPerhaps mover;

        public void Start()
        {
            UIEventEmitter.OnSignalEmit += OnSignalEmit;
        }

        public override void OnValidate()
        {
            base.OnValidate();

            if (mover == null)
                mover = GetComponent<UIMoverPerhaps>();
        }

        private void OnSignalEmit(string[] signals)
        {
            CheckAndExecute(signals);
        }

        void CheckAndExecute(string[] arguments)
        {
            for (int i = 0; i < arguments.Length; i++)
            {
                if (ArrayContains(arguments[i], listenedSignals))
                {
                    RespondToSignal();
                }
            }
        }

        static bool ArrayContains(string value, string[] array)
        {
            for (int i = 0; i < array.Length; i++)
            {
                if (value == array[i])
                    return true;
            }
            return false;
        }

        void RespondToSignal()
        {
            if (onSignalReceived != null)
            {
                onSignalReceived();
            }

            if (mover != null)
            {
                /*
                TransitionState oppositeState = mover.GetOppositeState();
                MoverCommand.destinationState = oppositeState;

                if (!MoverCommand.instant)
                {
                    if (MoverCommand.time == 0)
                    {
                        MoverCommand.time = mover.GetDefaultTransitionTime();
                    }
                }

                mover.Transition(MoverCommand);
                */
                mover.TransitionOpposite();
            }
        }
    }
}