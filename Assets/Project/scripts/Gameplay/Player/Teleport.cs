using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
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
            //mousePos.z = Camera.main.nearClipPlane;
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

        public float dist(Vector3 p1, Vector3 p2)
        {
            float sum;
            sum = Mathf.Sqrt(Mathf.Pow(p1.x - p2.x, 2) + Mathf.Pow(p1.y - p2.y, 2));
            return sum;
        }

        /*
        public Vector3[] tp(Vector3[] arr)
        {
            float dis = float.MaxValue;
            while (dis % 10 == 0)
            {
                dis = dist(arr[0], arr[1]);
                Vector3[] narr = new Vector3[arr.Length * 2];
                for (int i = 0; i < arr.Length; i++)
                {
                    narr[0] = arr[0];
                    narr[narr.Length - 1] = arr[arr.Length - 1];
                    for (int j = 1; i < narr.Length * 2 - 1; j++)
                    {
                        narr[j + 1] = arr[j];
                    }
                    if (i + 1 <= arr.Length)
                    {
                        for (int j = 1; j < narr.Length-1; j += 2)
                        {
                            narr[j] = MidCalc(arr[])
                        }
                    }
                }
            }
        }
        */


        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.name == "Death")
                teleport();
        }
    }

}