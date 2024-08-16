using Game.World;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class CopCar : NpcCar
{
    public void Update()
    {
        float steering = DetermineSteeringDirection();
        float v = 10;

        carController.Move(steering, v, 0f, 0f);
    }
}