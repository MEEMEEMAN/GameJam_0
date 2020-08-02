using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    public Vector2 force;
    private void Start()
    {
        GetComponent<Rigidbody2D>().AddForce(force, ForceMode2D.Impulse);
        StartCoroutine(b());
    }
    IEnumerator b()
    {
        yield return new WaitForSeconds(5);
        Destroy(gameObject);

    }
}
