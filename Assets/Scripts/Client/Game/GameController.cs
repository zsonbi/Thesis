#nullable enable

using Game.World;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using User;

namespace Game
{
    /// <summary>
    /// Handles the main flow of the game scene
    /// </summary>
    public class GameController : ThreadSafeMonoBehaviour
    {
        /// <summary>
        /// Reference to the generated world
        /// </summary>
        [SerializeField]
        private World.GameWorld world;

        /// <summary>
        /// How far should the chunks be loaded
        /// </summary>
        [SerializeField]
        private int chunkLoadDistance = 1;

        /// <summary>
        /// Reference to the game ui controller
        /// </summary>
        private GameUI gameUI;

        /// <summary>
        /// The possible skins that can be used
        /// </summary>
        private Dictionary<long, GameObject> playerVariants = new Dictionary<long, GameObject>();

        /// <summary>
        /// Gets the world
        /// </summary>
        public World.GameWorld World

        { get => world; private set => world = value; }

        /// <summary>
        /// Stores how long the game had been in progress (score is rounded from this)
        /// </summary>
        private float elapsedTime = 0;

        /// <summary>
        /// Reference to the gameobject which will handle the spawning of cars
        /// </summary>
        private CarSpawner? carSpawner;

        /// <summary>
        /// Getter to the score
        /// </summary>
        public int Score { get => Mathf.RoundToInt(elapsedTime); private set => elapsedTime = value; }

        /// <summary>
        /// Reference to the player's car
        /// </summary>
        public PlayerCar? Player { get; private set; }

        /// <summary>
        /// Getter to the player's current absolute position in the world
        /// </summary>
        public Vector3 PlayerPos => Player!.gameObject.transform.position;

        /// <summary>
        /// Is the game running
        /// </summary>
        public bool Running { get; private set; } = false;

        /// <summary>
        /// The current difficulty of the game
        /// </summary>
        public int Difficulty { get; private set; } = 0;

        /// <summary>
        /// The number of picked up coins
        /// </summary>
        public float Coins { get; private set; } = 0;

        /// <summary>
        /// Called when script is loaded
        /// </summary>
        private void Awake()
        {
            //Load the skins
            var handle = Addressables.LoadAssetsAsync<GameObject>("PlayerVariants", PlayerSkinsLoaded);
            //Get the reference to the other gameobjects
            this.carSpawner = this.GetComponentInChildren<CarSpawner>();
            gameUI = this.gameObject.GetComponentInChildren<GameUI>();
            if (gameUI is null)
            {
                Debug.LogError("Can't load the ui");
            }
            else
            {
                gameUI.Init(this);
            }
        }

        /// <summary>
        /// Called just before the first frame
        /// </summary>
        private void Start()
        {
            Application.targetFrameRate = 60;
            QualitySettings.vSyncCount = 0;
        }

        /// <summary>
        /// Called every frame
        /// </summary>
        private void Update()
        {
            if (!Running)
            {
                return;
            }
            if (this.Difficulty < 5)
            {
                if (Difficulty < this.elapsedTime / 20)
                {
                    this.Difficulty = (int)this.elapsedTime / 20;
                    this.gameUI.ChangeDifficulyDisplay(Difficulty);
                }
            }

            this.elapsedTime += Time.deltaTime;
        }

        /// <summary>
        /// Add the loaded skin to the player variants list
        /// </summary>
        /// <param name="playerSkin">The loaded player skin</param>
        private void PlayerSkinsLoaded(GameObject playerSkin)
        {
            this.playerVariants.Add(playerSkin.GetComponent<PlayerCar>().SkinId, playerSkin);
        }

        /// <summary>
        /// Increases the game's collected coin count
        /// </summary>
        /// <param name="amount">How much to increase it</param>
        public void IncreaseCoinCount(float amount)
        {
            this.Coins += amount;
        }

        /// <summary>
        /// Event for when the player is killed
        /// </summary>
        /// <param name="sender">(not used)</param>
        /// <param name="args">(not used)</param>
        private void PlayerDied(object? sender, EventArgs args)
        {
            //If the game is not running skip
            if (!Running)
            {
                return;
            }

            this.gameUI.ShowEndGameScreen();
            Running = false;
        }

        /// <summary>
        /// Create a new game
        /// </summary>
        /// <returns></returns>
        public async Task NewGame()
        {
            //Loads the player
            long skinId = UserData.Instance.Game.OwnedCars![gameUI.SelectedSkinIndex].ShopId;
            //If the skin hasn't loaded yet wait
            for (int i = 0; i < 100 && !playerVariants.ContainsKey(skinId); i++)
            {
                await Task.Delay(100);
            }
            Player = Instantiate(playerVariants[skinId], this.transform).GetComponent<PlayerCar>();
            Player.Init(this);
            Player.DestroyedEvent += PlayerDied;
            //Reset the parameters
            this.carSpawner?.Reset();
            this.Score = 0;
            await World.CreateNewGame();
            Vector3 baseChunkPos = (await World.GetChunk(GameConfig.CHUNK_COUNT / 2, GameConfig.CHUNK_COUNT / 2)).gameObject.transform.position;
            Player.gameObject.transform.position = new Vector3(baseChunkPos.x + GameConfig.CHUNK_SIZE * GameConfig.CHUNK_SCALE * GameConfig.CHUNK_CELL / 2 + 10, baseChunkPos.y + 2, baseChunkPos.z);
            this.gameUI.ChangeDifficulyDisplay(0);
            this.Running = true;
            this.Coins = 0;
            this.Difficulty = 0;
        }

        /// <summary>
        /// Handle the load and despawning of the chunks relative to the given indices
        /// </summary>
        /// <param name="centerRow">Chunk row index</param>
        /// <param name="centerColumn">Chunk col index</param>
        public async Task LoadAndDespawnChunks(int centerRow, int centerColumn)
        {
            await SpawnNearbyChunks(centerRow, centerColumn);
            DespawnFarAwayChunks(centerRow, centerColumn);
        }

        /// <summary>
        /// Despawn the chunks which are too far away from the player
        /// </summary>
        /// <param name="row">The center row index of the chunks</param>
        /// <param name="col">The center col index of the chunks</param>
        private void DespawnFarAwayChunks(int row, int col)
        {
            for (int i = -(chunkLoadDistance + 1); i <= chunkLoadDistance + 1; i++)
            {
                for (int j = -(chunkLoadDistance + 1); j <= chunkLoadDistance + 1; j++)
                {
                    //If the chunk is close enough skip it
                    if (Mathf.Abs(i) <= chunkLoadDistance && Mathf.Abs(j) <= chunkLoadDistance)
                    {
                        continue;
                    }
                    //If the chunk is far away unload it
                    if (i + row >= 0 && j + col >= 0 && i + row < GameConfig.CHUNK_COUNT && j + col < GameConfig.CHUNK_COUNT)
                    {
                        if (world is not null)
                            world.HideChunk(j + col, i + row);
                    }
                }
            }
        }

        /// <summary>
        /// Spawns and loads the nearby chunks
        /// </summary>
        /// <param name="row">The center row index of the chunks</param>
        /// <param name="col">The center col index of the chunks</param>
        private async Task SpawnNearbyChunks(int row, int col)
        {
            await ValidateAndLoadChunk(row, col);

            for (int currSize = 1; currSize <= chunkLoadDistance; currSize++)
            {
                for (int x = 0 - currSize; x <= 0 + currSize; x += currSize * 2)
                {
                    for (int y = 0; y <= currSize - 1; y++)
                    {
                        await ValidateAndLoadChunk(row + y, col + x);
                        await ValidateAndLoadChunk(row - y, col + x);
                    }
                }

                for (int y = 0 - currSize; y <= 0 + currSize; y += currSize * 2)
                {
                    for (int x = 0; x <= currSize - 1; x++)
                    {
                        await ValidateAndLoadChunk(row + y, col + x);
                        await ValidateAndLoadChunk(row + y, col - x);
                    }
                }

                await ValidateAndLoadChunk(row + currSize, col + currSize);
                await ValidateAndLoadChunk(row - currSize, col - currSize);
                await ValidateAndLoadChunk(row + currSize, col - currSize);
                await ValidateAndLoadChunk(row - currSize, col + currSize);
            }
        }

        /// <summary>
        /// Checks if the selected chunk is in range and loads it if the world exists
        /// </summary>
        /// <param name="row">Row index of the chunk</param>
        /// <param name="col">Col index of the chunk</param>
        private async Task ValidateAndLoadChunk(int row, int col)
        {
            if (row < 0 || row >= GameConfig.CHUNK_COUNT || col < 0 || col >= GameConfig.CHUNK_COUNT)
            {
                return;
            }
            if (world is not null)
                await world.LoadChunk(col, row);
        }
    }
}