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
                transform.SetParent(collision.gameObject.transform, true);
                GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
            }
            GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
        }
    }

}