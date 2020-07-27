using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Perhaps.Procedural;

namespace Perhaps
{
    [RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
    public class EditorMeshGenerator : MonoBehaviour
    {
        [Header("Mesh Generation")]
        [SerializeField] MeshRenderer meshRenderer;
        [SerializeField] MeshFilter meshFilter;

        [Header("Noise")]
        public bool randomSeed = true;
        public NoiseMutatorParameters noiseMutatorParameters;

        public bool StoreNoiseTexture = true;

        private void OnValidate()
        {
            if (meshRenderer == null)
            {
                meshRenderer = GetComponent<MeshRenderer>();
            }

            if (meshFilter == null)
            {
                meshFilter = GetComponent<MeshFilter>();
            }
        }

        public MeshGeneratorInput meshGeneratorParameters;

        public void GenerateMesh()
        {
            PerhapsMesh plane = MeshGenerator.GeneratePlane(meshGeneratorParameters);

            if(randomSeed)
            {
                noiseMutatorParameters.seed = Random.Range(0, int.MaxValue);
            }

            MeshNoiseMutator.MutateMesh(ref plane, noiseMutatorParameters);

            Mesh mesh = plane.ToUnityMesh();
            mesh.name = "Procedural Mesh";

            mesh.RecalculateNormals();
            mesh.RecalculateTangents();
            mesh.RecalculateBounds();
            meshFilter.mesh = mesh;

            NoiseTex = new Texture2D(plane.SquareSize, plane.SquareSize);
            NoiseTex.filterMode = FilterMode.Point;

            Color[] colors = new Color[plane.SquareSize * plane.SquareSize];
            var noiseFloats = MeshNoiseMutator.lastNoiseValues;
            for (int i = 0; i < plane.SquareSize; i++)
            {
                for (int j = 0; j < plane.SquareSize; j++)
                {
                    int index = i* plane.SquareSize + j;
                    float sample = noiseFloats[j, i];
                    colors[index] = new Color(sample, sample, sample, 1);
                }
            }

            NoiseTex.SetPixels(colors);
            NoiseTex.Apply();

            meshRenderer.sharedMaterial.SetTexture("_BaseMap", NoiseTex);
        }

        Texture2D NoiseTex;
        private void OnGUI() 
        {
            if(NoiseTex != null)
            {
                GUI.DrawTexture(new Rect(0, 0, 512, 512), NoiseTex);
            }
        }

    }

}