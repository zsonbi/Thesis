using Cysharp.Threading.Tasks.Triggers;
using Game;
using Game.World;
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

        [SerializeField]
        private float health = 10f;

        [SerializeField]
        private GameObject colliders = null;

        public float Health { get => health; protected set => health = value; }

        protected virtual void Update()
        {
            if (gameController is null)
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
                return true;
            }

            return false;
        }

        protected virtual async void ChunkChanged(Chunk newChunk)
        {
        }

        public void Init(GameController world)
        {
            this.gameController = world;
        }

        private void OnCollisionEnter(Collision collision)
        {
            Debug.Log("collided " + gameObject.name + collision.relativeVelocity.sqrMagnitude);
        }

        // Start is called before the first frame update
        private void Awake()
        {
            this.carController = this.gameObject.GetComponent<CarController>();
            if (colliders is not null)
            {
                Collider[] carColliders = colliders.GetComponentsInChildren<Collider>();
            }
            else
            {
                Debug.LogError("Colliders was not set for car");
            }
        }
    }
}