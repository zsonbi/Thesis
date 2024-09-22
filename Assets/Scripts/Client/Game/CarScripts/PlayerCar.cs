using Game.World;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace Game
{
    internal class PlayerCar : Car
    {
        [SerializeField]
        private float coinMultiplier = 1;

        [SerializeField]
        public int SkinId = 1;

        private class PoliceContainer
        {
            public float TimeReamaining;
            public GameObject PoliceCar { get; private set; }

            public PoliceContainer(GameObject policeCar)
            {
                this.PoliceCar = policeCar;
                this.TimeReamaining = GameConfig.POLICE_GAME_OVER;
            }
        }

        private int probeSize = 1;

        private List<PoliceContainer> policeContacts = new List<PoliceContainer>();

        public void PickedUpCoin()
        {
            this.gameController.IncreaseCoinCount(1 * coinMultiplier);
        }

        protected override async void ChunkChanged(Chunk newChunk)
        {
            await gameController.LoadAndDespawnChunks(newChunk.Row, newChunk.Col);
        }

        protected override void Update()
        {
            base.Update();

            for (int i = 0; i < policeContacts.Count; i++)
            {
                policeContacts[i].TimeReamaining -= Time.deltaTime;
                if (policeContacts[i].TimeReamaining <= 0f)
                {
                    this.DestroyedEvent?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        private void FixedUpdate()
        {
            if (!Alive)
            {
                carController.Move(0f, 0f, 0f, 0f);
                return;
            }

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

            //   Debug.Log(carController.CurrentSpeed);
        }

        protected override void OnCollisionEnter(Collision collision)
        {
            base.OnCollisionEnter(collision);

            if (collision.gameObject.tag == "Police")
            {
                policeContacts.Add(new PoliceContainer(collision.gameObject));
            }
        }

        private void OnCollisionExit(Collision collision)
        {
            if (collision.gameObject.tag == "Police")
            {
                policeContacts.RemoveAll(x => x.PoliceCar == collision.gameObject);

                // Debug.Log("Removed" + collision.gameObject);
            }
        }
    }
}