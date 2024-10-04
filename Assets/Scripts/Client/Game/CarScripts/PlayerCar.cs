using DataTypes;
using Game.World;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TouchPhase = UnityEngine.TouchPhase;

namespace Game
{
    public class PlayerCar : Car
    {
        [SerializeField]
        private float coinMultiplier = 1;

        [SerializeField]
        public int SkinId = 1;

        private Keyboard keyboard;

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

        public bool Immune { get; private set; } = false;
        public bool Turbo { get; private set; } = false;

        private float turboTimer = 0f;
        private float immuneTimer = 0f;

        protected new void Awake()
        {
            base.Awake();
        }

        public void PickedUpCoin()
        {
            this.gameController.IncreaseCoinCount(1 * coinMultiplier);
        }

        protected override async void ChunkChanged(Chunk newChunk)
        {
            await gameController.LoadAndDespawnChunks(newChunk.Row, newChunk.Col);
        }

        public void ApplyTurbo()
        {
            if (effects.ContainsKey(EffectType.Turbo))
            {
                effects[EffectType.Turbo].gameObject.SetActive(true);
            }
            this.turboTimer = 0f;
            this.Turbo = true;
        }

        public void ApplyImmunity()
        {
            if (effects.ContainsKey(EffectType.Shield))
            {
                effects[EffectType.Shield].gameObject.SetActive(true);
            }
            this.immuneTimer = 0f;
            this.Immune = true;
        }

        protected override void Update()
        {
            if (!gameController.Running)
            {
                return;
            }

            if (Turbo)
            {
                turboTimer += Time.deltaTime;
                if (turboTimer > GameConfig.TURBO_DURATION)
                {
                    Turbo = false;
                    if (effects.ContainsKey(EffectType.Turbo))
                    {
                        effects[EffectType.Turbo].gameObject.SetActive(false);
                    }
                }
            }

            if (Immune)
            {
                immuneTimer += Time.deltaTime;
                if (immuneTimer > GameConfig.TURBO_DURATION)
                {
                    Immune = false;
                    if (effects.ContainsKey(EffectType.Shield))
                    {
                        effects[EffectType.Shield].gameObject.SetActive(false);
                    }
                }
            }

            base.Update();

            for (int i = 0; i < policeContacts.Count; i++)
            {
                policeContacts[i].TimeReamaining -= Time.deltaTime;
                if (policeContacts[i].TimeReamaining <= 0f)
                {
                    this.DestroyedEvent?.Invoke(this, EventArgs.Empty);
                }
            }
            //If the car fell of the world trigger game over
            if (this.gameObject.transform.position.y < -5f)
            {
                this.DestroyedEvent?.Invoke(this, EventArgs.Empty);
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
            float accel = 0;
            this.keyboard = Keyboard.current;
            // pass the input to the car!
            if (this.keyboard is not null)
            {
                InputSystem.Update();
                if (this.keyboard.aKey.isPressed || this.keyboard.leftArrowKey.isPressed)
                {
                    turning = -1;
                }
                else if (this.keyboard.dKey.isPressed || this.keyboard.rightArrowKey.isPressed)
                {
                    turning = 1;
                }
                //turning = Input.GetAxis("Horizontal");

                if (this.keyboard.wKey.isPressed || this.keyboard.upArrowKey.isPressed)
                {
                    accel = 1;
                }
                else if (this.keyboard.sKey.isPressed || this.keyboard.downArrowKey.isPressed)
                {
                    accel = -1;
                }
            }

            //float accel = Input.GetAxis("Vertical");
            bool reverse = false;
            //  if (Input.touchSupported)
            // {
            if (Input.touchCount > 0)
            {
                for (int i = 0; i < Input.touchCount; i++)
                {
                    Touch touch = Input.GetTouch(i);

                    if (touch.phase == UnityEngine.TouchPhase.Began || touch.phase == UnityEngine.TouchPhase.Stationary)
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
                accel = Input.touchCount > 1 ? -1 : 1;
            carController.Move(reverse ? 0 : turning, (Turbo ? accel * 4 : accel), 0f, 0f);
        }

        protected override void OnCollisionEnter(Collision collision)
        {
            if (!Immune)
            {
                base.OnCollisionEnter(collision);
            }
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