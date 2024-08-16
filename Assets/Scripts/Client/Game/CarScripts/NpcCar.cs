using Game.World;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CarController))]
public class NpcCar : MonoBehaviour
{
    protected World world;
    protected CarController carController;
    private enum Direction
    {
        Up, Right, Down, Left
    }


    public void Init(World world)
    {
        this.world = world;
    }

    private void Awake()
    {
        this.carController = this.gameObject.GetComponent<CarController>();
    }

    public float DetermineSteeringDirection()
    {
     

        Direction facingDir;
        Vector3 rotation = this.gameObject.transform.rotation.eulerAngles;
        if (rotation.y > 0)
        {
            if ((rotation.y <= 90))
            {
                facingDir = Direction.Up;
            }
            else
            {
                facingDir=Direction.Right;
            }
        }
        else
        {
            if(rotation.y > -90)
            {
                facingDir = Direction.Left;
            }
            else
            {
                facingDir = Direction.Down;
            }
        }


        Vector3 difference = this.gameObject.transform.localPosition - this.world.PlayerPos;

        switch (facingDir)
        {
            case Direction.Up:
                if (difference.x < 0)
                {
                    return 1;
                }
                else
                {
                    return -1;
                }
                break;
            case Direction.Right:
                if (difference.z < 0)
                {
                    return 1;
                }
                else
                {
                    return -1;
                }
                break;
            case Direction.Down:
                if (difference.x < 0)
                {
                    return 1;
                }
                else
                {
                    return -1;
                }
                break;
            case Direction.Left:
                if (difference.z < 0)
                {
                    return -1;
                }
                else
                {
                    return 1;
                }
                break;
            default:
                break;
        }


        return Vector3.Angle(this.gameObject.transform.localPosition, this.world.PlayerPos);
    }
}