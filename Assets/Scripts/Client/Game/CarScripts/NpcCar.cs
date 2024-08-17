using Game.World;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcCar : Car
{
    public float DetermineSteeringDirection()
    {
        Vector3 difference = this.world.PlayerPos - this.gameObject.transform.position;
        Quaternion.RotateTowards(Quaternion.LookRotation(difference), this.gameObject.transform.rotation, Time.deltaTime);
        Vector3 crossProduct = Vector3.Cross(this.transform.forward, difference);
        //Debug.Log(crossProduct.y);
        return (crossProduct.y * 0.75f);
    }
}