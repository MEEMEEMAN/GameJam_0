using Perhaps;
using Perhaps.Procedural;
using System;
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
        public Vector2Int roomDimensions;
        public int generationChainCount = 1;

        [Header("Room Gaps")]
        public Vector2 ceilingGapDimensions;
        public Vector2 ceilingGapOffset;
        public Vector2 wallGapDimensions;
        public Vector2 wallGapOffset;
        public bool randomOffset = false;

        [Header("Platforms")]
        public PerhapsPoissonParameters platformParameters;
        public Vector2 platformMinScale;
        public Vector2 platformMaxScale;
        public float downscale = 0.7f;

        [Header("Rendering")]
        public Transform observer;
        public int renderChainCount = 1;

        [Header("Textures")]
        public Sprite wallSprite;
        public Sprite platformSprite;

        Dictionary<int, LinkedListNode<Room>> existingRooms = new Dictionary<int, LinkedListNode<Room>>();

        public void Clear()
        {
            existingRooms.Clear();

            foreach (Room room in roomList)
            {
                foreach (Block block in room.roomBlocks)
                {
                    try
                    {
                        Destroy(block.collider.gameObject);
                    }
                    catch
                    {
                        
                    }
                }
            }

            roomList.Clear();
            existingBlocks.Clear();

            GC.Collect();
        }

        private void OnDisable()
        {
            Clear();
        }

        public int GetClosestRoomId(Vector2 position)
        {
            int x = (int)PerhapsMath.NearestJunction(position.x, roomDimensions.x);
            int y = (int)PerhapsMath.NearestJunction(position.y, roomDimensions.y);

            return PerhapsMath.SignedCantorPair(x, y);
        }

        Vector2Int IdToRoomCoords(int id)
        {
            PerhapsMath.SignedReverseCantorPair(id, out int x, out int y);
            return new Vector2Int(x, y);
        }

        int[] GetAdjacentPlus(int id, int verticalAdjacency, int horizontalAdjacency)
        {
            Vector2Int middleChunk = IdToRoomCoords(id);
            buffer.Clear();

            for (int y = -verticalAdjacency; y <= verticalAdjacency; y++)
            {
                Vector2Int chunkCoord = new Vector2Int(middleChunk.x,
                        middleChunk.y + (int)(y * roomDimensions.y));

                int genId = GetClosestRoomId(chunkCoord);
                buffer.Add(genId);
            }

            for (int x = -horizontalAdjacency; x <= horizontalAdjacency; x++)
            {
                Vector2Int chunkCoord = new Vector2Int(middleChunk.x + (int)(x * roomDimensions.x),
                                                                                    middleChunk.y);

                if (chunkCoord.x < 0 || chunkCoord.y < 0)
                    continue;

                int genId = GetClosestRoomId(chunkCoord);
                buffer.Add(genId);
            }


            int[] ids = buffer.ToArray();
            return ids;
        }

        static List<int> buffer = new List<int>();
        int[] GetAdjacentIds(int id, int verticalAdjacency, int horizontalAdjacency)
        {
            Vector2Int middleChunk = IdToRoomCoords(id);
            buffer.Clear();

            for (int y = -verticalAdjacency; y <= verticalAdjacency; y++)
            {
                for (int x = -horizontalAdjacency; x <= horizontalAdjacency; x++)
                {
                    Vector2Int chunkCoord = new Vector2Int(middleChunk.x + (int)(x * roomDimensions.x),
                                                            middleChunk.y + (int)(y * roomDimensions.y));

                    if (chunkCoord.x < 0 || chunkCoord.y < 0)
                        continue;

                    int genId = GetClosestRoomId(chunkCoord);
                    buffer.Add(genId);
                }
            }

            int[] ids = buffer.ToArray();
            return ids;
        }

        LinkedList<Room> roomList = new LinkedList<Room>();
        LinkedListNode<Room> Generate(int id)
        {
            if (existingRooms.ContainsKey(id))
            {
                return existingRooms[id];
            }

            Room p = new Room();
            p.bounds = new Bounds((Vector2)IdToRoomCoords(id), (Vector2)roomDimensions);
            p.id = id;

            var node = roomList.AddLast(p);
            existingRooms.Add(id, node);


            OnRoomGenerated(node);

            return node;
        }

        Room GenerationCheck(int id)
        {
            if (!existingRooms.ContainsKey(id) || existingRooms[id].Next == null)
            {
                int tempId = id;

                for (int i = 0; i < generationChainCount; i++)
                {
                    if (!existingRooms.ContainsKey(id) && roomList.First != null)
                    {
                        /*
                         * We are trying to generate a platform, from a platform coordinate that wasnt generated.
                         * This is okay if its the first platform we ever generate, which is the root platform,
                         * but all platforms following have to be connected.
                         */

                        return null;
                    }

                    Generate(tempId);
                    tempId = GetRandomAdjacentPlusMiddleExcluded(tempId);
                }
            }


            var node = existingRooms[id];
            return node.Value;
        }

        int GetRandomAdjacentPlusMiddleExcluded(int id)
        {
            int[] ids = GetAdjacentPlus(id, 1, 1).Where(x => x > id).ToArray();
            int ret = ids[UnityEngine.Random.Range(0, ids.Length)];

            return ret;
        }

        List<Room> rendered = new List<Room>();
        void Render(Room p)
        {
            if (p == null)
                return;


            var node = roomList.First;
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

            int id = GetClosestRoomId(observer.position);
            Room p = GenerationCheck(id);
            if (roomList.Last.Previous != null && roomList.Last.Previous.Previous != null && roomList.Last.Previous.Previous.Value == p)
            {
                GenerationCheck(roomList.Last.Value.id);
            }


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

        void OnRoomGenerated(LinkedListNode<Room> node)
        {
            Room room = node.Value;
            List<Block> blocks = Encase(room.bounds);
            node.Value.roomBlocks = blocks;

            if (node.List.Count > 1)
            {
                Bounds platformBounds = room.bounds;
                platformBounds.extents *= downscale;

                PerhapsUtils.DrawBounds(platformBounds, Color.magenta);
                PerhapsPoissonVolume volume = new PerhapsPoissonVolume(platformParameters, platformBounds);
                volume.Calculate();
                List<Vector2> points = volume.GetPoints();

                for (int i = 0; i < points.Count; i++)
                {
                    Vector2 scale = PerhapsUtils.RandomVec2(platformMinScale, platformMaxScale);
                    GameObject go = new GameObject();
                    go.transform.SetParent(blockParent);

                    go.tag = "Ground";
                    go.layer = blockLayer;
                    go.transform.position = points[i];
                    go.transform.localScale = scale;

                    SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
                    sr.sprite = platformSprite;

                    BoxCollider2D col = go.AddComponent<BoxCollider2D>();

                    Block b = new Block();
                    b.collider = col;
                    b.spriteRenderer = sr;


                    Vector3 pos = b.collider.transform.position;
                    int id = BlockPositionToId(new Vector2Int((int)pos.x, (int)pos.y));
                    existingBlocks.Add(id, b);

                    room.roomBlocks.Add(b);
                }


                if (node.Previous != null && node.Previous.Previous != null)
                {
                    CutRoomEntries(node);
                    CutRoomEntries(node.Previous);
                }

            }
        }

        void CutRoomEntries(LinkedListNode<Room> roomNode)
        {
            Room room = roomNode.Value;

            if (!room.hasBeenCut)
            {
                Room prev = roomNode.Previous.Value;
                Vector3 diff = room.bounds.center - prev.bounds.center;

                Vector2 half = prev.bounds.center + (diff / 2);
                Vector2 dir = diff.normalized;
                Vector2 perp = Vector2.Perpendicular(dir);

                float angle = Vector2.Angle(Vector2.down, dir);
                bool isCeiling = angle > 90f ? true : false;

                Vector2 offset = perp;
                Vector2 dimensions;
                if (isCeiling)
                {
                    offset *= ceilingGapOffset;
                    dimensions = ceilingGapDimensions;
                }
                else
                {
                    offset *= wallGapOffset;
                    dimensions = wallGapDimensions;
                }

                if (randomOffset)
                {
                    offset = PerhapsMath.RandomVec2(-offset, offset);
                }

                room.cutScale = dimensions;

                half += offset;
                room.cutPos = half;
                room.hasBeenCut = true;
            }

            List<Block> selected = Select(room.cutPos, room.cutScale);

#if UNITY_EDITOR
            List<Bounds> destroyedBounds = selected.Select(x => x.collider.bounds).ToList();

            PerhapsUtils.Execute(() =>
            {
                foreach (var bounds in destroyedBounds)
                {
                    PerhapsUtils.DrawBounds(bounds, Color.black, true);
                }

                return roomList.Count != 0;
            });
#endif

            for (int i = 0; i < selected.Count; i++)
            {
                room.roomBlocks.Remove(selected[i]);
            }

            for (int i = 0; i < selected.Count; i++)
            {
                Destroy(selected[i].collider.gameObject);
            }
        }

        Dictionary<int, Block> existingBlocks = new Dictionary<int, Block>();

        int[] PositionsToId(params Vector2Int[] pos)
        {
            int[] ids = new int[pos.Length];

            for (int i = 0; i < pos.Length; i++)
            {
                int id = BlockPositionToId(pos[i]);
                ids[i] = id;
            }

            return ids;
        }

        public Vector2Int IdToBlockPosition(int id)
        {
            PerhapsMath.ReverseCantorPair(id, out int x, out int y);
            return new Vector2Int(x, y);
        }

        public int BlockPositionToId(Vector2Int position)
        {
            return PerhapsMath.CantorPair(position.x, position.y);
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

                Vector2Int coord = new Vector2Int((int)pos.x, (int)pos.y);
                int id = BlockPositionToId(coord);
                if (existingBlocks.ContainsKey(id))
                    continue;

                Block b = new Block();
                existingBlocks.Add(id, b);

                GameObject blockGo = new GameObject();
                blockGo.tag = "Ground";

                blockGo.transform.SetParent(blockParent);
                blockGo.transform.position = pos;
                blockGo.layer = blockLayer;

#if UNITY_EDITOR
                blockGo.name = "Block ID: " + id;
#endif

                SpriteRenderer sr = blockGo.AddComponent<SpriteRenderer>();
                sr.sprite = wallSprite;
                BoxCollider2D col = blockGo.AddComponent<BoxCollider2D>();

                b.spriteRenderer = sr;
                b.collider = col;

                blocks.Add(b);
            }

            return blocks;
        }

        int selectLayerMask = -1;
        const int blockLayer = 9;
        const int bufferSize = 64;
        static Collider2D[] selectBuffer = new Collider2D[bufferSize];
        List<Block> Select(Vector3 origin, Vector3 scale)
        {
            if (selectLayerMask == -1)
            {
                selectLayerMask = LayerMask.GetMask("Block");
            }

            int count = Physics2D.OverlapBoxNonAlloc(origin, scale, 0f, selectBuffer, selectLayerMask);

#if UNITY_EDITOR
            PerhapsUtils.Execute(() =>
            {
                PerhapsUtils.DrawBounds(new Bounds(origin, scale), Color.red);
                return existingRooms.Count != 0;
            });
#endif

            Collider2D[] cols = new Collider2D[count];
            Array.Copy(selectBuffer, cols, count);

            List<Block> blocks = new List<Block>();
            for (int i = 0; i < cols.Length; i++)
            {
                Vector2 pos = cols[i].transform.position;
                Vector2Int coord = new Vector2Int((int)pos.x, (int)pos.y);

                int id = BlockPositionToId(coord);

                if (existingBlocks.ContainsKey(id))
                {
                    blocks.Add(existingBlocks[id]);
                }
            }

            return blocks;
        }

        #endregion
    }

}