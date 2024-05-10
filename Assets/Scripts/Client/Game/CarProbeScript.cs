using Game.World;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class CarProbeScript : MonoBehaviour
{
    [SerializeField]
    private World world;

    // Start is called before the first frame update
    private void Start()
    {
    }

    private void OnTriggerEnter(Collider collision)
    {
        Debug.Log(collision.gameObject.name);
        Chunk chunk = collision.gameObject.transform.parent.gameObject.GetComponent<Chunk>();

        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (i + chunk.Row >= 0 && j + chunk.Col >= 0 && i + chunk.Row < GameConfig.CHUNK_COUNT && j + chunk.Col < GameConfig.CHUNK_COUNT)
                {
                    world.LoadChunk(j + chunk.Col, i + chunk.Row);
                }
            }
        }

        for (int i = -2; i <= 2; i++)
        {
            for (int j = -2; j <= 2; j++)
            {
                if (Mathf.Abs(i) <= 1 && Mathf.Abs(j) <= 1)
                {
                    continue;
                }

                if (i + chunk.Row >= 0 && j + chunk.Col >= 0 && i + chunk.Row < GameConfig.CHUNK_COUNT && j + chunk.Col < GameConfig.CHUNK_COUNT)
                {
                    world.HideChunk(j + chunk.Col, i + chunk.Row);
                }
            }
        }
    }
}