using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Perhaps.Procedural.FN;

namespace Perhaps.Procedural
{
    [System.Serializable]
    public class NoiseMutatorParameters
    {
        [Range(0, 1)]
        public float lacunarity;

        [Range(-1, 1)]
        public float gain;
        
        [Range(0, 6)]
        public int octaves;
        public int seed;
        public float noiseStrength  = 1f;

        [Range(0, 1)]
        public float frequency = 0.5f;
        public AnimationCurve noiseCurve;

        public FastNoise.NoiseType noiseType;
        public FastNoise.FractalType fractalType;
    }

    public static class MeshNoiseMutator
    {
        public static float[,] lastNoiseValues;

        public static void MutateMesh(ref PerhapsMesh mutatedMesh, NoiseMutatorParameters noiseParameters)
        {
            if(mutatedMesh == null || noiseParameters == null)
            {
                throw new System.Exception("a parameter is null!");
            }
            
            FastNoise fastNoise = new FastNoise(noiseParameters.seed);
            fastNoise.SetFractalOctaves(noiseParameters.octaves);
            fastNoise.SetFractalLacunarity(noiseParameters.lacunarity);
            fastNoise.SetFractalGain(noiseParameters.gain);
            fastNoise.SetNoiseType(noiseParameters.noiseType);
            fastNoise.SetFractalType(noiseParameters.fractalType);
            fastNoise.SetFrequency(noiseParameters.frequency);

            int sqrSize = mutatedMesh.SquareSize;
            lastNoiseValues = new float[sqrSize, sqrSize];
            
            for (int y = 0; y < sqrSize; y++)
            {
                for (int x = 0; x < sqrSize; x++)
                {
                    float noise = fastNoise.GetNoise(x, y); // -1 to 1
                    float sampleRange = (noise + 1f) * 0.5f; // 0 to 1
                    lastNoiseValues[x,y] = sampleRange;

                    int currentIndex = mutatedMesh.GetIndex(x, y);
                    float curveSample = noiseParameters.noiseCurve.Evaluate(sampleRange);
                    mutatedMesh.positions[currentIndex].y += noise * curveSample * noiseParameters.noiseStrength;
                }
            }
        }
    }

}