using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class aShuriken : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.gameObject.tag != "Ground")
        {
            transform.SetParent(collision.gameObject.transform, true);
            GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
        }
        GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
    }
    //private void OnCollisionExit2D(Collision2D collision)
    //{
        //Destroy(gameObject);
    //}
}
