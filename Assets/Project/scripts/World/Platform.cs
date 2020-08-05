using Perhaps;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class Room
    {
        public int id;
        public Bounds bounds;
        public List<Block> roomBlocks;

        public bool hasBeenCut = false;
        public Vector3 cutPos;
        public Vector3 cutScale;
    }

}