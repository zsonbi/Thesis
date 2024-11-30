
using DataTypes;

namespace Game
{
    namespace World
    {
        /// <summary>
        /// The building cell on the map
        /// </summary>
        internal class BuildingCell
        {
            /// <summary>
            /// Which direction is the road
            /// </summary>
            public BuildingDirection RoadDirection { get; private set; }
            /// <summary>
            /// Is this cell still buildable
            /// </summary>
            public bool Buildable { get; private set; }
            /// <summary>
            /// Does it have road next to it
            /// </summary>
            public bool GotRoadNext => RoadDirection != BuildingDirection.None;

            /// <summary>
            /// Create a new cell
            /// </summary>
            /// <param name="roadDirection">The direction of the road</param>
            /// <param name="buildable">Is it buildable</param>
            public BuildingCell(BuildingDirection roadDirection, bool buildable)
            {
                this.RoadDirection = roadDirection;
                Buildable = buildable;
            }

            /// <summary>
            /// Mark it unbuildable
            /// </summary>
            public void Occupy()
            {
                this.Buildable = false;
            }
        }
    }
}