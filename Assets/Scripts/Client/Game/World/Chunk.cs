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
using Cysharp.Threading.Tasks;
using UnityEngine.LowLevel;
using Utility;

namespace Game
{
    namespace World
    {
        /// <summary>
        /// Stores a single chunk of the world
        /// </summary>
        public class Chunk : ThreadSafeMonoBehaviour
        {
            [Header("Offset of perlin noise on X axis")]
            public float XOffset = 0f;

            [Header("Offset of perlin noise on Z axis")]
            public float ZOffset = 0f;

            /// <summary>
            /// Should it randomize the perlin noise offset
            /// </summary>
            [Header("Randomize Offset")]
            public bool RandomizeOffset = false;

            /// <summary>
            /// Reference to the straight road prefab
            /// </summary>
            [SerializeField]
            public GameObject straightRoadPrefab;

            /// <summary>
            /// Reference to the curved road prefab
            /// </summary>
            [SerializeField]
            private GameObject curvedRoadPrefab;

            /// <summary>
            /// Reference to the three way road prefab
            /// </summary>
            [SerializeField]
            private GameObject threeWayRoadPrefab;

            /// <summary>
            /// Reference to the cross roads prefab (4 way)
            /// </summary>
            [SerializeField]
            private GameObject crossRoadsPrefab;

            /// <summary>
            /// Reference to the grass prefab
            /// </summary>
            [SerializeField]
            public GameObject grassPrefab;

            /// <summary>
            /// Reference to the coin prefab
            /// </summary>
            [SerializeField]
            public GameObject coinPrefab;

            /// <summary>
            /// The buildings which can be spawn
            /// </summary>
            private List<Building> buildings;

            /// <summary>
            /// The probs which can be spawn (objects which are away from the road)
            /// </summary>
            private List<Prop> props;

            /// <summary>
            /// The size of the world on the z axis
            /// </summary>
            public int? zSize { get; private set; }

            /// <summary>
            /// The size of the world on the x axis
            /// </summary>
            public int? xSize { get; private set; }

            /// <summary>
            /// Row index of the chunk in the chunk matrix
            /// </summary>
            public int Row { get; private set; }

            /// <summary>
            /// Col index of the chunk in the chunk matrix
            /// </summary>
            public int Col { get; private set; }

            /// <summary>
            /// Matrix which stores the roads
            /// </summary>
            public bool[,] Roads => roadGenerator.RoadMatrix;

            /// <summary>
            /// Reference to the road generator's edge roads
            /// </summary>
            public List<EdgeRoadContainer> EdgeRoads { get => roadGenerator?.EdgeRoads; }

            /// <summary>
            /// The cells of tthe chunk categorized
            /// </summary>
            private Dictionary<ChunkCellType, List<Vector3>> chunkCells = new Dictionary<ChunkCellType, List<Vector3>>();

            /// <summary>
            /// Class which generates the road matrix
            /// </summary>
            private RoadGenerator roadGenerator;

            /// <summary>
            /// Reference to the parent world
            /// </summary>
            private GameWorld world;

            /// <summary>
            /// The road positions
            /// </summary>
            private List<Vector3Int> roads = new List<Vector3Int>();

            /// <summary>
            /// Mostly deprecated but can be used to combine the cell's meshes into a one big mesh
            /// </summary>
            private Dictionary<ChunkCellType, List<GameObject>> objectsToCombine;

            /// <summary>
            /// Matrix to help the building placements stores road direction and if it is buildable
            /// </summary>
            private BuildingCell[,] buildingCells;

            /// <summary>
            /// For single threaded tasks
            /// </summary>
            [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
            public static void InitUniTaskLoop()
            {
                var loop = PlayerLoop.GetCurrentPlayerLoop();
                Cysharp.Threading.Tasks.PlayerLoopHelper.Initialize(ref loop);
            }

            /// <summary>
            /// Start when the script is loaded
            /// </summary>
            private async void Awake()
            {
                //Load the addressables
                buildings = new List<Building>();
                var buildingsLoadHandle = Addressables.LoadAssetsAsync<GameObject>("Buildings", BuildingLoaded);
                await buildingsLoadHandle.Task;

                props = new List<Prop>();
                var propsLoadHandle = Addressables.LoadAssetsAsync<GameObject>("Props", PropLoaded);
                await propsLoadHandle.Task;

                //Load the values from the settings
                LoadFromSettings();
                //Randomizes the offset
                if (RandomizeOffset)
                {
                    this.XOffset = Random.Range(0, 99999);
                    this.ZOffset = Random.Range(0, 99999);
                }

                this.objectsToCombine = new Dictionary<ChunkCellType, List<GameObject>>();
            }

            /// <summary>
            /// Callback for when each building asset is loaded
            /// </summary>
            /// <param name="building">Loaded building</param>
            private void BuildingLoaded(GameObject building)
            {
                buildings.Add(building.GetComponent<Building>());
                buildings.Last().SetAddressableKey($"Buildings/{building.name}.prefab");
            }

            /// <summary>
            /// Callback for when each prop asset is loaded
            /// </summary>
            /// <param name="prop">Loaded prop</param>
            private void PropLoaded(GameObject prop)
            {
                props.Add(prop.GetComponent<Prop>());
                props.Last().SetAddressableKey($"Props/{prop.name}.prefab");
            }

            /// <summary>
            /// Get the chunk's absolute position in the world
            /// </summary>
            /// <returns>The chunk's absolute position in a tuple</returns>
            private (float, float) GetAbsolutePosition()
            {
                return (Col * GameConfig.CHUNK_SCALE * GameConfig.CHUNK_CELL * GameConfig.CHUNK_SIZE, Row * GameConfig.CHUNK_SCALE * GameConfig.CHUNK_CELL * GameConfig.CHUNK_SIZE);
            }

            /// <summary>
            /// Show the chunk
            /// </summary>
            public void Display()
            {
                this.gameObject.SetActive(true);
            }

            /// <summary>
            /// Initialize the chunk
            /// </summary>
            /// <param name="col">The column offset</param>
            /// <param name="row">The row offset</param>
            /// <param name="edgeRoads"></param>
            /// <param name="world"></param>
            /// <returns></returns>
            public async Task InitChunk(int col, int row, List<EdgeRoadContainer> edgeRoads, GameWorld world)
            {
                //Wait for the addressables to be loaded
                while (objectsToCombine is null)
                {
#if UNITY_WEBGL
                    await UniTask.Delay(System.TimeSpan.FromSeconds(0.001f), ignoreTimeScale: false);
#else
                    await Task.Delay(1);
#endif
                }
                this.world = world;
                this.Row = row;
                this.Col = col;
                //Add every type of chunk cell to the dictionary
                for (ChunkCellType i = 0; i <= ChunkCellType.StraightRoad; i++)
                {
                    chunkCells.Add(i, new List<Vector3>());
                }

                //Detroyed check
                if (Destroyed)
                {
                    return;
                }

                //Get the nearby chunks and generate the road matrix
                Chunk[,] nearbyChunks = await GetNearbyChunks(Row, Col);
                roadGenerator = new RoadGenerator(GameConfig.CHUNK_SIZE, edgeRoads, nearbyChunks);

                //Create the tiles
                CreateTiles();

                //Deprecated
                //Create the props
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
                //Destroyed check
                if (Destroyed)
                {
                    return;
                }
                this.gameObject.transform.localPosition = new Vector3(col * GameConfig.CHUNK_SIZE * GameConfig.CHUNK_SCALE * GameConfig.CHUNK_CELL, 0, row * GameConfig.CHUNK_SIZE * GameConfig.CHUNK_SCALE * GameConfig.CHUNK_CELL);
                this.gameObject.transform.localScale = new Vector3(GameConfig.CHUNK_SCALE, 1, GameConfig.CHUNK_SCALE);
                //Generate the obstacles
                await SpawnWorldObjects();
            }

            /// <summary>
            /// Hide the chunk
            /// </summary>
            public void HideChunk()
            {
                this.gameObject.SetActive(false);
            }

            /// <summary>
            /// Gets a random road's position
            /// </summary>
            /// <returns></returns>
            public Vector3 GetARandomRoad()
            {
                if (roads.Count == 0)
                {
                    return new Vector3(GameConfig.CHUNK_SIZE / 2, 0, GameConfig.CHUNK_SIZE / 2);
                }
                return roads[Random.Range(0, roads.Count)];
            }

            /// <summary>
            /// Load the chunk's sizes
            /// </summary>
            private void LoadFromSettings()
            {
                this.xSize = GameConfig.CHUNK_SIZE;
                this.zSize = GameConfig.CHUNK_SIZE;
            }

            /// <summary>
            /// Creates the tiles for the chunk
            /// </summary>
            private void CreateTiles()
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
                        //Load tthe correct prefab
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
                        //Move it to the proper position and store it
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

            /// <summary>
            /// Determine what type of road it is
            /// </summary>
            /// <param name="col">Column index</param>
            /// <param name="row">Row index</param>
            /// <returns>The chunk cell container with the correct road type</returns>
            private ChunkCellContainer DetermineRoadType(int col, int row)
            {
                int roadCount = 0;

                int firstIndex = -1;
                int lastIndex = -1;
                int secondIndex = -1;

                if (col - 1 >= 0 && roadGenerator.RoadMatrix[row, col - 1])
                {
                    roadCount++;
                    firstIndex = 0;
                    lastIndex = 0;
                }

                if (row + 1 < GameConfig.CHUNK_SIZE && roadGenerator.RoadMatrix[row + 1, col])
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

                if (col + 1 < GameConfig.CHUNK_SIZE && roadGenerator.RoadMatrix[row, col + 1])
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

                if (row - 1 >= 0 && roadGenerator.RoadMatrix[row - 1, col])
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
                //If later we want other types of biomes
                else if (noise > 0.05f)
                {
                    return new ChunkCellContainer(ChunkCellType.Sand, Vector3.zero);
                }
                else
                {
                    return new ChunkCellContainer(ChunkCellType.Water, Vector3.zero);
                }
            }

            /// <summary>
            /// Spawns the buildings and the props on a seperate thread
            /// </summary>
            public async Task SpawnWorldObjects()
            {
                buildingCells = new BuildingCell[GameConfig.CHUNK_SIZE, GameConfig.CHUNK_SIZE];
#if UNITY_WEBGL
                GenerateCellMatrix();
#else
                await Task.Run(() => GenerateCellMatrix());
#endif
                CoroutineRunner.RunCoroutine(AddRoadBuildings());
                CoroutineRunner.RunCoroutine(AddEnvironmentProps());
            }

            /// <summary>
            /// Generate the building cells matrix so we know which way the roads are
            /// </summary>
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

            /// <summary>
            /// Add the buildings which are next to the roads
            /// </summary>
            private IEnumerator AddRoadBuildings()
            {
                for (int i = 1; i < GameConfig.CHUNK_SIZE - 1; i++)
                {
                    for (int j = 1; j < GameConfig.CHUNK_SIZE - 1; j++)
                    {
                        //If the scene is destroyed
                        try
                        {
                            LockAndTryToPlaceBuilding(UnityEngine.Random.Range(0, this.buildings.Count), i, j);
                        }
                        catch (MissingReferenceException)
                        {
                        }
                        catch (System.NullReferenceException)
                        {
                        }
                    }
                    yield return null;
                }
                yield return null;
            }

            private IEnumerator AddEnvironmentProps()
            {
                for (int i = 1; i < GameConfig.CHUNK_SIZE - 1; i++)
                {
                    for (int j = 1; j < GameConfig.CHUNK_SIZE - 1; j++)
                    {
                        //If the scene is destroyed
                        try
                        {
                            if (buildingCells[i, j].GotRoadNext || !buildingCells[i, j].Buildable || Random.Range(0, 5) != 0)
                            {
                                continue;
                            }

                            if (Random.Range(0, 2) == 0)
                            {
                                Prop prop = props[Random.Range(0, props.Count)];
                                var absolutePosition = GetAbsolutePosition();
                                Vector3 postition = new Vector3(absolutePosition.Item1 + j * GameConfig.CHUNK_SCALE * GameConfig.CHUNK_CELL, 0, absolutePosition.Item2 + i * GameConfig.CHUNK_SCALE * GameConfig.CHUNK_CELL);
                                Addressables.InstantiateAsync(prop.AddressableKey, postition, Quaternion.Euler(0, Random.Range(0, 4) * 90f, 0), this.gameObject.transform);
                            }
                        }
                        catch (MissingReferenceException)
                        {
                        }
                        catch (System.NullReferenceException)
                        {
                        }
                        yield return null;
                    }
                }
                yield return null;
            }

            /// <summary>
            /// Lock the cell and try to place it
            /// </summary>
            /// <param name="index">The building index</param>
            /// <param name="row">The row position</param>
            /// <param name="col">The column position</param>
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

                    if (Random.Range(0, 2) == 0)
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

            /// <summary>
            /// Gets tthe nearby chunks
            /// </summary>
            /// <param name="row">The row index of the center chunk</param>
            /// <param name="col">The col index of the center chunk</param>
            /// <returns>3*3 matrix of chunks</returns>
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