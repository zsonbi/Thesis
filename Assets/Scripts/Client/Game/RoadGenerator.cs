using System.Collections.Generic;
using UnityEngine;
using DataTypes;

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
    public RoadGenerator(int size, Vector2Int generateStartPos)
    {
        this.RoadMatrix = new bool[size, size];
        this.EdgeRoads = new List<EdgeRoadContainer>();
        this.size = size;
        Direction dir;

        if (generateStartPos.x == 0)
        {
            dir = Direction.Right;
        }
        else if (generateStartPos.x == size - 1)
        {
            dir = Direction.Left;
        }
        else if (generateStartPos.y == 0)
        {
            dir = Direction.Down;
        }
        else
        {
            dir = Direction.Up;
        }
        this.RoadMatrix[generateStartPos.y, generateStartPos.x] = true;

        CreateRoad(generateStartPos + DirectionConverter.VectorFromDirection(dir), dir);
    }

    private void CreateRoad(Vector2Int roadPos, Direction roadDir, int forwardCounter = 0)
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
            this.EdgeRoads.Add(new EdgeRoadContainer(roadPos, forwardCounter));
            return;
        }

        //One more exit condition
        if (Random.Range(0f, 1f) < numberOfPlacedRoads / (size * size))
        {
            //Debug.Log("Exit");
            return;
        }

        if (Mathf.Pow(forwardCounter - 30, 3) / 20000 > Random.Range(0.1f, 1f))
        {
            int interSectionCount = Random.Range(0, 3);
            List<Vector2Int> possibleDirs = new List<Vector2Int>();

            if (!RoadMatrix[roadPos.y, roadPos.x - 1])
                possibleDirs.Add(new Vector2Int(roadPos.x - 1, roadPos.y));
            if (!RoadMatrix[roadPos.y, roadPos.x + 1])
                possibleDirs.Add(new Vector2Int(roadPos.x + 1, roadPos.y));
            if (!RoadMatrix[roadPos.y - 1, roadPos.x])
                possibleDirs.Add(new Vector2Int(roadPos.x, roadPos.y - 1));
            if (!RoadMatrix[roadPos.y + 1, roadPos.x])
                possibleDirs.Add(new Vector2Int(roadPos.x, roadPos.y + 1));

            //Debug.Log(possibleDirs.Count);

            for (int i = 0; i < interSectionCount && possibleDirs.Count != 0; i++)
            {
                int index = Random.Range(0, possibleDirs.Count);

                CreateRoad(possibleDirs[index], DirectionConverter.DirectionFromVector(possibleDirs[index] - roadPos), 0);
                possibleDirs.RemoveAt(index);
            }
        }
        CreateRoad(roadPos + DirectionConverter.VectorFromDirection(roadDir), roadDir, forwardCounter + 1);
    }
}