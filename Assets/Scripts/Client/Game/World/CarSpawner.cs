using Codice.CM.Client.Differences;
using Game;
using Game.World;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSpawner : MonoBehaviour
{
    [SerializeField]
    private GameController gameController;

    [SerializeField]
    private List<GameObject> CopCars;

    [SerializeField]
    private List<GameObject> CitizenCars;

    private float timer = 0;

    private const float spawnInterval = 10f;

    public void Update()
    {
        timer += Time.deltaTime;
        if (timer > spawnInterval)
        {
            timer = 0;
            SpawnPolice();
        }
    }

    public void SpawnPolice()
    {
        foreach (var item in CopCars)
        {
            Chunk playerChunk = gameController.World.GetChunk(gameController.PlayerPos);

            CopCar copCar = Instantiate(item, playerChunk.gameObject.transform, true).GetComponent<CopCar>();
            copCar.Init(gameController);

            Vector3 roadPos = playerChunk.GetARandomRoad();
            copCar.transform.transform.localPosition = new Vector3(roadPos.x * GameConfig.CHUNK_SCALE * GameConfig.CHUNK_CELL, playerChunk.gameObject.transform.position.y + 2, roadPos.z * GameConfig.CHUNK_SCALE * GameConfig.CHUNK_CELL);
        }
    }

    public void SpawnNpc()
    {
    }
}