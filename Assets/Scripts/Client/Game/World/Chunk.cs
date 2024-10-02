using UnityEngine;
using System.Collections.Generic;
using DataTypes;
using Assets.Scripts.Client.DataTypes;
using System.Linq;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement;
using System.Threading.Tasks;
using System.Collections;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEditor;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine.LowLevel;

namespace Game
{
    namespace World
    {
        /// <summary>
        /// Used for generating a new world
        /// </summary>
        public class Chunk : MonoBehaviour
        {
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
            public GameObject coinPrefab;

            private List<Building> buildings;

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

            public bool[,] Roads => roadGenerator.RoadMatrix;
            public bool Destroyed { get; private set; } = false;

            public List<EdgeRoadContainer> EdgeRoads { get => roadGenerator?.EdgeRoads; }

            private Dictionary<ChunkCellType, List<Vector3>> chunkCells = new Dictionary<ChunkCellType, List<Vector3>>();
            private RoadGenerator roadGenerator;
            private GameWorld world;
            private List<Vector3Int> roads = new List<Vector3Int>();
            private Dictionary<ChunkCellType, List<GameObject>> objectsToCombine;
            private BuildingCell[,] buildingCells;

            [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
            public static void InitUniTaskLoop()
            {
                var loop = PlayerLoop.GetCurrentPlayerLoop();
                Cysharp.Threading.Tasks.PlayerLoopHelper.Initialize(ref loop);
            }

            private void OnDestroy()
            {
                Destroyed = true;
            }

            // Start is called before the first frame update
            private async void Awake()
            {
                buildings = new List<Building>();
                //PlayerLoopHelper.Initialize();
                var handle = Addressables.LoadAssetsAsync<GameObject>("Buildings", BuildingLoaded);

                await handle.Task;

                //Load the values from the settings
                LoadFromSettings();
                //Randomizes the offset
                if (RandomizeOffset)
                {
                    this.XOffset = Random.Range(0, 99999);
                    this.ZOffset = Random.Range(0, 99999);
                }

                this.objectsToCombine = new Dictionary<ChunkCellType, List<GameObject>>();
                Debug.Log("World buildings");
            }

            // Callback for when each asset is loaded
            private void BuildingLoaded(GameObject building)
            {
                buildings.Add(building.GetComponent<Building>());
                buildings.Last().SetAddressableKey($"Buildings/{building.name}.prefab");
            }

            private (float, float) GetAbsolutePosition()
            {
                return (Col * GameConfig.CHUNK_SCALE * GameConfig.CHUNK_CELL * GameConfig.CHUNK_SIZE, Row * GameConfig.CHUNK_SCALE * GameConfig.CHUNK_CELL * GameConfig.CHUNK_SIZE);
            }

            public void Display()
            {
                this.gameObject.SetActive(true);
            }

            public async Task InitChunk(int xOffset, int zOffset, List<EdgeRoadContainer> edgeRoads, GameWorld world)
            {
                while (objectsToCombine is null)
                {
#if UNITY_WEBGL
                    await UniTask.Delay(System.TimeSpan.FromSeconds(0.001f), ignoreTimeScale: false);
#else
                    await Task.Delay(1);
#endif
                }
                this.world = world;
                this.Row = zOffset;
                this.Col = xOffset;
                for (ChunkCellType i = 0; i <= ChunkCellType.StraightRoad; i++)
                {
                    chunkCells.Add(i, new List<Vector3>());
                }

                if (Destroyed)
                {
                    return;
                }

                Chunk[,] nearbyChunks = await GetNearbyChunks(Row, Col);
                roadGenerator = new RoadGenerator(GameConfig.CHUNK_SIZE, edgeRoads, nearbyChunks);

                //Create the tiles
                CreateTiles();

                ////Create the props
                //AddEnviromentObjects();
                //Combine the objects
                foreach (var item in objectsToCombine)
                {
                    if (item.Value.Count != 0)
                    {
                        //   CombineMeshes(item.Key);
                    }
                }

                //CreateMeshes();
                if (Destroyed)
                {
                    return;
                }
                this.gameObject.transform.localPosition = new Vector3(xOffset * GameConfig.CHUNK_SIZE * GameConfig.CHUNK_SCALE * GameConfig.CHUNK_CELL, 0, zOffset * GameConfig.CHUNK_SIZE * GameConfig.CHUNK_SCALE * GameConfig.CHUNK_CELL);

                this.gameObject.transform.localScale = new Vector3(GameConfig.CHUNK_SCALE, 1, GameConfig.CHUNK_SCALE);
                await GenerateBuildings();
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

            //--------------------------------------------------------------
            /// <summary>
            /// Combines the gameobjects into meshes and cleares out that index of the objectsToCombine
            /// </summary>
            /// <param name="cellType">the index of the objectsToCombine we want to combine</param>
            private void CombineMeshes(ChunkCellType cellType)
            {
                MeshRenderer[] filters = objectsToCombine[cellType][0].GetComponentsInChildren<MeshRenderer>();
                int itr = 0;
                foreach (MeshRenderer filter in filters)
                {
                    //Creates the parent which will have the combined mesh
                    GameObject parent = new GameObject(objectsToCombine[cellType][0].name.Replace("(Clone)", "") + "Mesh", typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider));
                    MeshFilter meshFilter = parent.GetComponent<MeshFilter>();
                    //Assign the material of the first gameobject in the list to the parent
                    parent.GetComponent<MeshRenderer>().material = filter.material;
                    parent.transform.parent = this.transform;

                    meshFilter.mesh = new Mesh();
                    //Makes so that really big meshes are supported also
                    meshFilter.mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
                    CombineInstance[] combine = new CombineInstance[objectsToCombine[cellType].Count];
                    for (int i = 0; i < objectsToCombine[cellType].Count; i++)
                    {
                        MeshFilter[] objectMeshFilters = objectsToCombine[cellType][i].GetComponentsInChildren<MeshFilter>();
                        combine[i].mesh = objectMeshFilters[itr].sharedMesh;
                        combine[i].transform = objectMeshFilters[itr].transform.localToWorldMatrix;
                    }
                    meshFilter.mesh.CombineMeshes(combine, true, true);
                    meshFilter.gameObject.SetActive(true);

                    MeshCollider collider = parent.GetComponent<MeshCollider>();
                    collider.sharedMesh = meshFilter.sharedMesh;
                    parent.isStatic = true;

                    itr++;
                }

                //Empties the list and deletes the no longer used gameobjects
                do
                {
                    Destroy(objectsToCombine[cellType][0]);
                    objectsToCombine[cellType].RemoveAt(0);
                } while (objectsToCombine[cellType].Count > 0);
            }

            private void CreateMeshes()
            {
                foreach (var item in chunkCells)
                {
                    GameObject parent = new GameObject(item.Key.ToString() + "Mesh", typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider));
                    parent.transform.parent = this.transform;
                    MeshFilter meshFilter = parent.GetComponent<MeshFilter>();
                    //parent.GetComponent<MeshRenderer>().material = TileMaterials[(int)item.Key];
                    //parent.GetComponent<MeshCollider>().sharedMaterial = PhysicsMaterials[(int)(item.Key)];
                    meshFilter.mesh = MeshGenerator.CreateMultiShape(item.Value);
                    parent.GetComponent<MeshCollider>().sharedMesh = meshFilter.mesh;
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
            private async void CreateTiles()
            {
                for (int x = 0; x < xSize; x++)
                {
                    for (int z = 0; z < zSize; z++)
                    {
                        if (Destroyed)
                        {
                            return;
                        }

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
                        if (roadGenerator.RoadMatrix[z, x] && Random.Range(0f, 1f) <= GameConfig.COIN_RATE)
                        {
                            Coin coin = Instantiate(coinPrefab, created.transform).GetComponent<Coin>();
                            coin.gameObject.transform.position = new Vector3(x * 32, created.transform.localPosition.y + 3, z * 32);
                        }

                        if (!objectsToCombine.ContainsKey(tileType.Type))
                        {
                            objectsToCombine.Add(tileType.Type, new List<GameObject>());
                        }
                        created.isStatic = true;
                        objectsToCombine[tileType.Type].Add(created);
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
                    roads.Add(new Vector3Int(x, 0, z));
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

            public async Task GenerateBuildings()
            {
                buildingCells = new BuildingCell[GameConfig.CHUNK_SIZE, GameConfig.CHUNK_SIZE];
#if UNITY_WEBGL
                GenerateCellMatrix();
#else
                await Task.Run(() => GenerateCellMatrix());
#endif
                await AddLargeRoadBuildings();
            }

            private void GenerateCellMatrix()
            {
                foreach (var road in roads)
                {
                    buildingCells[road.z, road.x] = new BuildingCell(BuildingDirection.None, false);

                    for (var i = road.z - 1; i <= road.z + 1; i += 2)
                    {
                        if (i < 0 || i >= GameConfig.CHUNK_SIZE || buildingCells[i, road.x] != null)
                        {
                            continue;
                        }
                        if (roadGenerator.RoadMatrix[i, road.x])
                        {
                            buildingCells[i, road.x] = new BuildingCell(BuildingDirection.None, false);
                        }
                        else
                        {
                            BuildingDirection roadDir = BuildingDirection.None;
                            if (i > road.z)
                            {
                                roadDir = BuildingDirection.Down;
                            }
                            else if (i < road.z)
                            {
                                roadDir = BuildingDirection.Up;
                            }

                            buildingCells[i, road.x] = new BuildingCell(roadDir, true);
                        }
                    }

                    for (int j = road.x - 1; j <= road.x + 1; j += 2)
                    {
                        if (j < 0 || j >= GameConfig.CHUNK_SIZE || buildingCells[road.z, j] != null)
                        {
                            continue;
                        }
                        if (roadGenerator.RoadMatrix[road.z, j])
                        {
                            buildingCells[road.z, j] = new BuildingCell(BuildingDirection.None, false);
                        }
                        else
                        {
                            BuildingDirection roadDir = BuildingDirection.None;

                            if (j > road.x)
                            {
                                roadDir = BuildingDirection.Left;
                            }
                            else if (j < road.x)
                            {
                                roadDir = BuildingDirection.Right;
                            }

                            buildingCells[road.z, j] = new BuildingCell(roadDir, true);
                        }
                    }
                }

                for (int i = 0; i < GameConfig.CHUNK_SIZE; i++)
                {
                    for (int j = 0; j < GameConfig.CHUNK_SIZE; j++)
                    {
                        if (buildingCells[i, j] is null)
                        {
                            buildingCells[i, j] = new BuildingCell(BuildingDirection.None, true);
                        }
                    }
                }
            }

            private async Task AddLargeRoadBuildings()
            {
                for (int i = 1; i < GameConfig.CHUNK_SIZE - 1; i++)
                {
                    for (int j = 1; j < GameConfig.CHUNK_SIZE - 1; j++)
                    {
                        LockAndTryToPlaceBuilding(Random.Range(0, this.buildings.Count), i, j);
                    }
                }
            }

            private void LockAndTryToPlaceBuilding(int index, int row, int col)
            {
                if (buildingCells[row, col].GotRoadNext && buildingCells[row, col].Buildable)
                {
                    Building building = this.buildings[index];
                    List<BuildingCell> neighbourBuildingCells = new List<BuildingCell>();
                    float zPosCorrection = 0f;
                    float xPosCorrection = 0f;
                    if (((int)buildingCells[row, col].RoadDirection) % 2 == 1)
                    {
                        for (int i = building.RowCount / -2; i < Mathf.CeilToInt(building.RowCount / 2f); i++)
                        {
                            if (!buildingCells[row, col + i].Buildable)
                            {
                                return;
                            }
                            neighbourBuildingCells.Add(buildingCells[row, col + i]);
                        }
                        if (building.RowCount % 2 == 0)
                        {
                            xPosCorrection = GameConfig.CHUNK_SIZE / -2;
                        }
                        if (building.ColumnCount % 2 == 0)
                        {
                            zPosCorrection = GameConfig.CHUNK_SIZE / 2;
                        }
                    }
                    else
                    {
                        for (int i = building.RowCount / -2; i < Mathf.CeilToInt(building.RowCount / 2f); i++)
                        {
                            if (!buildingCells[row + i, col].Buildable)
                            {
                                return;
                            }
                            neighbourBuildingCells.Add(buildingCells[row + i, col]);
                        }

                        if (building.RowCount % 2 == 0)
                        {
                            zPosCorrection = GameConfig.CHUNK_SIZE / -2;
                        }
                        if (building.ColumnCount % 2 == 0)
                        {
                            xPosCorrection = GameConfig.CHUNK_SIZE / 2;
                        }
                    }

                    if (Random.Range(0, 3) == 0)
                    {
                        float rotation = ((int)buildingCells[row, col].RoadDirection) % 4 * 90f;
                        var absolutePosition = GetAbsolutePosition();
                        Vector3 postition = new Vector3(absolutePosition.Item1 + col * GameConfig.CHUNK_SCALE * GameConfig.CHUNK_CELL + xPosCorrection, 0, absolutePosition.Item2 + row * GameConfig.CHUNK_SCALE * GameConfig.CHUNK_CELL + zPosCorrection);

                        Addressables.InstantiateAsync(building.AddressableKey, postition, Quaternion.Euler(0, rotation, 0), this.gameObject.transform);

                        foreach (var item in neighbourBuildingCells)
                        {
                            item.Occupy();
                        }
                    }
                }
            }

            private async Task<Chunk[,]> GetNearbyChunks(int row, int col)
            {
                Chunk[,] nearbyChunks = new Chunk[3, 3];

                for (int i = -1; i < 2; i++)
                {
                    if (row + i < 0 || row + i >= GameConfig.CHUNK_COUNT || i == 0)
                    {
                        continue;
                    }
                    for (int j = -1; j < 2; j++)
                    {
                        if (col + j < 0 || col + j >= GameConfig.CHUNK_COUNT || j == 0)
                        {
                            continue;
                        }

                        nearbyChunks[i + 1, j + 1] = world.GetChunkWithoutLoad(col + j, row + i);
                        if (nearbyChunks[i + 1, j + 1] is not null)
                        {
                            if (nearbyChunks[i + 1, j + 1].roadGenerator is null)
                            {
                                return nearbyChunks;
                            }
                            while (nearbyChunks[i + 1, j + 1].Roads is null)
                            {
                                await UniTask.Delay(System.TimeSpan.FromSeconds(0.001f), ignoreTimeScale: false);
                            }
                        }
                    }
                }
                return nearbyChunks;
            }
        }
    }
}