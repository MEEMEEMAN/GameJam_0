using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class aShuriken : MonoBehaviour
    {
        private void OnCollisionEnter2D(Collision2D collision)
        {

            if (collision.gameObject.tag != "Ground")
            {
                Rigidbody2D rb = GetComponent<Rigidbody2D>();

                Vector3 pos = transform.position;
                transform.SetParent(collision.transform, false);
                transform.position = pos;
                rb.bodyType = RigidbodyType2D.Kinematic;
            }
        }
    }

}