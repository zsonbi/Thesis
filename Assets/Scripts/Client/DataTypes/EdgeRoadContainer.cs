using UnityEngine;

public class EdgeRoadContainer
{
    public Vector2Int EdgeRoad { get; private set; }
    public int roadCounter { get; private set; }

    public EdgeRoadContainer(Vector2Int edgeRoad, int roadCounter)
    {
        EdgeRoad = edgeRoad;
        this.roadCounter = roadCounter;
    }
}