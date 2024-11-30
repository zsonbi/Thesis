using DataTypes;
using UnityEngine;

namespace Assets.Scripts.Client.DataTypes
{
    /// <summary>
    /// Creates a single chunk cell container object
    /// </summary>
    public struct ChunkCellContainer
    {
        /// <summary>
        /// The type of the cell
        /// </summary>
        public ChunkCellType Type { get; private set; }
        /// <summary>
        /// The rotation
        /// </summary>
        public Vector3 Orientation { get; private set; }

        /// <summary>
        /// Creates a new instance
        /// </summary>
        /// <param name="type">The type of the cell</param>
        /// <param name="orientation">The orientation of the cell</param>
        public ChunkCellContainer(ChunkCellType type, Vector3 orientation)
        {
            this.Type = type;
            this.Orientation = orientation;
        }
    }
}