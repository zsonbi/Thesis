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
                facingDir = Direction.Right;
            }
        }
        else
        {
            if (rotation.y > -90)
            {
                facingDir = Direction.Left;
            }
            else
            {
                facingDir = Direction.Down;
            }
        }

        Vector3 difference = this.world.PlayerPos - this.gameObject.transform.localPosition;
        // difference *= -1;
        Quaternion.RotateTowards(Quaternion.LookRotation(difference), this.gameObject.transform.rotation, Time.deltaTime);
        Vector3 angles = Quaternion.LookRotation(difference).eulerAngles;
        Vector3 crossProduct = Vector3.Cross(this.transform.forward, difference);
        Debug.Log(crossProduct.y);
        // Debug.Log((this.gameObject.transform. - Quaternion.LookRotation(difference)).eulerAngles);
        return (crossProduct.y);
        // this.transform.rotation = Quaternion.LookRotation(difference);
        this.transform.rotation = Quaternion.RotateTowards(Quaternion.LookRotation(difference), this.gameObject.transform.rotation, 10);
        //this.transform.rotation = Quaternion.LookRotation(difference);
        return 0;
        Debug.Log($"x:{difference.x} z:{difference.z}");

        Debug.Log(Mathf.Rad2Deg * Mathf.Atan2(difference.z, difference.x));

        return Mathf.Atan2(difference.z, difference.x) / 3.14f;
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
                    return -1;
                }
                else
                {
                    return 1;
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
                    return 1;
                }
                else
                {
                    return -1;
                }
                break;

            default:
                break;
        }

        return Vector3.Angle(this.gameObject.transform.localPosition, this.world.PlayerPos);
    }
}