using DataTypes;
using Game.World;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game
{
    /// <summary>
    /// Handles the player's car
    /// </summary>
    public class PlayerCar : Car
    {
        /// <summary>
        /// The multiplier for the coins if we later want a car which gives more coins
        /// </summary>
        [SerializeField]
        private float coinMultiplier = 1;

        /// <summary>
        /// The skin's id logically in the database
        /// </summary>
        [SerializeField]
        public int SkinId = 1;

        /// <summary>
        /// Reference to the keyboard
        /// </summary>
        private Keyboard keyboard;

        /// <summary>
        /// A container to store the police to track how long they have been touching the player for game over detection
        /// </summary>
        private class PoliceContainer
        {
            /// <summary>
            /// How long till game over
            /// </summary>
            public float TimeReamaining;
            /// <summary>
            /// Reference to the police car
            /// </summary>
            public GameObject PoliceCar { get; private set; }
            /// <summary>
            /// Creates a new PoliceCar object
            /// </summary>
            /// <param name="policeCar">Reference to the police car</param>
            public PoliceContainer(GameObject policeCar)
            {
                this.PoliceCar = policeCar;
                this.TimeReamaining = GameConfig.POLICE_GAME_OVER;
            }
        }

        /// <summary>
        /// How big radius should the chunk load be
        /// </summary>
        private int probeSize = 1;
        /// <summary>
        /// Keeping track of the police cars which touched the player
        /// </summary>
        private List<PoliceContainer> policeContacts = new List<PoliceContainer>();
        /// <summary>
        /// Is the player currently immune
        /// </summary>
        public bool Immune { get; private set; } = false;
        /// <summary>
        /// Does the player has turbo active
        /// </summary>
        public bool Turbo { get; private set; } = false;
        /// <summary>
        /// How long till the turbo is over
        /// </summary>
        private float turboTimer = 0f;
        /// <summary>
        /// How long till the immunity is over
        /// </summary>
        private float immuneTimer = 0f;

        /// <summary>
        /// When the player picks up a coin
        /// </summary>
        public void PickedUpCoin()
        {
            this.gameController.IncreaseCoinCount(1 * coinMultiplier);
        }
        /// <summary>
        /// Override the chunk changed method to make it load and despawn the chunks
        /// </summary>
        /// <param name="newChunk">The new chunk of the player</param>
        protected override async void ChunkChanged(Chunk newChunk)
        {
            await gameController.LoadAndDespawnChunks(newChunk.Row, newChunk.Col);
        }
        /// <summary>
        /// Apply the turbo bonus to the player
        /// </summary>
        public void ApplyTurbo()
        {
            if (effects.ContainsKey(EffectType.Turbo))
            {
                effects[EffectType.Turbo].gameObject.SetActive(true);
            }
            this.turboTimer = 0f;
            this.Turbo = true;
        }

        /// <summary>
        /// Apply the immunity bonus to the player
        /// </summary>
        public void ApplyImmunity()
        {
            if (effects.ContainsKey(EffectType.Shield))
            {
                effects[EffectType.Shield].gameObject.SetActive(true);
            }
            this.immuneTimer = 0f;
            this.Immune = true;
        }

        /// <summary>
        /// Cancel the turbo bonus
        /// </summary>
        public void CancelTurbo()
        {
            Turbo = false;
            if (effects.ContainsKey(EffectType.Turbo))
            {
                effects[EffectType.Turbo].gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// Cancel the immunity bonus
        /// </summary>
        public void CancelImmunity()
        {
            Immune = false;
            if (effects.ContainsKey(EffectType.Shield))
            {
                effects[EffectType.Shield].gameObject.SetActive(false);
            }
        }
        /// <summary>
        /// Called every frame
        /// Check the game over state and timers
        /// </summary>
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
        /// <summary>
        /// Handle the user inputs
        /// </summary>
        private void FixedUpdate()
        {
            //For the  testing unload doesn't cause error
            try
            {
                if (!Alive)
                {
                    carController.Move(0f, 0f, 0f, 0f);
                    return;
                }

                float turning = 0;
                float accel = 0;
                this.keyboard = Keyboard.current;
                // pass the input to the car
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
            catch (System.NullReferenceException)
            {
                Debug.LogWarning("Player car null reference exception");

                return;
            }
            catch (MissingReferenceException)
            {
                Debug.LogWarning("Player car missing reference");

                return;
            }
        }

        /// <summary>
        /// Handle the collisions
        /// </summary>
        /// <param name="collision">The entity it collided with</param>
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

        /// <summary>
        /// When the collided entity leaves collision
        /// </summary>
        /// <param name="collision">The entity which left</param>
        private void OnCollisionExit(Collision collision)
        {
            if (collision.gameObject.tag == "Police")
            {
                policeContacts.RemoveAll(x => x.PoliceCar == collision.gameObject);

            }
        }
    }
}