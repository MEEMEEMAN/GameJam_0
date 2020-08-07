using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Perhaps
{
    public class BandanaRope : MonoBehaviour
    {
        [SerializeField] Rigidbody2D rb;
        [SerializeField] LineRenderer lr;
        [SerializeField] Vector3 offset;

        private void OnValidate()
        {
            if (rb == null)
                rb = GetComponent<Rigidbody2D>();

            if (lr == null)
                lr = GetComponent<LineRenderer>();
        }

        private void Start()
        {
            lr.positionCount = transform.childCount;
        }

        private void Update()
        {
            rb.position = transform.parent.position + offset;

            for (int i = 0; i < transform.childCount; i++)
            {
                Transform child = transform.GetChild(i);
                lr.SetPosition(i, child.transform.position);
            }
        }
    }

}