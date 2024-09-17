using Game.World;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace Game
{
    public class CopCar : NpcCar
    {
        protected override void Update()
        {
            if (!Alive)
            {
                carController.Move(0f, 0f, 0f, 0f);
                return;
            }
            base.Update();

            float steering = DetermineSteeringDirection();
            float v = 10;

            carController.Move(steering, v, 0f, 0f);
        }
    }
}