using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game
{
    namespace World
    {
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
                        LoadChunk(j, i);
                    }
                }
            }

            public void LoadChunk(int x, int z)
            {
                if (Chunks[z, x] == null)
                {
                    Chunks[z, x] = Instantiate(ChunkPrefab, this.transform, false).GetComponent<Chunk>();
                    List<EdgeRoadContainer> edges = new List<EdgeRoadContainer>();
                    if (z - 1 >= 0 && Chunks[z - 1, x] != null)
                    {
                        edges.AddRange(Chunks[z - 1, x].EdgeRoads.Where(x => x.EdgeRoad.y == GameConfig.CHUNK_SIZE - 1));
                    }
                    if (z + 1 < GameConfig.CHUNK_COUNT && Chunks[z + 1, x] != null)
                    {
                        edges.AddRange(Chunks[z + 1, x].EdgeRoads.Where(x => x.EdgeRoad.y == 0));
                    }
                    if (x - 1 >= 0 && Chunks[z, x - 1] != null)
                    {
                        edges.AddRange(Chunks[z, x - 1].EdgeRoads.Where(x => x.EdgeRoad.x == GameConfig.CHUNK_SIZE - 1));
                    }
                    if (x + 1 < GameConfig.CHUNK_COUNT && Chunks[z, x + 1] != null)
                    {
                        edges.AddRange(Chunks[z, x + 1].EdgeRoads.Where(x => x.EdgeRoad.x == 0));
                    }

                    Chunks[z, x].InitChunk(x, z, edges);
                }
            }
        }
    }
}