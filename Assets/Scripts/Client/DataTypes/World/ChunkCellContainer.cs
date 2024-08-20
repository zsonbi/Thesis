using UnityEngine;
namespace Assets.Scripts.Client.DataTypes
{
    public struct ChunkCellContainer
    {
        public ChunkCellType Type{get; private set; }
        public Vector3 Orientation{get; private set; }

        public ChunkCellContainer(ChunkCellType type, Vector3 orientation)
        {
            this.Type = type;
            this.Orientation = orientation;
        }
    }
}
