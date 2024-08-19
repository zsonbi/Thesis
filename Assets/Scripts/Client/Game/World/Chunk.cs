using UnityEngine;
using System.Collections.Generic;
using DataTypes;
using Assets.Scripts.Client.DataTypes;
using System.Linq;

namespace Game
{
    namespace World
    {
        /// <summary>
        /// Used for generating a new world
        /// </summary>
        public class Chunk : MonoBehaviour
        {
            //[Header("Materials for the tiles")]
            //[SerializeField]
            //private Material[] TileMaterials;

            //[SerializeField]
            //[Header("PhysicsMaterials")]
            //private PhysicMaterial[] PhysicsMaterials;

            [Header("Offset of perlin noise on X axis")]
            public float XOffset = 0f;

            [Header("Offset of perlin noise on Z axis")]
            public float ZOffset = 0f;

            [Header("Randomize Offset")]
            public bool RandomizeOffset = false;

            [SerializeField]
            public GameObject straightRoadPrefab;

            [SerializeField]
            private GameObject curvedRoadPrefab;

            [SerializeField]
            private GameObject threeWayRoadPrefab;

            [SerializeField]
            private GameObject crossRoadsPrefab;

            [SerializeField]
            public GameObject grassPrefab;

            [SerializeField]
            public PhysicMaterial grassPhysics;

            /// <summary>
            /// The size of the world on the z axis
            /// </summary>
            public int zSize { get; private set; }

            /// <summary>
            /// The size of the world on the x axis
            /// </summary>
            public int xSize { get; private set; }

            public int Row { get; private set; }
            public int Col { get; private set; }

            public bool Loaded { get => this.gameObject.activeSelf; }

            public List<EdgeRoadContainer> EdgeRoads { get => roadGenerator.EdgeRoads; }

            private Dictionary<ChunkCellType, List<Vector3>> chunkCells = new Dictionary<ChunkCellType, List<Vector3>>();
            private RoadGenerator roadGenerator;
            private GameWorld world;
            private List<Vector3> roads = new List<Vector3>();

            // Start is called before the first frame update
            private void Awake()
            {
                //Load the values from the settings
                LoadFromSettings();
                //Randomizes the offset
                if (RandomizeOffset)
                {
                    this.XOffset = Random.Range(0, 99999);
                    this.ZOffset = Random.Range(0, 99999);
                }
            }

            public void Display()
            {
                this.gameObject.SetActive(true);
            }

            public void InitChunk(int xOffset, int zOffset, List<EdgeRoadContainer> edgeRoads, GameWorld world)
            {
                this.world = world;
                this.Row = zOffset;
                this.Col = xOffset;
                for (ChunkCellType i = 0; i <= ChunkCellType.StraightRoad; i++)
                {
                    chunkCells.Add(i, new List<Vector3>());
                }
                if (edgeRoads.Count == 0)
                    Debug.Log($"Didn't get edges: ({zOffset},{xOffset})");

                roadGenerator = new RoadGenerator(GameConfig.CHUNK_SIZE, edgeRoads);

                //Create the tiles
                CreateTiles();
                ////Create the props
                //AddEnviromentObjects();
                //Combine the objects
                //for (byte i = 0; i < numberOfDifferentObjects; i++)
                //{
                //    if (objectsToCombine[i].Count != 0)
                //        CombineMeshes(i);
                //}

                //CreateMeshes();

                this.gameObject.transform.localPosition = new Vector3(xOffset * GameConfig.CHUNK_SIZE * GameConfig.CHUNK_SCALE * GameConfig.CHUNK_CELL, 0, zOffset * GameConfig.CHUNK_SIZE * GameConfig.CHUNK_SCALE * GameConfig.CHUNK_CELL);

                this.gameObject.transform.localScale = new Vector3(GameConfig.CHUNK_SCALE, 1, GameConfig.CHUNK_SCALE);
            }

            public void HideChunk()
            {
                this.gameObject.SetActive(false);
            }

            public Vector3 GetARandomRoad()
            {
                if (roads.Count == 0)
                {
                    return new Vector3(GameConfig.CHUNK_SIZE / 2, 0, GameConfig.CHUNK_SIZE / 2);
                }
                return roads[Random.Range(0, roads.Count)];
            }

            private void CreateMeshes()
            {
                foreach (var item in chunkCells)
                {
                    //GameObject parent = new GameObject(item.Key.ToString() + "Mesh", typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider));
                    //parent.transform.parent = this.transform;
                    //MeshFilter meshFilter = parent.GetComponent<MeshFilter>();
                    //parent.GetComponent<MeshRenderer>().material = TileMaterials[(int)item.Key];
                    //parent.GetComponent<MeshCollider>().sharedMaterial = PhysicsMaterials[(int)(item.Key)];
                    //meshFilter.mesh = MeshGenerator.CreateMultiShape(item.Value);
                    //parent.GetComponent<MeshCollider>().sharedMesh = meshFilter.mesh;
                }
            }

            //------------------------------------------------------------------------
            //Load the sizes from the settings
            private void LoadFromSettings()
            {
                this.xSize = GameConfig.CHUNK_SIZE;
                this.zSize = GameConfig.CHUNK_SIZE;
            }

            //---------------------------------------------------------------------------
            // <summary>
            // Creates the tiles for the world
            private void CreateTiles()
            {
                for (int x = 0; x < xSize; x++)
                {
                    for (int z = 0; z < zSize; z++)
                    {
                        ChunkCellContainer tileType = DetermineTileType(x, z);
                        GameObject created = null;
                        switch (tileType.Type)
                        {
                            case ChunkCellType.StraightRoad:
                                created = Instantiate(straightRoadPrefab, this.transform);
                                break;

                            case ChunkCellType.CurvedRoad:
                                created = Instantiate(curvedRoadPrefab, this.transform);
                                break;

                            case ChunkCellType.ThreeWay:
                                created = Instantiate(threeWayRoadPrefab, this.transform);
                                break;

                            case ChunkCellType.CrossRoads:
                                created = Instantiate(crossRoadsPrefab, this.transform);
                                break;

                            case ChunkCellType.Grass:
                                created = Instantiate(grassPrefab, this.transform);
                                //Collider renderer = created.GetComponentInChildren<Collider>();
                                //renderer.material = grassPhysics;
                                break;

                            case ChunkCellType.Sand:
                                break;

                            case ChunkCellType.Water:
                                break;

                            default:
                                break;
                        }
                        if (created != null)
                        {
                            created.transform.localScale = created.transform.localScale * 1.01f;
                            created.transform.localPosition = new Vector3(x * 32, created.transform.localPosition.y, z * 32);
                            if (tileType.Orientation != Vector3.zero)
                                created.transform.Rotate(tileType.Orientation);
                        }

                        //chunkCells[tileType].Add(new Vector3(x, 0f, z));
                    }
                }
            }

            private ChunkCellContainer DetermineRoadType(int x, int z)
            {
                int roadCount = 0;

                int firstIndex = -1;
                int lastIndex = -1;
                int secondIndex = -1;

                if (x - 1 >= 0 && roadGenerator.RoadMatrix[z, x - 1])
                {
                    roadCount++;
                    firstIndex = 0;
                    lastIndex = 0;
                }

                if (z + 1 < GameConfig.CHUNK_SIZE && roadGenerator.RoadMatrix[z + 1, x])
                {
                    roadCount++;

                    if (firstIndex == -1)
                    {
                        firstIndex = 1;
                    }
                    else
                    {
                        secondIndex = 1;
                    }
                    lastIndex = 1;
                }

                if (x + 1 < GameConfig.CHUNK_SIZE && roadGenerator.RoadMatrix[z, x + 1])
                {
                    roadCount++;

                    if (firstIndex == -1)
                    {
                        firstIndex = 2;
                    }
                    else if (secondIndex == -1)
                    {
                        secondIndex = 2;
                    }
                    lastIndex = 2;
                }

                if (z - 1 >= 0 && roadGenerator.RoadMatrix[z - 1, x])
                {
                    roadCount++;

                    if (firstIndex == -1)
                    {
                        firstIndex = 3;
                    }
                    else if (secondIndex == -1)
                    {
                        secondIndex = 3;
                    }
                    lastIndex = 3;
                }

                if (roadCount == 4)
                {
                    return new ChunkCellContainer(ChunkCellType.CrossRoads, Vector3.zero);
                }
                else if (roadCount == 3)
                {
                    //if (lastIndex == 3)
                    //{
                    //    return new ChunkCellContainer(ChunkCellType.ThreeWay, new Vector3(0, (firstIndex+1) * 90, 0));
                    //}

                    if (firstIndex == 0 && lastIndex == 3)
                    {
                        if (secondIndex == 1)
                        {
                            return new ChunkCellContainer(ChunkCellType.ThreeWay, new Vector3(0, (1) * 90, 0));
                        }
                        else
                        {
                            return new ChunkCellContainer(ChunkCellType.ThreeWay, Vector3.zero);
                        }
                    }

                    return new ChunkCellContainer(ChunkCellType.ThreeWay, new Vector3(0, (secondIndex + 1) * 90, 0));
                }
                else if (roadCount == 2 && (lastIndex - firstIndex) % 2 == 1)
                {
                    if (secondIndex == 3 && firstIndex == 0)
                    {
                        return new ChunkCellContainer(ChunkCellType.CurvedRoad, new Vector3(0, (-1) * 90, 0));
                    }

                    return new ChunkCellContainer(ChunkCellType.CurvedRoad, new Vector3(0, (lastIndex - 1) * 90, 0));
                }
                else
                {
                    return new ChunkCellContainer(ChunkCellType.StraightRoad, new Vector3(0, (lastIndex % 2 + 1) * 90, 0));
                }
            }

            //-------------------------------------------------------------------------
            /// <summary>
            /// Determines the type of the tile at the coord
            /// </summary>
            /// <param name="x">x coord where we want to sample the perlin noise</param>
            /// <param name="z">z coord where we want to sample the perlin noise</param>
            /// <returns>the proper TileType</returns>
            private ChunkCellContainer DetermineTileType(int x, int z)
            {
                if (roadGenerator.RoadMatrix[z, x])
                {
                    roads.Add(new Vector3(x, 0, z));
                    return DetermineRoadType((int)x, (int)z);
                }

                float noise = Mathf.PerlinNoise((float)(x / (float)xSize * 5f) + XOffset, (float)(z / (float)zSize * 5f) + ZOffset) * 10f - 1.5f;

                // Debug.Log("noise:" + noise);
                if (noise > 0.15f || true)
                {
                    return new ChunkCellContainer(ChunkCellType.Grass, Vector3.zero);
                }
                else if (noise > 0.05f)
                {
                    return new ChunkCellContainer(ChunkCellType.Sand, Vector3.zero);
                }
                else
                {
                    return new ChunkCellContainer(ChunkCellType.Water, Vector3.zero);
                }
            }
        }
    }
}