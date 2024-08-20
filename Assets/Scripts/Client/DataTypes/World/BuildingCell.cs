using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataTypes;

namespace Game
{
    namespace World
    {
        internal class BuildingCell
        {
            public BuildingDirection RoadDirection { get; private set; }
            public bool Buildable { get; private set; }
            public bool GotRoadNext => RoadDirection != BuildingDirection.None;

            public BuildingCell(BuildingDirection roadDirection, bool buildable)
            {
                this.RoadDirection = roadDirection;
                Buildable = buildable;
            }

            public void Occupy()
            {
                this.Buildable = false;
            }
        }
    }
}