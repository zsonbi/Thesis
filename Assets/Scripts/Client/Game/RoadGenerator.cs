using System.Collections.Generic;
using UnityEngine;
using DataTypes;

namespace Game
{
    namespace World
    {
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

            private int numberOfPlacedRoads = 1;

            /// <summary>
            /// Creates the road matrix
            /// </summary>
            /// <param name="numberOfEdgeRoads">How many roads should be on the matrix edges</param>
            /// <param name="size"></param>
            /// <param name="generateStartPos"></param>
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
                }

                foreach (var generateStartPos in startPositions)
                {
                    Vector2Int startPos = generateStartPos.EdgeRoad + generateStartPos.Direction;
                    Vector2Int newStart = new Vector2Int((startPos.x + size) % size, (startPos.y + size) % size);
                    RoadMatrix[newStart.y, newStart.x] = true;
                    EdgeRoads.Add(new EdgeRoadContainer(newStart, 0, generateStartPos.Direction));

                    CreateRoad(newStart + generateStartPos.Direction, generateStartPos.Direction, generateStartPos.roadCounter);
                }
            }

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

                //One more exit condition
                if (Random.Range(0.1f, 1f) < numberOfPlacedRoads / (size * size))
                {
                    //Debug.Log("Exit");
                    return;
                }

                if (Mathf.Pow(forwardCounter - 7, 3) / 1000 > Random.Range(0f, 1f) && !CheckForward(roadPos + roadDir, roadDir, 0))
                {
                    int interSectionCount = Random.Range(2, 3);
                    List<Vector2Int> possibleDirs = new List<Vector2Int>();

                    possibleDirs.Add(new Vector2Int(roadPos.x - 1, roadPos.y));

                    possibleDirs.Add(new Vector2Int(roadPos.x + 1, roadPos.y));

                    possibleDirs.Add(new Vector2Int(roadPos.x, roadPos.y - 1));

                    possibleDirs.Add(new Vector2Int(roadPos.x, roadPos.y + 1));

                    //Debug.Log(possibleDirs.Count);

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
                    CreateRoad(roadPos + roadDir, roadDir, forwardCounter + 1);
                }
            }

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