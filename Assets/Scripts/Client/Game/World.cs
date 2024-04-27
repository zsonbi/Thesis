using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    [SerializeField]
    private GameObject ChunkPrefab;

    private Chunk[,] Chunks;

    // Start is called before the first frame update
    private void Start()
    {
        Chunks = new Chunk[GameConfig.CHUNK_COUNT, GameConfig.CHUNK_COUNT];

        for (int i = 0; i < GameConfig.CHUNK_COUNT; i++)
        {
            for (int j = 0; j < GameConfig.CHUNK_COUNT; j++)
            {
                Chunks[i, j] = Instantiate(ChunkPrefab, this.transform, false).GetComponent<Chunk>();

                Chunks[i, j].InitChunk(j, i);
            }
        }
    }
}