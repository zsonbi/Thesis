using DataTypes;
using UnityEngine;
using Utility;

namespace Game
{
    namespace World
    {
        /// <summary>
        /// Building handler
        /// </summary>
        public class Building : ThreadSafeMonoBehaviour
        {
            /// <summary>
            /// The building's type
            /// </summary>
            [SerializeField]
            private BuildingType buildingType;

            /// <summary>
            /// How many rows does the building need
            /// </summary>
            [SerializeField]
            private int rowCount;

            /// <summary>
            /// How many columns does the building need
            /// </summary>
            [SerializeField]
            private int columnCount;

            //Which direction does the enterance is
            [SerializeField]
            private Direction direction = Direction.Down;

            /// <summary>
            /// Addressable key
            /// </summary>
            [SerializeField]
            private string addressableKey;

            /// <summary>
            /// Building type getter
            /// </summary>
            public BuildingType BuildingType => buildingType;

            /// <summary>
            /// Row size getter
            /// </summary>
            public int RowCount => rowCount;

            /// <summary>
            /// Column size getter
            /// </summary>
            public int ColumnCount => columnCount;

            /// <summary>
            /// Enterance direction getter
            /// </summary>
            public Direction Direction => direction;

            /// <summary>
            /// Getter for the addressable key
            /// </summary>
            public string AddressableKey => addressableKey;

            /// <summary>
            /// Setter to the addressable key
            /// </summary>
            /// <param name="newKey">The new addressable key for addressing</param>
            public void SetAddressableKey(string newKey)
            {
                this.addressableKey = newKey;
            }
        }
    }
}