using Game.World;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class CarProbeScript : MonoBehaviour
{
    [SerializeField]
    private World world;

    private int probeSize = 1;

    private void OnTriggerEnter(Collider collision)
    {
        Debug.Log(collision.gameObject.name);
        Chunk chunk = collision.gameObject.transform.parent.transform.parent.gameObject.GetComponent<Chunk>();

        SpawnNearbyChunks(chunk.Row, chunk.Col);

        DespawnFarAwayChunks(chunk.Row, chunk.Col);
    }

    private void DespawnFarAwayChunks(int row, int col)
    {
        for (int i = -(probeSize + 1); i <= probeSize + 1; i++)
        {
            for (int j = -(probeSize + 1); j <= probeSize + 1; j++)
            {
                if (Mathf.Abs(i) <= probeSize && Mathf.Abs(j) <= probeSize)
                {
                    continue;
                }

                if (i + row >= 0 && j + col >= 0 && i + row < GameConfig.CHUNK_COUNT && j + col < GameConfig.CHUNK_COUNT)
                {
                    world.HideChunk(j + col, i + row);
                }
            }
        }
    }

    private void SpawnNearbyChunks(int row, int col)
    {
        ValidateAndLoadChunk(row, col);

        for (int currSize = 1; currSize <= probeSize; currSize++)
        {
            for (int x = 0 - currSize; x <= 0 + currSize; x += currSize * 2)
            {
                for (int y = 0; y <= currSize - 1; y++)
                {
                    ValidateAndLoadChunk(row + y, col + x);
                    ValidateAndLoadChunk(row - y, col + x);
                }
            }

            for (int y = 0 - currSize; y <= 0 + currSize; y += currSize * 2)
            {
                for (int x = 0; x <= currSize - 1; x++)
                {
                    ValidateAndLoadChunk(row + y, col + x);
                    ValidateAndLoadChunk(row + y, col - x);
                }
            }

            ValidateAndLoadChunk(row + currSize, col + currSize);
            ValidateAndLoadChunk(row - currSize, col - currSize);
            ValidateAndLoadChunk(row + currSize, col - currSize);
            ValidateAndLoadChunk(row - currSize, col + currSize);
        }
    }

    private void ValidateAndLoadChunk(int row, int col)
    {
        if (row < 0 || row >= GameConfig.CHUNK_COUNT || col < 0 || col >= GameConfig.CHUNK_COUNT)
        {
            return;
        }
        world.LoadChunk(col, row);
    }
}