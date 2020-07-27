using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Perhaps.Procedural.FN;

namespace Perhaps.Procedural
{
    [System.Serializable]
    public class MeshGeneratorInput
    {
        public Vector2 scale = Vector2.one;
        public int MeshSquareSize = 64;
        public int modulus = -1;
    }

    public static class MeshGenerator
    {
        public static PerhapsMesh GeneratePlane(MeshGeneratorInput input)
        {
            PerhapsMesh perhapsMesh = new PerhapsMesh(input.MeshSquareSize);

            Vector2 scale = input.scale;
            List<int> indices = new List<int>();
            for (int y = 0; y < perhapsMesh.SquareSize; y++)
            {
                if (input.modulus > 0 && y % input.modulus == 0)
                    continue;

                for (int x = 0; x < perhapsMesh.SquareSize; x++)
                {
                    if (input.modulus > 0 && x % input.modulus == 0)
                        continue;

                    int currentIndex = perhapsMesh.GetIndex(x, y);
                    Vector3 vertex = new Vector3(x * scale.x, 0, y * scale.y);
                    perhapsMesh.positions[currentIndex] = vertex;

                    float xUV = (float)x / (float)perhapsMesh.SquareSize;
                    float yUV = (float)y / (float)perhapsMesh.SquareSize;
                    Vector2 uv = new Vector2(xUV, yUV);
                    perhapsMesh.uvs[currentIndex] = uv;

                    if (y > 0 && x > 0) //completes a quad
                    {
                        int topRightIndex = perhapsMesh.GetIndex(x, y);
                        int bottomRightIndex = perhapsMesh.GetIndex(x, y - 1);
                        int topLeftIndex = perhapsMesh.GetIndex(x - 1, y);
                        int bottomLeftIndex = perhapsMesh.GetIndex(x - 1, y - 1);

                        indices.Add(topLeftIndex);
                        indices.Add(bottomRightIndex);
                        indices.Add(bottomLeftIndex);

                        indices.Add(topLeftIndex);
                        indices.Add(topRightIndex);
                        indices.Add(bottomRightIndex);
                    }
                }
            }

            if (perhapsMesh.indices != null && indices.Count != perhapsMesh.indices.Length)
            {
                string msg = string.Format("Anomaly, indicies List has {0} while perhapsMesh array has {1}", indices.Count, perhapsMesh.indices.Length);
                Debug.Log(msg);
            }

            perhapsMesh.indices = indices.ToArray();

            return perhapsMesh;
        }
    }

}