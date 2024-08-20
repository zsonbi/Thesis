using System.Collections.Generic;
using DataTypes;
using UnityEngine;

namespace Game
{
    namespace World
    {
        internal class BuildingGenerator : MonoBehaviour
        {
            private BuildingCell[,] buildingCells;

            public void GenerateBuildings(List<Vector3Int> roads)
            {
                GenerateCellMatrix(roads);
            }

            private void GenerateCellMatrix(List<Vector3Int> roads)
            {
                foreach (var road in roads)
                {
                    for (var i = road.z - 1; i < road.z + 1; i++)
                    {
                        for (int j = road.x - 1; j < road.x; j++)
                        {
                            if (i < 0 || j < 0 || i >= GameConfig.CHUNK_SIZE || j >= GameConfig.CHUNK_SIZE || buildingCells[i, j] != null)
                            {
                                continue;
                            }
                            if (i == 0 && j == 0)
                            {
                                buildingCells[i, j] = new BuildingCell(Direction.None, false);
                            }
                            else
                            {
                                Direction roadDir = Direction.None;
                                if (i > road.z)
                                {
                                    roadDir = Direction.Down;
                                }
                                else if (i < road.z)
                                {
                                    roadDir = Direction.Up;
                                }
                                else
                                {
                                    if (j > road.x)
                                    {
                                        roadDir = Direction.Right;
                                    }
                                    else if (j < road.x)
                                    {
                                        roadDir = Direction.Left;
                                    }
                                }
                                buildingCells[i, j] = new BuildingCell(roadDir, true);
                            }
                        }
                    }
                }
            }
        }
    }
}