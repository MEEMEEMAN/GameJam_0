using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class GooglyEyes : MonoBehaviour
    {
        public Transform eye;
        public Vector3 centerLocalPosition;
        public float radius = 0.1f;

        private void OnValidate()
        {
            if (eye != null)
            {
                centerLocalPosition = eye.transform.localPosition;
            }
        }

        private void Start()
        {
            if (eye != null)
            {
                centerLocalPosition = eye.transform.localPosition;
            }

            LookAt(eye.transform.position);
        }

        public void LookAt(Vector3 worldSpacePosition)
        {
            Vector3 pos = CalcLocalEyePos(worldSpacePosition);
            eye.transform.localPosition = pos;
        }

        private void Update()
        {
            Vector3 wsPos = MainCamera.instance.WSCursorPosition;
            LookAt(wsPos);
        }

        Vector3 CalcLocalEyePos(Vector3 wsLookatPos)
        {
            Vector3 localLookAtPos = eye.parent.InverseTransformPoint(wsLookatPos);
            localLookAtPos.z = centerLocalPosition.z;

            Vector3 diff = localLookAtPos - centerLocalPosition;

            if(diff.magnitude <= radius)
            {
                return localLookAtPos;
            }


            diff.Normalize();
            diff *= radius;

            return centerLocalPosition + diff;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireSphere(eye.parent.TransformPoint(centerLocalPosition), radius);
        }
    }

}