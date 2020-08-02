using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Perhaps;

namespace Game
{
    public class EntityBase : MonoBehaviour
    {
        [Header("Entity Base")]
        [SerializeField] Collider[] Colliders;
        [SerializeField] Rigidbody2D rb2d;

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

        public virtual void Update()
        {
            
        }
    }
}
