using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

            public void CreateNewGame()
            {
                Chunks = new Chunk[GameConfig.CHUNK_COUNT, GameConfig.CHUNK_COUNT];

                LoadChunk(GameConfig.CHUNK_COUNT / 2, GameConfig.CHUNK_COUNT / 2);
            }

            /// <summary>
            /// Loads a chunk
            /// </summary>
            /// <param name="x">The x index of the chunk (horizontal)</param>
            /// <param name="z">The z index of the chunk (vertical)</param>
            public void LoadChunk(int x, int z)
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

                    Chunks[z, x].InitChunk(x, z, edges, this);
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

            public Chunk GetChunk(int x, int z)
            {
                if (Chunks[z, x] is null)
                {
                    LoadChunk(x, z);
                    HideChunk(x, z);
                }

                return Chunks[z, x];
            }
        }
    }
}