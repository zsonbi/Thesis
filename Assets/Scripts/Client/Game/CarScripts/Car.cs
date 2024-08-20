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

        protected virtual void ChunkChanged(Chunk newChunk)
        {
        }

        public void Init(GameController world)
        {
            this.gameController = world;
        }

        // Start is called before the first frame update
        private void Awake()
        {
            this.carController = this.gameObject.GetComponent<CarController>();
        }
    }
}