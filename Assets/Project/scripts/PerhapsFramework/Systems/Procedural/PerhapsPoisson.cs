using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Perhaps.Procedural
{
    [System.Serializable]
    public class PerhapsPoissonParameters
    {
        public float radius = 1f;
        public Vector2 sampleRegionSize = new Vector2(10, 10);
        public int maxSamplesBeforeRejection = 30;
        public int seed;
    }

    public class PerhapsPoissonVolume
    {
        public static List<Vector2> GeneratePoints(IEnumerable<Vector2> existingPoints, Bounds bounds, float pointRadius = 1f, int maxSamples = 30)
        {
            PerhapsPoissonParameters parameters = new PerhapsPoissonParameters();
            parameters.radius = pointRadius;
            parameters.maxSamplesBeforeRejection = maxSamples;

            PerhapsPoissonVolume volume = new PerhapsPoissonVolume(parameters, bounds);
            volume.generatedPoints = existingPoints.ToList();
            volume.Calculate();
            return volume.GetPoints();
        }

        public static List<Vector2> GeneratePoints(Bounds bounds, float pointRadius = 1f, int maxSamples = 30)
        {
            PerhapsPoissonParameters parameters = new PerhapsPoissonParameters();
            parameters.radius = pointRadius;
            parameters.maxSamplesBeforeRejection = maxSamples;

            PerhapsPoissonVolume volume = new PerhapsPoissonVolume(parameters, bounds);
            volume.Calculate();
            return volume.GetPoints();
        }

        public static List<Vector2> GeneratePointsInRadius(IEnumerable<Vector2> existingPoints, Vector2 center, float generateRadius = 1f, float pointRadius = 1f, int maxSamples = 30)
        {
            Bounds b = new Bounds(center, (Vector2.right * generateRadius + Vector2.up * generateRadius) * 2);
            var points = GeneratePoints(existingPoints, b, pointRadius, maxSamples);

            float sqrGenRadius = generateRadius * generateRadius;
            for (int i = 0; i < points.Count; i++)
            {
                Vector2 diff = points[i] - center;
                if (diff.sqrMagnitude > sqrGenRadius)
                {
                    points.RemoveAt(i);
                    i--;
                }
            }

            return points;
        }

        public static List<Vector2> GeneratePointsInRadius(Vector2 center, float generateRadius = 1f, float pointRadius = 1f, int maxSamples = 30)
        {
            Bounds b = new Bounds(center, (Vector2.right * generateRadius + Vector2.up * generateRadius) * 2);
            var points = GeneratePoints(b, pointRadius, maxSamples);

            float sqrGenRadius = generateRadius * generateRadius;
            for (int i = 0; i < points.Count; i++)
            {
                Vector2 diff = points[i] - center;
                if(diff.sqrMagnitude > sqrGenRadius)
                {
                    points.RemoveAt(i);
                    i--;
                }
            }

            return points;
        }


        float cellSize, radius;
        Vector2 sampleRegionSize;
        int maxSamplesBeforeRejection;
        List<Vector2> candidatePoints;
        List<Vector2> generatedPoints;
        int[,] grid;
        int seed;


        public bool hasGenBounds {get; private set;}= false;
        public Bounds genBounds {get; private set;}
        public PerhapsPoissonVolume(PerhapsPoissonParameters parameters, Bounds genBounds)
        {
            Initialize(parameters);
            
            sampleRegionSize = new Vector2(genBounds.size.x, genBounds.size.y);
            this.genBounds = genBounds;
            hasGenBounds = true;
        }

        public PerhapsPoissonVolume(PerhapsPoissonParameters parameters)
        {
            Initialize(parameters);
        }

        void Initialize(PerhapsPoissonParameters parameters)
        {
            radius = parameters.radius;
            sampleRegionSize = parameters.sampleRegionSize;
            maxSamplesBeforeRejection = parameters.maxSamplesBeforeRejection;
            cellSize = parameters.radius / Mathf.Sqrt(2f);
        }

        public void Calculate()
        {
            //Random.InitState(seed);
            float cellSize = radius / Mathf.Sqrt(2);

            int gridSizeX = Mathf.CeilToInt(sampleRegionSize.x / cellSize);
            int gridSizeY = Mathf.CeilToInt(sampleRegionSize.y / cellSize);
            grid = new int[gridSizeX, gridSizeY];

            generatedPoints = new List<Vector2>();
            candidatePoints = new List<Vector2>();

            candidatePoints.Add(sampleRegionSize / 2);
            while (candidatePoints.Count > 0)
            {
                int spawnIndex = Random.Range(0, candidatePoints.Count);
                Vector2 spawnCentre = candidatePoints[spawnIndex];
                bool candidateAccepted = false;

                for (int i = 0; i < maxSamplesBeforeRejection; i++)
                {
                    float angle = Random.value * Mathf.PI * 2;
                    Vector2 dir = new Vector2(Mathf.Sin(angle), Mathf.Cos(angle));
                    Vector2 candidate = spawnCentre + dir * Random.Range(radius, 2 * radius);

                    if (CandidateIsValid(candidate))
                    {
                        generatedPoints.Add(candidate);
                        candidatePoints.Add(candidate);

                        grid[(int)(candidate.x / cellSize), (int)(candidate.y / cellSize)] = generatedPoints.Count;
                        candidateAccepted = true;
                        break;
                    }
                }
                if (!candidateAccepted)
                {
                    candidatePoints.RemoveAt(spawnIndex);
                }

            }

            if(hasGenBounds)
            {
                //all points get generated with Vector3(0,0,0) as their origin point.
                //we simply offset a point by the bound's extents, and shift them the bound's origin
                for (int i = 0; i < generatedPoints.Count; i++)
                {
                    generatedPoints[i] -= new Vector2(genBounds.extents.x, genBounds.extents.y);
                    generatedPoints[i] += new Vector2(genBounds.center.x, genBounds.center.y);
                }
            }

        }

        bool CandidateIsValid(Vector2 candidate)
        {
            if (candidate.x >= 0 && candidate.x < sampleRegionSize.x && candidate.y >= 0 && candidate.y < sampleRegionSize.y)
            {
                int cellX = (int)(candidate.x / cellSize);
                int cellY = (int)(candidate.y / cellSize);
                int searchStartX = Mathf.Max(0, cellX - 2);
                int searchEndX = Mathf.Min(cellX + 2, grid.GetLength(0) - 1);
                int searchStartY = Mathf.Max(0, cellY - 2);
                int searchEndY = Mathf.Min(cellY + 2, grid.GetLength(1) - 1);

                for (int x = searchStartX; x <= searchEndX; x++)
                {
                    for (int y = searchStartY; y <= searchEndY; y++)
                    {
                        int pointIndex = grid[x, y] - 1;
                        if (pointIndex != -1)
                        {
                            float sqrDst = (candidate - generatedPoints[pointIndex]).sqrMagnitude;
                            if (sqrDst < radius * radius)
                            {
                                return false;
                            }
                        }
                    }
                }
                return true;
            }

            return false;
        }

        public List<Vector2> GetPoints()
        {
            return generatedPoints;
        }
    }

}