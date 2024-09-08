using Codice.CM.Common;
using Cysharp.Threading.Tasks.Triggers;
using DataTypes;
using Game;
using Game.World;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    [RequireComponent(typeof(CarController))]
    public class Car : MonoBehaviour
    {
        protected GameController gameController;
        protected CarController carController;
        protected Chunk lastChunk;
        public EventHandler DestroyedEvent;

        [SerializeField]
        private float health = 10f;

        //[SerializeField]
        //private GameObject colliders = null;

        protected Dictionary<EffectType, EffectScript> effects = new Dictionary<EffectType, EffectScript>();

        public float Health { get => health; protected set => health = value; }

        public float MAX_HEALTH { get; private set; }

        public bool Alive => Health > 0;

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

        protected bool ChangeChunkIfNeeded()
        {
            Chunk newChunk = this.gameController.World.GetChunk(this.gameObject.transform.position);
            if (lastChunk != newChunk)
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

        protected virtual async void ChunkChanged(Chunk newChunk)
        {
        }

        public void Init(GameController world)
        {
            this.health = MAX_HEALTH;
            this.gameController = world;

            if (effects.ContainsKey(EffectType.Smoke))
                effects[EffectType.Smoke].gameObject.SetActive(false);

            if (effects.ContainsKey(EffectType.Fire))
                effects[EffectType.Fire].gameObject.SetActive(false);
        }

        private void OnCollisionEnter(Collision collision)
        {
            Debug.Log("Collided " + gameObject.name + "with " + collision.gameObject.name + "at " + collision.relativeVelocity.sqrMagnitude);
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

        // Start is called before the first frame update
        private void Awake()
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