using Perhaps;
using Perhaps.Procedural;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class WorldGenerator : MonoBehaviour
    {
        public Transform observerTransform;
        [Header("World Parameters")]
        public Vector2 chunkSize;

        [Header("Render Parameters")]
        public int horizontalChunkRenderRadius = 1;
        public int verticalChunkRenderRadius = 1;

        [Header("Platform Parameters")]
        public Transform platformParent;
        public Vector2 minDimensions;
        public Vector2 maxDimensions;
        public PerhapsPoissonParameters poissonParams;
        public Sprite platformSprite;


        Dictionary<int, WorldChunk> existingChunks = new Dictionary<int, WorldChunk>();

        private void OnEnable()
        {
            ClearWorld();
        }

        private void Start()
        {

        }

        private void Update()
        {
            if (observerTransform == null)
                return;

            int[] chunks = GetChunksToRender(observerTransform.position);
            Generate(chunks);
            Render(chunks);
        }

        public void ClearWorld()
        {
            existingChunks.Clear();
        }

        static List<int> chunkBuffer = new List<int>();
        int[] GetChunksToRender(Vector3 observerPosition)
        {
            Vector2Int middleChunk = GetChunkCoordinate(observerPosition);

            chunkBuffer.Clear();

            for (int y = -verticalChunkRenderRadius; y <= verticalChunkRenderRadius; y++)
            {
                for (int x = -horizontalChunkRenderRadius; x <= horizontalChunkRenderRadius; x++)
                {
                    Vector2Int chunkCoord = new Vector2Int(middleChunk.x + (int)(x * chunkSize.x),
                                                            middleChunk.y + (int)(y * chunkSize.y));

                    int id = ChunkToId(chunkCoord);
                    chunkBuffer.Add(id);
                }
            }

            int[] chunksToRender = chunkBuffer.ToArray();
            return chunksToRender;
        }

        void Generate(params int[] chunks)
        {
            for (int i = 0; i < chunks.Length; i++)
            {
                if (existingChunks.ContainsKey(chunks[i]))
                    continue;

                GenerateChunk(chunks[i]);
            }
        }

        void Render(params int[] chunkIds)
        {
            for (int i = 0; i < chunkIds.Length; i++)
            {
                WorldChunk chunk = existingChunks[chunkIds[i]];
                

#if UNITY_EDITOR
                bool crossLine = i == chunkIds.Length / 2;
                PerhapsUtils.DrawBounds(chunk.bounds, Color.blue, crossLine);
#endif
            }
        }


        public Vector2Int GetChunkCoordinate(Vector2 wsCoordinate)
        {
            int x = (int)PerhapsMath.NearestJunction(wsCoordinate.x, chunkSize.x);
            int y = (int)PerhapsMath.NearestJunction(wsCoordinate.y, chunkSize.y);

            return new Vector2Int(x, y);
        }

        public int ChunkToId(Vector2Int chunkPosition)
        {
            return PerhapsMath.SignedCantorPair(chunkPosition.x, chunkPosition.y);
        }

        public Vector2Int IdToChunk(int id)
        {
            PerhapsMath.SignedReverseCantorPair(id, out int x, out int y);
            return new Vector2Int(x, y);
        }



        void GenerateChunk(int id)
        {
            WorldChunk chunk = new WorldChunk();
            chunk.id = id;

            Vector2Int ChunkCoord = IdToChunk(id);
            chunk.bounds = new Bounds((Vector2)ChunkCoord, chunkSize);

            existingChunks.Add(id, chunk);
            PerhapsPoissonVolume poisson = new PerhapsPoissonVolume(poissonParams, chunk.bounds);
            poisson.Calculate();

            List<Vector2> points = poisson.GetPoints();

            for (int i = 0; i < points.Count; i++)
            {
                Vector2 position = points[i];
                Vector2 scale = PerhapsUtils.RandomVec2(minDimensions, maxDimensions);

                GameObject go = new GameObject();
                go.transform.SetParent(platformParent);
                go.transform.position = position;
                go.transform.localScale = scale;

                SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
                sr.sprite = platformSprite;
                go.AddComponent<BoxCollider2D>();


                chunk.platforms = new List<GameObject>();
                go.name = $"{id}: Platform {i}";
                chunk.platforms.Add(go);
            }


        }
    }

}