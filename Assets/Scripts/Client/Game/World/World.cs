using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace Game
{
    namespace World
    {
        /// <summary>
        /// A class for handling chunk loading and unloading
        /// </summary>
        public class GameWorld : MonoBehaviour
        {
            /// <summary>
            /// Chunk prefab for the chunk creation
            /// </summary>
            [SerializeField]
            private GameObject ChunkPrefab;

            [SerializeField]
            private int ScaleAmount = 1;

            /// <summary>
            /// The chunks of the world
            /// </summary>
            private Chunk[,] Chunks;

            public Chunk GetChunk(Vector3 GameObjectPos)
            {
                int row = (int)(GameObjectPos.z / (GameConfig.CHUNK_SIZE * GameConfig.CHUNK_SCALE * GameConfig.CHUNK_CELL));
                int col = (int)(GameObjectPos.x / (GameConfig.CHUNK_SIZE * GameConfig.CHUNK_SCALE * GameConfig.CHUNK_CELL));
                if (row < 0 || col < 0 || row >= GameConfig.CHUNK_COUNT || col >= GameConfig.CHUNK_COUNT)
                {
                    return null;
                }
                return Chunks[row, col];
            }

            public async Task CreateNewGame()
            {
                if (Chunks is not null)
                {
                    for (int i = 0; i < Chunks.GetLength(0); i++)
                    {
                        for (int j = 0; j < Chunks.GetLength(1); j++)
                        {
                            if (Chunks[i, j] != null)
                            {
                                Destroy(Chunks[i, j].gameObject);
                                Chunks[i, j] = null;
                            }
                        }
                    }
                }

                Chunks = new Chunk[GameConfig.CHUNK_COUNT, GameConfig.CHUNK_COUNT];

                await LoadChunk(GameConfig.CHUNK_COUNT / 2, GameConfig.CHUNK_COUNT / 2);
            }

            /// <summary>
            /// Loads a chunk
            /// </summary>
            /// <param name="x">The x index of the chunk (horizontal)</param>
            /// <param name="z">The z index of the chunk (vertical)</param>
            public async Task LoadChunk(int x, int z)
            {
                if (Chunks[z, x] == null)
                {
                    Chunks[z, x] = Instantiate(ChunkPrefab, this.transform, true).GetComponent<Chunk>();
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

                    await Chunks[z, x].InitChunk(x, z, edges, this);
                }
                else
                {
                    Chunks[z, x].Display();
                }
            }

            public void HideChunk(int x, int z)
            {
                if (Chunks[z, x] is not null)
                {
                    Chunks[z, x].HideChunk();
                }
            }

            public async Task<Chunk> GetChunk(int x, int z)
            {
                if (Chunks[z, x] is null)
                {
                    await LoadChunk(x, z);
                    HideChunk(x, z);
                }

                return Chunks[z, x];
            }
        }
    }
}