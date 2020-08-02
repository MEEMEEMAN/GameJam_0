using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Perhaps;

namespace Game
{
    public class EntityBase : MonoBehaviour
    {
        [Header("Entity Base")]
        [SerializeField] Collider2D[] Colliders;
        [SerializeField] Rigidbody2D rb2d;

        public Collider2D[] colliders => Colliders;
        public Rigidbody2D rb => rb2d;

        public virtual void OnValidate()
        {
            if (Colliders == null || Colliders.Length == 0)
            {
                Colliders = GetComponentsInChildren<Collider2D>();
            }

            if (rb2d == null)
                rb2d = GetComponent<Rigidbody2D>();
        }

        public virtual void Update()
        {
            
        }
    }
}
