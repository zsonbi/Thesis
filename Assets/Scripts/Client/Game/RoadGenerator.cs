using System.Collections.Generic;
using UnityEngine;
using DataTypes;

namespace Game
{
    namespace World
    {
        /// <summary>
        /// Generates a new road structure
        /// </summary>
        public class RoadGenerator
        {
            /// <summary>
            /// How many roads must the outside should have
            /// </summary>
            public int NumberOfEdgeRoads { get => EdgeRoads.Count; }

            /// <summary>
            /// The matrix which stores where the roads are
            /// </summary>
            public bool[,] RoadMatrix { get; private set; }

            /// <summary>
            /// Size of the matrix
            /// </summary>
            private int size;

            /// <summary>
            /// How many roads are currently on the edge
            /// </summary>
            public List<EdgeRoadContainer> EdgeRoads { get; private set; }

            /// <summary>
            /// The number of roads which was placed
            /// </summary>
            private int numberOfPlacedRoads = 0;

            /// <summary>
            /// Creates the road matrix
            /// </summary>
            /// <param name="numberOfEdgeRoads">How many roads should be on the matrix edges</param>
            /// <param name="size">How large is the chunk is to generate the road for</param>
            /// <param name="startPositions">The positions to start the generation from</param>
            public RoadGenerator(int size, List<EdgeRoadContainer> startPositions)
            {
                this.RoadMatrix = new bool[size, size];
                this.EdgeRoads = new List<EdgeRoadContainer>();
                this.size = size;
                if (startPositions.Count == 0)
                {
                    startPositions.Add(new EdgeRoadContainer(new Vector2Int(GameConfig.CHUNK_SIZE / 2, 0), 0, new Vector2Int(0, 1)));
                    this.RoadMatrix[0, GameConfig.CHUNK_SIZE / 2] = true;
                    EdgeRoads.Add(new EdgeRoadContainer(new Vector2Int(GameConfig.CHUNK_SIZE / 2, 0), 0, new Vector2Int(0, -1)));
                    numberOfPlacedRoads++;
                }

                foreach (var generateStartPos in startPositions)
                {
                    Vector2Int startPos = generateStartPos.EdgeRoad + generateStartPos.Direction;
                    Vector2Int newStart = new Vector2Int((startPos.x + size) % size, (startPos.y + size) % size);
                    RoadMatrix[newStart.y, newStart.x] = true;
                    numberOfPlacedRoads++;
                    EdgeRoads.Add(new EdgeRoadContainer(newStart, 0, generateStartPos.Direction));
                    CreateRoad(newStart + generateStartPos.Direction, generateStartPos.Direction, generateStartPos.roadCounter);
                }
            }

            /// <summary>
            /// Creates the roads recursively
            /// </summary>
            /// <param name="roadPos">The road's current position</param>
            /// <param name="roadDir">Where should the road's direction cuntinue to</param>
            /// <param name="forwardCounter">How long did the road go straight for</param>
            private void CreateRoad(Vector2Int roadPos, Vector2Int roadDir, int forwardCounter = 0)
            {
                if (RoadMatrix[roadPos.y, roadPos.x])
                {
                    //Debug.Log("Already set");
                    return;
                }
                this.RoadMatrix[roadPos.y, roadPos.x] = true;
                numberOfPlacedRoads++;
                if (roadPos.x == 0 || roadPos.x == size - 1 || roadPos.y == 0 || roadPos.y == size - 1)
                {
                    this.EdgeRoads.Add(new EdgeRoadContainer(roadPos, forwardCounter, roadDir));
                    return;
                }

                //Make sure that the chunk doesn't get flooded
                if (Random.Range(0.1f, 1f) < numberOfPlacedRoads / (size * size))
                {
                    return;
                }

                //Should the road change direction
                if (Mathf.Pow(forwardCounter - 7, 3) / 500 > Random.Range(0f, 1f) && !CheckForward(roadPos + roadDir, roadDir, 0))
                {
                    int interSectionCount = Random.Range(2, 3);
                    List<Vector2Int> possibleDirs = new List<Vector2Int>();

                    //The possible directions
                    possibleDirs.Add(new Vector2Int(roadPos.x - 1, roadPos.y));
                    possibleDirs.Add(new Vector2Int(roadPos.x + 1, roadPos.y));
                    possibleDirs.Add(new Vector2Int(roadPos.x, roadPos.y - 1));
                    possibleDirs.Add(new Vector2Int(roadPos.x, roadPos.y + 1));

                    for (int i = 0; i < interSectionCount && possibleDirs.Count != 0; i++)
                    {
                        int index = Random.Range(0, possibleDirs.Count);

                        if (!CheckForward(possibleDirs[index], possibleDirs[index] - roadPos, 0))
                        {
                            CreateRoad(possibleDirs[index], possibleDirs[index] - roadPos, 0);
                        }
                        possibleDirs.RemoveAt(index);
                    }
                }
                else
                {
                    //Make the road move forward
                    CreateRoad(roadPos + roadDir, roadDir, forwardCounter + 1);
                }
            }

            /// <summary>
            /// Checks if the road moving in that direction will run into an another road
            /// </summary>
            /// <param name="position">The position of the road which is being checked</param>
            /// <param name="dir">The direction where the checking should be</param>
            /// <param name="counter">How long the checking has processed</param>
            /// <returns>true-if it ran into a road, false if it didn't run into a road</returns>
            private bool CheckForward(Vector2Int position, Vector2Int dir, int counter)
            {
                if (position.x < 0 || position.y < 0 || position.y >= size || position.x >= size)
                    return false;

                if (RoadMatrix[position.y, position.x])
                {
                    return true;
                }
                else if (counter < 8)
                {
                    return CheckForward(position + dir, dir, counter + 1);
                }
                else
                {
                    return false;
                }
            }
        }
    }
}