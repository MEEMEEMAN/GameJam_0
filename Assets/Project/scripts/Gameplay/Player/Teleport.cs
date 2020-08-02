using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport : MonoBehaviour
{
    public Vector3 loc;
    public int tpTokens;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
            loc = transform.position;
        if (Input.GetKeyDown(KeyCode.Y))
            teleport();
    }

    public void teleport()
    {
        if (tpTokens != 0)
        {
            var mid = MidCalc(transform.position, loc);
            transform.position = mid;
            StartCoroutine(a());
            
        }
        tpTokens--; 
    }


    IEnumerator a()
    {
        var mid = MidCalc(transform.position, loc);
        var midf = MidCalc(transform.position, mid);
        var mida = MidCalc(mid, loc);
        transform.position = MidCalc(transform.position, midf);
        yield return new WaitForSeconds(0.005f);
        transform.position = midf;
        yield return new WaitForSeconds(0.005f);
        transform.position = MidCalc(midf, mid);
        yield return new WaitForSeconds(0.005f);
        transform.position = mid;
        yield return new WaitForSeconds(0.005f);
        transform.position = MidCalc(mid, mida);
        yield return new WaitForSeconds(0.005f);
        transform.position = mida;
        yield return new WaitForSeconds(0.005f);
        transform.position = MidCalc(mida, loc);
        yield return new WaitForSeconds(0.005f);
        transform.position = loc;
    }

    public Vector3 MidCalc(Vector3 from, Vector3 to)
    {
        Vector3 mid;
        mid.x = (from.x + to.x) / 2;
        mid.y = (from.y + to.y) / 2;
        mid.z = 0;
        return mid;
    }
}
