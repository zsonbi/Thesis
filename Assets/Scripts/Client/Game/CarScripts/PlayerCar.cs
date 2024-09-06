using Game.World;
using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace Game
{
    internal class PlayerCar : Car
    {
        private int probeSize = 1;

        protected override async void ChunkChanged(Chunk newChunk)
        {
            await gameController.LoadAndDespawnChunks(newChunk.Row, newChunk.Col);
        }

        private void FixedUpdate()
        {

            float turning = 0;

            // pass the input to the car!

            turning = CrossPlatformInputManager.GetAxis("Horizontal");
            float accel = CrossPlatformInputManager.GetAxis("Vertical");
            bool reverse = false;
            //  if (Input.touchSupported)
            // {
            if (Input.touchCount > 0)
            {
                for (int i = 0; i < Input.touchCount; i++)
                {
                    Touch touch = Input.GetTouch(i);

                    if (touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Stationary)
                    {
                        if (turning != 0)
                        {
                            reverse = true;
                        }
                        //Check if the touch is on the right or left side of the screen

                        if (touch.position.x > Screen.width / 2)
                        {
                            turning = 1;
                        }
                        else if (touch.position.x < Screen.width / 2)
                        {
                            turning = -1;
                        }
                    }
                }
            }
            if (accel == 0)
                accel = Input.touchCount > 1 ? -20 : 10;
            //   }
            carController.Move(reverse ? 0 : turning, accel, 0f, 0f);

            Debug.Log(carController.CurrentSpeed);
        }
    }
}