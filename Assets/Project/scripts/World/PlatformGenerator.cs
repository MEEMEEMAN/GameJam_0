using Perhaps;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game
{
    public class PlatformGenerator : MonoBehaviour
    {
        [Header("Generation Parameters")]
        public Transform blockParent;
        public float blockDistance = 0.64f;
        public Sprite wallSprite;
        public Vector2Int roomDimensions;
        public int generationChainCount = 1;

        [Header("Rendering")]
        public Transform observer;
        public int renderChainCount = 1;

        Dictionary<int, LinkedListNode<Platform>> existingPlatforms = new Dictionary<int, LinkedListNode<Platform>>();

        public int GetClosestPlatformID(Vector2 position)
        {
            int x = (int)PerhapsMath.NearestJunction(position.x, roomDimensions.x);
            int y = (int)PerhapsMath.NearestJunction(position.y, roomDimensions.y);

            return PerhapsMath.SignedCantorPair(x, y);
        }

        Vector2Int IdToCoords(int id)
        {
            PerhapsMath.SignedReverseCantorPair(id, out int x, out int y);
            return new Vector2Int(x, y);
        }

        int[] GetAdjacentPlus(int id, int verticalAdjacency, int horizontalAdjacency)
        {
            Vector2Int middleChunk = IdToCoords(id);
            buffer.Clear();

            for (int y = -verticalAdjacency; y <= verticalAdjacency; y++)
            {
                Vector2Int chunkCoord = new Vector2Int(middleChunk.x,
                        middleChunk.y + (int)(y * roomDimensions.y));

                int genId = GetClosestPlatformID(chunkCoord);
                buffer.Add(genId);
            }

            for (int x = -horizontalAdjacency; x <= horizontalAdjacency; x++)
            {
                Vector2Int chunkCoord = new Vector2Int(middleChunk.x + (int)(x * roomDimensions.x),
                                                                                    middleChunk.y);

                if (chunkCoord.x < 0 || chunkCoord.y < 0)
                    continue;

                int genId = GetClosestPlatformID(chunkCoord);
                buffer.Add(genId);
            }

            
            int[] ids = buffer.ToArray();
            return ids;
        }

        static List<int> buffer = new List<int>();
        int[] GetAdjacentIds(int id, int verticalAdjacency, int horizontalAdjacency)
        {
            Vector2Int middleChunk = IdToCoords(id);
            buffer.Clear();

            for (int y = -verticalAdjacency; y <= verticalAdjacency; y++)
            {
                for (int x = -horizontalAdjacency; x <= horizontalAdjacency; x++)
                {
                    Vector2Int chunkCoord = new Vector2Int(middleChunk.x + (int)(x * roomDimensions.x),
                                                            middleChunk.y + (int)(y * roomDimensions.y));

                    if (chunkCoord.x < 0 || chunkCoord.y < 0)
                        continue;

                    int genId = GetClosestPlatformID(chunkCoord);
                    buffer.Add(genId);
                }
            }

            int[] ids = buffer.ToArray();
            return ids;
        }


        LinkedList<Platform> platforms = new LinkedList<Platform>();
        LinkedListNode<Platform> Generate(int id)
        {
            if (existingPlatforms.ContainsKey(id))
                return existingPlatforms[id];

            Platform p = new Platform();
            p.bounds = new Bounds((Vector2)IdToCoords(id), (Vector2)roomDimensions);
            p.id = id;

            var node = platforms.AddLast(p);
            existingPlatforms.Add(id, node);

            Encase(p.bounds);

            return node;
        }

        Platform GenerationCheck(int id)
        {
            if (!existingPlatforms.ContainsKey(id) || existingPlatforms[id].Next == null)
            {
                int tempId = id;

                for (int i = 0; i < generationChainCount; i++)
                {
                    Generate(tempId);
                    tempId = GetRandomAdjacentExcluded(tempId);
                }
            }

            var node = existingPlatforms[id];
            return node.Value;
        }

        int GetRandomAdjacentExcluded(int id)
        {
            int[] ids = GetAdjacentPlus(id, 1, 1).Where(x => x > id).ToArray();

            int ret = ids[Random.Range(0, ids.Length)];

            return ret;
        }


        void Render(Platform p)
        {
            //var node = existingPlatforms[p.id];

            var node = platforms.First;
            while (node != null)
            {
                if (node.Previous != null)
                {
                    Debug.DrawLine(node.Previous.Value.bounds.center, node.Value.bounds.center, Color.green);
                }

                if (node.Value == p)
                {
                    node = node.Next;
                    continue;
                }
               

                PerhapsUtils.DrawBounds(node.Value.bounds, Color.blue);
                node = node.Next;
            }

            PerhapsUtils.DrawBounds(p.bounds, Color.red);
        }

        private void Update()
        {
            if (observer == null)
                return;

            int id = GetClosestPlatformID(observer.position);
            Platform p = GenerationCheck(id);
            Render(p);
        }


        #region Blocks

        /// <summary>
        /// Encases a Bounds with blocks.
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        List<Block> Encase(Bounds b)
        {
            List<Block> blocks = new List<Block>();
            blocks.AddRange(CreateLine(b.BottomLeft(), b.BottomRight()));
            blocks.AddRange(CreateLine(b.BottomLeft(), b.TopLeft()));
            blocks.AddRange(CreateLine(b.TopRight(), b.BottomRight()));
            blocks.AddRange(CreateLine(b.TopRight(), b.TopLeft()));

            return blocks;
        }

        /// <summary>
        /// Creates a line of blocks from start to end.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        List<Block> CreateLine(Vector3 start, Vector3 end)
        {
            Vector3 diff = end - start;
            float fillDistance = diff.magnitude;
            int count = Mathf.CeilToInt(fillDistance / blockDistance) + 1;

            List<Block> blocks = new List<Block>();
            for (int i = 0; i < count; i++)
            {
                Vector3 pos = Vector3.Lerp(start, end, i * blockDistance / fillDistance);
                pos.x = PerhapsMath.NearestJunction(pos.x, blockDistance);
                pos.y = PerhapsMath.NearestJunction(pos.y, blockDistance);

                Block b = new Block();

                GameObject blockGo = new GameObject();
                blockGo.transform.SetParent(blockParent);

                blockGo.transform.position = pos;
                blockGo.name = "Block";
                SpriteRenderer sr = blockGo.AddComponent<SpriteRenderer>();
                sr.sprite = wallSprite;
                BoxCollider2D col = blockGo.AddComponent<BoxCollider2D>();

                b.spriteRenderer = sr;
                b.collider = col;

                blocks.Add(b);
            }

            return blocks;
        }

        #endregion
    }

}