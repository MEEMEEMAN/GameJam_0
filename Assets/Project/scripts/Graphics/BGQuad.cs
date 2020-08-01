using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Perhaps
{
    public class BGQuad : MonoBehaviour
    {
        public MeshFilter filter;

        private void OnValidate()
        {
            if(filter != null)
            {
                Mesh m = new Mesh();
                m.vertices = new Vector3[]
                {
                new Vector3(-1, -1), //bottom left
                new Vector3(-1, 1),//top left
                new Vector3(1, -1),//bottom right
                new Vector3(1,1)//top right
                };

                m.uv = new Vector2[]
                {
                new Vector2(0,0),
                new Vector2(0, 1),
                new Vector2(1, 0),
                new Vector2(1,1)
                };

                m.triangles = new int[]
                {
                0,1,2,
                2,1,3
                };

                filter.sharedMesh = m;
            }
        }

        private void LateUpdate()
        {
            if(MainCamera.instance != null)
            {
                Vector3 pos = MainCamera.instance.transform.position;
                pos.z = transform.position.z;

                transform.position = pos;
            }
        }
    }

}