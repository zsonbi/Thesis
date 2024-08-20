using Game.World;
using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace Game
{
    internal class PlayerCar : Car
    {
        private int probeSize = 1;

        protected override void ChunkChanged(Chunk newChunk)
        {
            gameController.LoadAndDespawnChunks(newChunk.Row, newChunk.Col);
        }

        private void FixedUpdate()
        {
            // pass the input to the car!
            float h = CrossPlatformInputManager.GetAxis("Horizontal");
            float v = CrossPlatformInputManager.GetAxis("Vertical");
#if !MOBILE_INPUT
            float handbrake = CrossPlatformInputManager.GetAxis("Jump");
            carController.Move(h, v, v, handbrake);
#else
            carController.Move(h, v, v, 0f);
#endif

            Debug.Log(carController.CurrentSpeed);
        }
    }
}