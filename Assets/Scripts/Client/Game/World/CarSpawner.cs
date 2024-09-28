using Game;
using Game.World;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    namespace World
    {
        public class CarSpawner : MonoBehaviour
        {
            [SerializeField]
            private GameController gameController;

            [SerializeField]
            private List<GameObject> CopCars;

            [SerializeField]
            private List<GameObject> CitizenCars;

            private Dictionary<int, float> spawnIntervalsTier1 = new Dictionary<int, float>() { { 0, 6 }, { 1, 5}, { 2, 4 }, { 3, 3 }, { 4, 1 }, { 5, 1 } };
            private Dictionary<int, float> spawnIntervalsTier2 = new Dictionary<int, float>() { { 0, float.PositiveInfinity }, { 1, float.PositiveInfinity }, { 2, float.PositiveInfinity }, { 3, 8 }, { 4, 6 }, { 5, 3 } };
            private Dictionary<int, float> spawnIntervalsTier3 = new Dictionary<int, float>() { { 0, float.PositiveInfinity }, { 1, float.PositiveInfinity }, { 2, float.PositiveInfinity }, { 3, float.PositiveInfinity }, { 4, 10 }, { 5, 5 } };

            private float tier1SpawnerTimer = 0f;
            private float tier2SpawnerTimer = 0f;
            private float tier3SpawnerTimer = 0f;

            public void Update()
            {
                if (!gameController.Running)
                {
                    return;
                }

                tier1SpawnerTimer += Time.deltaTime;
                tier2SpawnerTimer += Time.deltaTime;
                tier3SpawnerTimer += Time.deltaTime;

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

            public void SpawnPolice(GameObject carPrefab)
            {
                Chunk playerChunk = gameController.World.GetChunk(gameController.PlayerPos);
                CopCar copCar = Instantiate(carPrefab, playerChunk.gameObject.transform, true).GetComponent<CopCar>();
                copCar.Init(gameController);
                Vector3 roadPos = playerChunk.GetARandomRoad();
                copCar.transform.transform.localPosition = new Vector3(roadPos.x * GameConfig.CHUNK_SCALE * GameConfig.CHUNK_CELL, playerChunk.gameObject.transform.position.y + 2, roadPos.z * GameConfig.CHUNK_SCALE * GameConfig.CHUNK_CELL);
            }

            public void Reset()
            {
                tier1SpawnerTimer = 0f;
                tier2SpawnerTimer = 0f;
                tier3SpawnerTimer = 0f;
            }

            public void SpawnNpc()
            {
            }
        }
    }
}