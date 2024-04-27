using UnityEngine;
using System.Collections.Generic;
using DataTypes;

/// <summary>
/// Used for generating a new world
/// </summary>
public class Chunk : MonoBehaviour
{
    [Header("Materials for the tiles")]
    [SerializeField]
    private Material[] TileMaterials;

    [Header("Tile scale")]
    public int TileScale = 1;

    [Header("Offset of perlin noise on X axis")]
    public float XOffset = 0f;

    [Header("Offset of perlin noise on Z axis")]
    public float ZOffset = 0f;

    [Header("Randomize Offset")]
    public bool RandomizeOffset = false;

    //------------------------------------------
    [HideInInspector]
    public List<byte[,]> layers = new List<byte[,]>(); //The layers used for navigation and other stuff

    /// <summary>
    /// The size of the world on the z axis
    /// </summary>
    public int zSize { get; private set; }

    /// <summary>
    /// The size of the world on the x axis
    /// </summary>
    public int xSize { get; private set; }

    //private byte[,] waterLayer { get => layers[0]; } //1 for water tiles
    private byte[,] moveLayer { get => layers[0]; } //1 for passable tiles

    public List<EdgeRoadContainer> EdgeRoads { get => roadGenerator.EdgeRoads; }

    //private byte[,] grassLayer { get => layers[2]; } //1 for grass tiles used for the prop generation
    //private byte[,] plantLayer { get => layers[3]; } //1 where the plants are

    private byte numberOfDifferentObjects;
    private List<List<GameObject>> objectsToCombine;
    private GameObject plantParent;
    private Dictionary<ChunkCellType, List<Vector3>> chunkCells = new Dictionary<ChunkCellType, List<Vector3>>();
    private RoadGenerator roadGenerator;

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

    private void Display()
    {
        this.gameObject.SetActive(true);
    }

    public void InitChunk(int xOffset, int zOffset, List<EdgeRoadContainer> edgeRoads)
    {
        for (ChunkCellType i = 0; i <= ChunkCellType.Road; i++)
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
        CreateMeshes();
        this.gameObject.transform.localPosition = new Vector3(xOffset * GameConfig.CHUNK_SIZE, 0, zOffset * GameConfig.CHUNK_SIZE);
    }

    public void HideChunk()
    {
    }

    private void CreateMeshes()
    {
        foreach (var item in chunkCells)
        {
            GameObject parent = new GameObject(item.Key.ToString() + "Mesh", typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider));
            parent.transform.parent = this.transform;
            MeshFilter meshFilter = parent.GetComponent<MeshFilter>();
            parent.GetComponent<MeshRenderer>().material = TileMaterials[(int)item.Key];

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
    private void CreateTiles()
    {
        for (int x = 0; x < xSize; x++)
        {
            for (int z = 0; z < zSize; z++)
            {
                ChunkCellType tileType = DetermineTileType(x, z);
                chunkCells[tileType].Add(new Vector3(x, 0f, z));
            }
        }
    }

    //--------------------------------------------------------------
    /// <summary>
    /// Combines the gameobjects into meshes and cleares out that index of the objectsToCombine
    /// </summary>
    /// <param name="index">the index of the objectsToCombine we want to combine</param>
    private void CombineMeshes(byte index)
    {
        //Creates the parent which will have the combined mesh
        GameObject parent = new GameObject(objectsToCombine[index][0].name.Replace("(Clone)", "") + "Mesh", typeof(MeshFilter), typeof(MeshRenderer));
        MeshFilter meshFilter = parent.GetComponent<MeshFilter>();
        //Assign the material of the first gameobject in the list to the parent
        //Assign the material of the first gameobject in the list to the parent
        parent.GetComponent<MeshRenderer>().material = objectsToCombine[index][0].GetComponent<MeshRenderer>().material;
        parent.transform.parent = this.transform;

        meshFilter.mesh = new Mesh();
        //Makes so that really big meshes are supported also
        meshFilter.mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        CombineInstance[] combine = new CombineInstance[objectsToCombine[index].Count];
        for (int i = 0; i < objectsToCombine[index].Count; i++)
        {
            MeshFilter objectMeshFilter = objectsToCombine[index][i].GetComponent<MeshFilter>();
            combine[i].mesh = objectMeshFilter.sharedMesh;
            combine[i].transform = objectMeshFilter.transform.localToWorldMatrix;
        }

        //Empties the list and deletes the no longer used gameobjects
        do
        {
            Destroy(objectsToCombine[index][0]);
            objectsToCombine[index].RemoveAt(0);
        } while (objectsToCombine[index].Count > 0);

        meshFilter.mesh.CombineMeshes(combine, true, true);
        meshFilter.gameObject.SetActive(true);
    }

    //-------------------------------------------------------------------------
    /// <summary>
    /// Determines the type of the tile at the coord
    /// </summary>
    /// <param name="x">x coord where we want to sample the perlin noise</param>
    /// <param name="z">z coord where we want to sample the perlin noise</param>
    /// <returns>the proper TileType</returns>
    private ChunkCellType DetermineTileType(int x, int z)
    {
        if (roadGenerator.RoadMatrix[z, x])
        {
            return ChunkCellType.Road;
        }

        float noise = Mathf.PerlinNoise((float)(x / (float)xSize * 5f) + XOffset, (float)(z / (float)zSize * 5f) + ZOffset) * 10f - 1.5f;

        // Debug.Log("noise:" + noise);
        if (noise > 0.15f || true)
        {
            return ChunkCellType.Grass;
        }
        else if (noise > 0.05f)
        {
            return ChunkCellType.Sand;
        }
        else
        {
            return ChunkCellType.Water;
        }
    }
}