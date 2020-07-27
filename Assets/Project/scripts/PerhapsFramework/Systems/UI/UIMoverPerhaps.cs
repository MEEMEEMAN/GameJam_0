using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Perhaps
{
    public class UIMoverPerhaps : UIElementPerhaps
    {
        [SerializeField] UIMoverTransition CurrentTransition;
        [SerializeField] float transitionTime = 0.2f;
        [SerializeField] MovementInfo info;
        [SerializeField] AnimationCurve moveCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
        public bool ignoreLayoutConfig;

        public UIMoverTransition currentTransition => CurrentTransition;

        public override void OnValidate()
        {
            bool isNull = rect == null;

            base.OnValidate();

            if(isNull)
            {
                WriteBegin();
                WriteEnd();
                CurrentTransition = UIMoverTransition.BEGIN;
            }
        }

        Coroutine currentRoutine = null;
        public void Transition(UIMoverTransition transition, bool instant = false, Action callback = null)
        {
            if (currentRoutine != null)
                StopCoroutine(currentRoutine);

            CurrentTransition = transition;
            currentRoutine = StartCoroutine(TransitionRoutine(transition, instant, callback));
        }

        public void TransitionOpposite(bool opposite = false, Action callback = null)
        {
            CurrentTransition = CurrentTransition == UIMoverTransition.BEGIN ? UIMoverTransition.END : UIMoverTransition.BEGIN;
            Transition(CurrentTransition,opposite, callback);
        }

        IEnumerator TransitionRoutine(UIMoverTransition transition, bool instant = false, Action callback = null)
        {
            Vector3 targetPos = transition == UIMoverTransition.BEGIN ? info.beginPos : info.endPos;
            targetPos.z = transform.position.z;

            Vector2 targetScale = transition == UIMoverTransition.BEGIN ? info.beginScale : info.endScale;
            float targetRot = transition == UIMoverTransition.BEGIN ? info.beginAngle : info.endAngle;

            if (!instant)
            {
                Vector3 startPos = rect.anchoredPosition;
                Vector2 startScale = transform.localScale;
                float startRot = transform.localEulerAngles.z;

                float beginTime = Time.time;
                float t = 0f;
                float scaledT = 0f;
                while (t < 1)
                {
                    float lerpTime = scaledT;

                    rect.anchoredPosition = Vector2.LerpUnclamped(startPos, targetPos, lerpTime);
                    transform.localScale = Vector2.LerpUnclamped(startScale, targetScale, lerpTime);
                    Vector3 angles = transform.localEulerAngles;
                    angles.z = Mathf.LerpUnclamped(startRot, targetRot, lerpTime);
                    transform.localEulerAngles = angles;

                    t = (Time.time - beginTime) / transitionTime;
                    scaledT = moveCurve.Evaluate(t);

                    yield return null;
                }
            }

            rect.anchoredPosition = targetPos;
            Vector3 angle = transform.localEulerAngles;
            angle.z = targetRot;
            transform.localEulerAngles = angle;
            transform.localScale = targetScale;

            if (callback != null)
                callback();
        }

        public void WriteBegin()
        {
            info.beginPos = rect.anchoredPosition;
            info.beginAngle = transform.eulerAngles.z;
            info.beginScale = transform.localScale;
        }

        public void WriteEnd()
        {
            info.endPos = rect.anchoredPosition;
            info.endAngle = transform.eulerAngles.z;
            info.endScale = transform.localScale;
        }
    }

    public enum UIMoverTransition
    {
        BEGIN, END
    }

    [System.Serializable]
    public struct MovementInfo
    {
        public Vector2 beginPos;
        public Vector2 endPos;
        public float beginAngle;
        public float endAngle;
        public Vector2 beginScale;
        public Vector2 endScale;
    }

}