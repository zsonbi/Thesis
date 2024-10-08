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
        public class GameWorld : ThreadSafeMonoBehaviour
        {
            /// <summary>
            /// Chunk prefab for the chunk creation
            /// </summary>
            [SerializeField]
            private GameObject ChunkPrefab;

            /// <summary>
            /// The chunks of the world
            /// </summary>
            private Chunk[,] Chunks;

            /// <summary>
            /// Gets the chunk based on the absolute position which chunk it would fall into
            /// </summary>
            /// <param name="GameObjectPos">The postion we're looking up</param>
            /// <returns>The chunk or null if not found</returns>
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

            /// <summary>
            /// Creates a new game and create the chunks
            /// </summary>
            public async Task CreateNewGame()
            {
                //Delete the previous chunks
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
                //Create the chunks
                Chunks = new Chunk[GameConfig.CHUNK_COUNT, GameConfig.CHUNK_COUNT];
                //Load the initial chunk
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
                    //Destroyed check
                    if (Destroyed)
                    {
                        return;
                    }
                    //Instantiate the prefab
                    Chunks[z, x] = Instantiate(ChunkPrefab, this.transform, true).GetComponent<Chunk>();
                    //Get the road edges for the road generation
                    List<EdgeRoadContainer> edges = new List<EdgeRoadContainer>();
                    if (z - 1 >= 0 && Chunks[z - 1, x] != null)
                    {
                        if (Chunks[z - 1, x].EdgeRoads is null)
                        {
                            return;
                        }

                        edges.AddRange(Chunks[z - 1, x].EdgeRoads.Where(x => x?.EdgeRoad.y == GameConfig.CHUNK_SIZE - 1));
                    }
                    if (z + 1 < GameConfig.CHUNK_COUNT && Chunks[z + 1, x] != null)
                    {
                        if (Chunks[z + 1, x].EdgeRoads is null)
                        {
                            return;
                        }
                        edges.AddRange(Chunks[z + 1, x].EdgeRoads.Where(x => x?.EdgeRoad.y == 0));
                    }
                    if (x - 1 >= 0 && Chunks[z, x - 1] != null)
                    {
                        if (Chunks[z, x - 1].EdgeRoads is null)
                        {
                            return;
                        }
                        edges.AddRange(Chunks[z, x - 1].EdgeRoads.Where(x => x?.EdgeRoad.x == GameConfig.CHUNK_SIZE - 1));
                    }
                    if (x + 1 < GameConfig.CHUNK_COUNT && Chunks[z, x + 1] != null)
                    {
                        if (Chunks[z, x + 1].EdgeRoads is null)
                        {
                            return;
                        }
                        edges.AddRange(Chunks[z, x + 1].EdgeRoads.Where(x => x?.EdgeRoad.x == 0));
                    }
                    //Create the chunk
                    await Chunks[z, x].InitChunk(x, z, edges, this);
                }
                else
                {
                    //If the chunk is already loaded display it
                    Chunks[z, x].Display();
                }
            }

            /// <summary>
            /// Hide the given chunk
            /// </summary>
            /// <param name="col">The chunk's column index</param>
            /// <param name="row">The row chunk's index</param>
            public void HideChunk(int col, int row)
            {
                if (Chunks[row, col] is not null)
                {
                    Chunks[row, col].HideChunk();
                }
            }

            /// <summary>
            /// Get the chunk without trying it to load
            /// </summary>
            /// <param name="col">The chunk's column index</param>
            /// <param name="row">The row chunk's index</param>
            /// <returns>The chunk or null if it wasn't loaded yet</returns>
            public Chunk GetChunkWithoutLoad(int col, int row)
            {
                return Chunks[row, col];
            }

            /// <summary>
            /// Gets the chunk it will load it if it hasn't been and hide it then otherwise just return it
            /// </summary>
            /// <param name="col">The chunk's column index</param>
            /// <param name="row">The row chunk's index</param>
            /// <returns>The chunk</returns>
            public async Task<Chunk> GetChunk(int col, int row)
            {
                if (Chunks[row, col] is null)
                {
                    await LoadChunk(col, row);
                    HideChunk(col, row);
                }

                return Chunks[row, col];
            }
        }
    }
}