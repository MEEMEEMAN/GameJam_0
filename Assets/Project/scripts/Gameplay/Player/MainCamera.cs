﻿using Perhaps;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    [ExecuteInEditMode]
    public class MainCamera : MonoBehaviour
    {
        public static MainCamera instance { get; private set; }
        public Camera cam { get; private set; }
        [Header("Follow")]
        public Vector2 followOffset = new Vector2(0, 2);
        public float followLerpFactor = 10f;


        private void OnValidate()
        {
            if (cam == null)
                cam = GetComponent<Camera>();
        }

        private void Awake()
        {
            instance = this;
        }

        LivingEntity followed;
        public void SetFollowTarget(LivingEntity e)
        {
            followed = e;
        }

        private void LateUpdate()
        {
            if (followed == null)
                return;

            Vector3 center = followed.transform.position + (Vector3)followOffset;
            center.z = transform.position.z;
            transform.position = Vector3.Lerp(transform.position, center, Time.deltaTime * followLerpFactor);
        }

        public Vector3 WSCursorPosition
        {
            get
            {
                Ray ray = cam.ScreenPointToRay(PerhapsInput.MouseScreenPosition);
                return ray.origin;
            }
        }
    }

}