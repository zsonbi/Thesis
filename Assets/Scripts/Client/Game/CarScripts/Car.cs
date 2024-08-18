using Game;
using Game.World;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        int row = (int)(this.gameObject.transform.position.z / (GameConfig.CHUNK_SIZE * GameConfig.CHUNK_SCALE * GameConfig.CHUNK_CELL));
        int col = (int)(this.gameObject.transform.position.x / (GameConfig.CHUNK_SIZE * GameConfig.CHUNK_SCALE * GameConfig.CHUNK_CELL));
        Debug.Log("col:" + col);
        Debug.Log("row:" + row);
        if (lastChunk != this.gameController.World.GetChunk(col, row))
        {
            ChunkChanged(this.gameController.World.GetChunk(col, row));

            lastChunk = this.gameController.World.GetChunk(col, row);
            this.gameObject.transform.parent = lastChunk.transform;
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