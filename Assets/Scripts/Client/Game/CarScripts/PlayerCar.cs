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
            float h = 0;
            if (!Input.touchSupported)
            {
                // pass the input to the car!

                h = CrossPlatformInputManager.GetAxis("Horizontal");
                float v = CrossPlatformInputManager.GetAxis("Vertical");
                float handbrake = CrossPlatformInputManager.GetAxis("Jump");
                carController.Move(h, v, v, handbrake);
            }
            else
            {
                bool reverse = false;
                if (Input.touchCount > 0)
                {
                    for (int i = 0; i < Input.touchCount; i++)
                    {
                        Touch touch = Input.GetTouch(i);

                        if (touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Stationary)
                        {
                            if (h != 0)
                            {
                                reverse = true;
                            }
                            //Check if the touch is on the right or left side of the screen

                            if (touch.position.x > Screen.width / 2)
                            {
                                h = 1;
                            }
                            else if (touch.position.x < Screen.width / 2)
                            {
                                h = -1;
                            }
                        }
                    }
                }
                carController.Move(reverse ? 0 : h, Input.touchCount > 1 ? -20 : 10, 0f, 0f);
            }

            Debug.Log(carController.CurrentSpeed);
        }
    }
}