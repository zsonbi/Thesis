using Cysharp.Threading.Tasks.Triggers;
using DataTypes;
using Game;
using Game.World;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;

namespace Game
{
    /// <summary>
    /// Handles the basic Car functionalities
    /// </summary>
    [RequireComponent(typeof(CarController))]
    public class Car : ThreadSafeMonoBehaviour
    {
        protected GameController gameController;
        protected CarController carController;
        protected Chunk lastChunk;
        public EventHandler DestroyedEvent;

        [SerializeField]
        private float health = 10f;

        /// <summary>
        /// The possible effects of the car
        /// </summary>
        protected Dictionary<EffectType, EffectScript> effects = new Dictionary<EffectType, EffectScript>();

        /// <summary>
        /// The current health of the car
        /// </summary>
        public float Health { get => health; protected set => health = value; }

        /// <summary>
        /// The health of the car on spawn
        /// </summary>
        public float MAX_HEALTH { get; private set; }

        /// <summary>
        /// Is the car still alive
        /// </summary>
        public bool Alive => Health > 0;

        /// <summary>
        /// Called every frame
        /// </summary>
        protected virtual void Update()
        {
            if (gameController is null || !Alive)
            {
                return;
            }
            if (ChangeChunkIfNeeded())
            {
            }
        }

        /// <summary>
        /// Kills the car from the outside
        /// </summary>
        public void Kill()
        {
            this.health = 0;
            this.DestroyedEvent?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Change the chunk if it went over the chunk border
        /// </summary>
        /// <returns>true-changed chunk, false-didn't change chunk</returns>
        protected bool ChangeChunkIfNeeded()
        {
            Chunk newChunk = this.gameController.World.GetChunk(this.gameObject.transform.position);
            if (lastChunk != newChunk && newChunk != null)
            {
                ChunkChanged(newChunk);

                lastChunk = newChunk;
                this.gameObject.transform.parent = newChunk.transform;
                if (this.gameObject.isStatic)
                {
                    this.gameObject.isStatic = false;
                }
                return true;
            }

            return false;
        }

        /// <summary>
        /// Virtual method for chunk changed
        /// </summary>
        /// <param name="newChunk">The new chunk to change to</param>
        protected virtual async void ChunkChanged(Chunk newChunk)
        {
        }

        /// <summary>
        /// Initializes the car
        /// </summary>
        /// <param name="gameController">Reference to the game's controller</param>
        public void Init(GameController gameController)
        {
            this.health = MAX_HEALTH;
            this.gameController = gameController;
            //Init the effects
            if (effects.ContainsKey(EffectType.Smoke))
                effects[EffectType.Smoke].gameObject.SetActive(false);

            if (effects.ContainsKey(EffectType.Fire))
                effects[EffectType.Fire].gameObject.SetActive(false);

            if (effects.ContainsKey(EffectType.Shield))
                effects[EffectType.Shield].gameObject.SetActive(false);

            if (effects.ContainsKey(EffectType.Turbo))
                effects[EffectType.Turbo].gameObject.SetActive(false);
        }

        /// <summary>
        /// Car damage control
        /// </summary>
        /// <param name="collision">What it collided with</param>
        protected virtual void OnCollisionEnter(Collision collision)
        {
            float dmgAmount = collision.relativeVelocity.sqrMagnitude / 500f;

            if (dmgAmount > GameConfig.CAR_DAMAGE_LOWER_LIMIT)
            {
                if (dmgAmount > GameConfig.CAR_DAMAGE_UPPER_LIMIT)
                {
                    dmgAmount = GameConfig.CAR_DAMAGE_UPPER_LIMIT;
                }

                Health -= dmgAmount;

                if (health < 0)
                {
                    DestroyedEvent?.Invoke(this, EventArgs.Empty);
                }

                if (Health / MAX_HEALTH < GameConfig.SMOKE_THRESHOLD)
                {
                    if (effects.ContainsKey(EffectType.Smoke))
                        effects[EffectType.Smoke].gameObject.SetActive(true);
                }
                if (Health / MAX_HEALTH < GameConfig.FIRE_THRESHOLD)
                {
                    if (effects.ContainsKey(EffectType.Fire))
                        effects[EffectType.Fire].gameObject.SetActive(true);
                }
            }
        }

        /// <summary>
        /// Awake called when script is loaded
        /// </summary>
        protected void Awake()
        {
            this.carController = this.gameObject.GetComponent<CarController>();
            this.MAX_HEALTH = health;
            foreach (var item in this.GetComponentsInChildren<EffectScript>())
            {
                item.gameObject.SetActive(false);
                if (!effects.ContainsKey(item.EffectType))
                {
                    effects.Add(item.EffectType, item);
                }
                else
                {
                    Debug.LogError("multiple events with this type");
                }
            }
        }
    }
}