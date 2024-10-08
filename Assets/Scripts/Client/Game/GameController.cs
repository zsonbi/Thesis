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

        private CarSpawner? carSpawner;

        public int Score { get => Mathf.RoundToInt(elapsedTime); private set => elapsedTime = value; }

        public PlayerCar? Player { get; private set; }

        public Vector3 PlayerPos => Player!.gameObject.transform.position;

        public bool Running { get; private set; } = false;

        public int Difficulty { get; private set; } = 0;

        public float Coins { get; private set; } = 0;

        private void Awake()
        {
            var handle = Addressables.LoadAssetsAsync<GameObject>("PlayerVariants", PlayerSkinsLoaded);
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

        private void Start()
        {
            Application.targetFrameRate = 60;
            QualitySettings.vSyncCount = 0;
        }

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

        private void PlayerSkinsLoaded(GameObject playerSkin)
        {
            this.playerVariants.Add(playerSkin.GetComponent<PlayerCar>().SkinId, playerSkin);
        }

        public void IncreaseCoinCount(float amount)
        {
            this.Coins += amount;
        }

        private void PlayerDied(object? sender, EventArgs args)
        {
            if (!Running)
            {
                return;
            }

            this.gameUI.ShowEndGameScreen();
            Running = false;
        }

        public async Task NewGame()
        {
            Player = Instantiate(playerVariants[UserData.Instance.Game.OwnedCars![gameUI.SelectedSkinIndex].ShopId], this.transform).GetComponent<PlayerCar>();
            Player.Init(this);
            Player.DestroyedEvent += PlayerDied;
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

        public async Task LoadAndDespawnChunks(int centerRow, int centerColumn)
        {
            await SpawnNearbyChunks(centerRow, centerColumn);
            DespawnFarAwayChunks(centerRow, centerColumn);
        }

        private void DespawnFarAwayChunks(int row, int col)
        {
            for (int i = -(chunkLoadDistance + 1); i <= chunkLoadDistance + 1; i++)
            {
                for (int j = -(chunkLoadDistance + 1); j <= chunkLoadDistance + 1; j++)
                {
                    if (Mathf.Abs(i) <= chunkLoadDistance && Mathf.Abs(j) <= chunkLoadDistance)
                    {
                        continue;
                    }

                    if (i + row >= 0 && j + col >= 0 && i + row < GameConfig.CHUNK_COUNT && j + col < GameConfig.CHUNK_COUNT)
                    {
                        if (world is not null)
                            world.HideChunk(j + col, i + row);
                    }
                }
            }
        }

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