using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyB : MonoBehaviour
{
    public GameObject target;
    Vector3 t;
    public float speed;

    bool start;
    // Start is called before the first frame update
    void Start()
    {
        start = true;
    }
    private void Awake()
    {
        t = target.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (start)
        {
            fixedStart();
            start = false;
        }
        transform.position = Vector2.MoveTowards(transform.position, t, speed);
    }
    void fixedStart()
    {
        target = GameObject.FindGameObjectWithTag("Player");
        t = target.transform.position;
        StartCoroutine(b());
    }

    IEnumerator b()
    {
        yield return new WaitForSeconds(5f);
        Destroy(gameObject);
    }
}
