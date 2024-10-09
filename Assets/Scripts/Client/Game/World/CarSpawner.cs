using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    namespace World
    {
        /// <summary>
        /// Car spawner
        /// </summary>
        public class CarSpawner : ThreadSafeMonoBehaviour
        {
            /// <summary>
            /// Reference to the game controller
            /// </summary>
            [SerializeField]
            private GameController gameController;

            /// <summary>
            /// Cop car prefabs
            /// </summary>
            [SerializeField]
            private List<GameObject> CopCars;

            /// <summary>
            /// Citizen car prefabs
            /// </summary>
            [SerializeField]
            private List<GameObject> CitizenCars;

            /// <summary>
            /// Tier one car spawn timers (regular cop car)
            /// </summary>
            private Dictionary<int, float> spawnIntervalsTier1 = new Dictionary<int, float>() { { 0, 6 }, { 1, 5 }, { 2, 4 }, { 3, 3 }, { 4, 1 }, { 5, 1 } };

            /// <summary>
            /// Tier two car spawn timers (cop van)
            /// </summary>
            private Dictionary<int, float> spawnIntervalsTier2 = new Dictionary<int, float>() { { 0, float.PositiveInfinity }, { 1, float.PositiveInfinity }, { 2, float.PositiveInfinity }, { 3, 8 }, { 4, 6 }, { 5, 3 } };

            /// <summary>
            /// Tier three car spawn timers (tank)
            /// </summary>
            private Dictionary<int, float> spawnIntervalsTier3 = new Dictionary<int, float>() { { 0, float.PositiveInfinity }, { 1, float.PositiveInfinity }, { 2, float.PositiveInfinity }, { 3, float.PositiveInfinity }, { 4, 10 }, { 5, 5 } };

            /// <summary>
            /// Tier one cop car timer
            /// </summary>
            private float tier1SpawnerTimer = 0f;

            /// <summary>
            /// Tier two cop car timer
            /// </summary>
            private float tier2SpawnerTimer = 0f;

            /// <summary>
            /// Tier three cop car timer
            /// </summary>
            private float tier3SpawnerTimer = 0f;

            /// <summary>
            /// Called every frame and increase the timers and spawn if they reached the threshold
            /// </summary>
            public void Update()
            {
                //Make it only run during running game
                if (!gameController.Running)
                {
                    return;
                }

                //Increase the timers

                tier1SpawnerTimer += Time.deltaTime;
                tier2SpawnerTimer += Time.deltaTime;
                tier3SpawnerTimer += Time.deltaTime;

                //After threshold is reached spawn the car

                if (tier1SpawnerTimer > spawnIntervalsTier1[gameController.Difficulty])
                {
                    SpawnPolice(CopCars[0]);
                    tier1SpawnerTimer = 0f;
                }
                if (tier2SpawnerTimer > spawnIntervalsTier2[gameController.Difficulty])
                {
                    SpawnPolice(CopCars[1]);
                    tier2SpawnerTimer = 0f;
                }
                if (tier3SpawnerTimer > spawnIntervalsTier3[gameController.Difficulty])
                {
                    SpawnPolice(CopCars[2]);
                    tier3SpawnerTimer = 0f;
                }
            }

            /// <summary>
            /// Spawn the given cop car
            /// </summary>
            /// <param name="carPrefab">Cop car to spawn</param>
            public void SpawnPolice(GameObject carPrefab)
            {
                Chunk playerChunk = gameController.World.GetChunk(gameController.PlayerPos);
                CopCar copCar = Instantiate(carPrefab, playerChunk.gameObject.transform, true).GetComponent<CopCar>();
                copCar.Init(gameController);
                Vector3 roadPos = playerChunk.GetARandomRoad();
                copCar.transform.transform.localPosition = new Vector3(roadPos.x * GameConfig.CHUNK_SCALE * GameConfig.CHUNK_CELL, playerChunk.gameObject.transform.position.y + 2, roadPos.z * GameConfig.CHUNK_SCALE * GameConfig.CHUNK_CELL);
            }

            /// <summary>
            /// Reset the timers
            /// </summary>
            public void Reset()
            {
                tier1SpawnerTimer = 0f;
                tier2SpawnerTimer = 0f;
                tier3SpawnerTimer = 0f;
            }

            /// <summary>
            /// Spawn the npc car
            /// </summary>
            public void SpawnNpc()
            {
            }
        }
    }
}