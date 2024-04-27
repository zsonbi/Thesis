using DataTypes;
using UnityEngine;

public static class DirectionConverter
{
    public static Direction DirectionFromVector(Vector2Int input)
    {
        if (input.x == -1)
        {
            return Direction.Left;
        }
        else if (input.y == -1)
        {
            return Direction.Up;
        }
        else if (input.x == 1)
        {
            return Direction.Right;
        }
        else
        {
            return Direction.Down;
        }
    }

    public static Vector2Int VectorFromDirection(Direction direction)
    {
        switch (direction)
        {
            case Direction.Left:
                return new Vector2Int(-1, 0);

            case Direction.Up:
                return new Vector2Int(0, -1);

            case Direction.Right:
                return new Vector2Int(1, 0);

            case Direction.Down:
                return new Vector2Int(0, 1);

            default:
                return new Vector2Int(0, 0);
        }
    }
}