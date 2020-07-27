    using UnityEngine;
    using System.Collections.Generic;
    
    /// <summary>
    /// A Custom mesh class
    /// </summary>
    public class PerhapsMesh
    {
        public PerhapsMesh(int squareSize)
        {
            SquareSize = squareSize;
            int squareCount = squareSize * squareSize;
            positions = new Vector3[squareCount];
            uvs = new Vector2[squareCount];
        }

        public int SquareSize {get; private set;} = 0;
        public Vector3[] positions;
        public Vector2[] uvs;
        public int[] indices;

        public int GetSupposedIndexCount()
        {
            return (SquareSize -1) * (SquareSize -1) * 6;
        }

        public int GetIndex(int x, int y)
        {
            return y * SquareSize + x;
        }

        public Vector2Int GetXY(int index)
        {
            return new Vector2Int(index / SquareSize, index % SquareSize);
        }

        /// <summary>
        /// Returns a unity mesh, with no normals, no tangents and no bounds.
        /// </summary>
        /// <returns></returns>
        public Mesh ToUnityMesh()
        {
            Mesh mesh = new Mesh();
            mesh.vertices = positions;
            mesh.uv  = uvs;
            mesh.triangles = indices;
            
            return mesh;
        }
    }