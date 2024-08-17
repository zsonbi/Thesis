using Game.World;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CarController))]
public class Car : MonoBehaviour
{
    protected World world;
    protected CarController carController;
    protected Chunk lastChunk;

    protected virtual void Update()
    {
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
        if (lastChunk != this.world.GetChunk(col, row))
        {
            ChunkChanged(this.world.GetChunk(col, row));

            lastChunk = this.world.GetChunk(col, row);
            this.gameObject.transform.parent = lastChunk.transform;
            return true;
        }

        return false;
    }

    protected virtual void ChunkChanged(Chunk newChunk)
    {
    }

    public void Init(World world)
    {
        this.world = world;
    }

    // Start is called before the first frame update
    private void Awake()
    {
        this.carController = this.gameObject.GetComponent<CarController>();
    }
}