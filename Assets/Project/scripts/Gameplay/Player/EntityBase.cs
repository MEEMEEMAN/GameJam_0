using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Perhaps
{
    public class EntityBase : MonoBehaviour
    {
        [Header("Entity Base")]
        [SerializeField] Collider[] Colliders;
        [SerializeField] Rigidbody2D rb2d;
        public float groundRayLength = 1f;
        public Vector2 groundCastSize = new Vector2(1, 1);

        public Collider[] colliders => Colliders;
        public Rigidbody2D rb => rb2d;

        public virtual void OnValidate()
        {
            if (Colliders == null || Colliders.Length == 0)
            {
                Colliders = GetComponentsInChildren<Collider>();
            }

            if (rb2d == null)
                rb2d = GetComponent<Rigidbody2D>();
        }

        public const int hitBufferSize = 5;
        public RaycastHit2D[] groundBuffer = new RaycastHit2D[hitBufferSize];
        public bool IsGrounded()
        {
            int layer = gameObject.layer;
            //ignore raycast
            gameObject.SetLayerRecursive(2);

            int hitCount = Physics2D.BoxCastNonAlloc(transform.position, groundCastSize, 
                0f, Vector2.down, groundBuffer, groundRayLength);

            Debug.DrawRay(transform.position, Vector2.down * groundRayLength, Color.red);

            //restore
            gameObject.SetLayerRecursive(layer);

            return hitCount > 0;

        }
    }
}
