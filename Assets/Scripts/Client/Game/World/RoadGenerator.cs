using System.Collections.Generic;
using UnityEngine;

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
            /// The nearby chunks for checking the roads forward
            /// </summary>
            private Chunk[,] nearbyChunks;

            /// <summary>
            /// Load the road and nearby cells, so we don't have to roads parallel next to each other
            /// </summary>
            private bool[,] lockRoad;

            /// <summary>
            /// Creates the road matrix
            /// </summary>
            /// <param name="numberOfEdgeRoads">How many roads should be on the matrix edges</param>
            /// <param name="size">How large is the chunk is to generate the road for</param>
            /// <param name="startPositions">The positions to start the generation from</param>
            public RoadGenerator(int size, List<EdgeRoadContainer> startPositions, Chunk[,] nearbyChunks)
            {
                this.nearbyChunks = nearbyChunks;
                this.RoadMatrix = new bool[size, size];
                this.EdgeRoads = new List<EdgeRoadContainer>();
                this.size = size;
                this.lockRoad = new bool[size, size];
                if (startPositions.Count == 0)
                {
                    startPositions.Add(new EdgeRoadContainer(new Vector2Int(GameConfig.CHUNK_SIZE / 2, 0), 0, new Vector2Int(0, 1)));
                    this.RoadMatrix[0, GameConfig.CHUNK_SIZE / 2] = true;
                    EdgeRoads.Add(new EdgeRoadContainer(new Vector2Int(GameConfig.CHUNK_SIZE / 2, 0), 1, new Vector2Int(0, -1)));
                    numberOfPlacedRoads++;
                }

                foreach (var generateStartPos in startPositions)
                {
                    Vector2Int startPos = generateStartPos.EdgeRoad + generateStartPos.Direction;
                    Vector2Int newStart = new Vector2Int((startPos.x + size) % size, (startPos.y + size) % size);
                    RoadMatrix[newStart.y, newStart.x] = true;
                    numberOfPlacedRoads++;
                    EdgeRoads.Add(new EdgeRoadContainer(newStart, 1, generateStartPos.Direction));
                    CreateRoad(new Vector2Int(newStart.x + generateStartPos.Direction.x, newStart.y + generateStartPos.Direction.y), generateStartPos.Direction, 0);
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
                if (false && Random.Range(0.8f, 1f) < numberOfPlacedRoads / (size * size))
                {
                    return;
                }
                // || Mathf.Pow(forwardCounter - 1, 5) / 2 > Random.Range(0f, 1f)
                //Should the road change direction
                if ((forwardCounter > 2 && !lockRoad[roadPos.y, roadPos.x]) && !CheckForward(roadPos + roadDir, roadDir, 0) && !GoinTowardsEdge(roadPos, roadDir))
                {
                    int interSectionCount = Random.Range(3, 4);
                    List<Vector2Int> possibleDirs = new List<Vector2Int>();

                    //The possible directions
                    possibleDirs.Add(new Vector2Int(roadPos.x - 1, roadPos.y));
                    possibleDirs.Add(new Vector2Int(roadPos.x + 1, roadPos.y));
                    possibleDirs.Add(new Vector2Int(roadPos.x, roadPos.y - 1));
                    possibleDirs.Add(new Vector2Int(roadPos.x, roadPos.y + 1));

                    for (int i = 0; i < interSectionCount && possibleDirs.Count != 0; i++)
                    {
                        int index = Random.Range(0, possibleDirs.Count);

                        if (!CheckForward(possibleDirs[index], possibleDirs[index] - roadPos, 0) && !GoinTowardsEdge(roadPos, roadDir))
                        {
                            CreateRoad(possibleDirs[index], possibleDirs[index] - roadPos, 0);
                        }
                        possibleDirs.RemoveAt(index);
                    }
                }
                else
                {
                    lockRoad[roadPos.y, roadPos.x] = true;

                    if (roadDir.x == 0)
                    {
                        if (roadDir.x - 1 >= 0)
                        {
                            lockRoad[roadPos.y, roadPos.x - 1] = true;
                        }

                        if (roadDir.x + 1 < GameConfig.CHUNK_SIZE)
                        {
                            lockRoad[roadPos.y, roadPos.x + 1] = true;
                        }
                    }
                    else
                    {
                        if (roadDir.y - 1 >= 0)
                        {
                            lockRoad[roadPos.y - 1, roadPos.x] = true;
                        }
                        if (roadDir.y + 1 < GameConfig.CHUNK_SIZE)
                        {
                            lockRoad[roadPos.y + 1, roadPos.x] = true;
                        }
                    }
                    //Make the road move forward
                    CreateRoad(roadPos + roadDir, roadDir, forwardCounter + 1);
                }
            }

            /// <summary>
            /// Check if the road is going towards the edge
            /// </summary>
            /// <param name="position">The staring position</param>
            /// <param name="dir">Move direction</param>
            /// <returns>true-going towards the edge, false-still inside</returns>
            private bool GoinTowardsEdge(Vector2Int position, Vector2Int dir)
            {
                int newXCoord = position.x + dir.x * 5;
                int newYCoord = position.y + dir.y * 5;

                return newXCoord < 0 || newYCoord < 0 || newXCoord >= GameConfig.CHUNK_SIZE || newYCoord >= GameConfig.CHUNK_SIZE;
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
                if (position.x < 0)
                {
                    if (nearbyChunks[1, 0] is null)
                    {
                        return false;
                    }
                    if (nearbyChunks[1, 0].Roads[position.y, position.x + GameConfig.CHUNK_SIZE])
                    {
                        return true;
                    }
                }
                else if (position.y < 0)
                {
                    if (nearbyChunks[0, 1] is null)
                    {
                        return false;
                    }
                    if (nearbyChunks[0, 1].Roads[position.y + GameConfig.CHUNK_SIZE, position.x])
                    {
                        return true;
                    }
                }
                else if (position.x >= size)
                {
                    if (nearbyChunks[1, 2] is null)
                    {
                        return false;
                    }
                    if (nearbyChunks[1, 2].Roads[position.y, position.x - GameConfig.CHUNK_SIZE])
                    {
                        return true;
                    }
                }
                else if (position.y >= size)
                {
                    if (nearbyChunks[2, 1] is null)
                    {
                        return false;
                    }
                    if (nearbyChunks[2, 1].Roads[position.y - GameConfig.CHUNK_SIZE, position.x])
                    {
                        return true;
                    }
                }
                else if (RoadMatrix[position.y, position.x])
                {
                    return true;
                }
                if (counter < 5)
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