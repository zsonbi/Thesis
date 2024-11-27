using UnityEngine;

/// <summary>
/// Store the edge road of the chunk
/// </summary>
public class EdgeRoadContainer
{
    /// <summary>
    /// The position of the edge road
    /// </summary>
    public Vector2Int EdgeRoad { get; private set; }
    /// <summary>
    /// How big was the road counter then
    /// </summary>
    public int roadCounter { get; private set; }
    /// <summary>
    /// The direction the road was heading
    /// </summary>
    public Vector2Int Direction { get; private set; }

    /// <summary>
    /// Create a new edge road
    /// </summary>
    /// <param name="edgeRoad">The edge road's position</param>
    /// <param name="roadCounter">The counter</param>
    /// <param name="direction">The direction the road was heading</param>
    public EdgeRoadContainer(Vector2Int edgeRoad, int roadCounter, Vector2Int direction)
    {
        EdgeRoad = edgeRoad;
        this.roadCounter = roadCounter;
        this.Direction = direction;
    }
}