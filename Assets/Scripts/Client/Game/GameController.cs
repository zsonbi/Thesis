using Game.World;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using User;

namespace Game
{
    public class GameController : MonoBehaviour
    {
        [SerializeField]
        private World.GameWorld world;

        [SerializeField]
        private GameObject playerPrefab;

        [SerializeField]
        private int probeSize = 1;

        private GameUI gameUI;

        private Dictionary<long, GameObject> playerVariants = new Dictionary<long, GameObject>();

        [SerializeField]
        public World.GameWorld World
        { get => world; private set => world = value; }

        private float ScoreCounter = 0;

        private CarSpawner carSpawner;

        public int Score { get => Mathf.RoundToInt(ScoreCounter); private set => ScoreCounter = value; }

        private PlayerCar player;

        public Vector3 PlayerPos => player.gameObject.transform.position;

        public bool Running { get; private set; } = false;

        public int Difficulty { get; private set; } = 0;

        public float Coins { get; private set; } = 0;

        private async void Awake()
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
            //   await NewGame();
        }

        private void Start()
        {
            //player = Instantiate(playerPrefab).GetComponent<PlayerCar>();

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
                if (Difficulty < this.ScoreCounter / 20)
                {
                    this.Difficulty = (int)this.ScoreCounter / 20;
                    this.gameUI.ChangeDifficulyDisplay(Difficulty);
                }
            }

            this.ScoreCounter += Time.deltaTime;
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
            this.gameUI.ShowEndGameScreen();
            Running = false;
        }

        public async Task NewGame()
        {
            player = Instantiate(playerVariants[UserData.Instance.Game.OwnedCars[gameUI.SelectedSkinIndex].ShopId], this.transform).GetComponent<PlayerCar>();
            player.Init(this);
            player.DestroyedEvent += PlayerDied;
            this.carSpawner.Reset();
            this.Score = 0;
            await World.CreateNewGame();
            Vector3 baseChunkPos = (await World.GetChunk(GameConfig.CHUNK_COUNT / 2, GameConfig.CHUNK_COUNT / 2)).gameObject.transform.position;
            player.gameObject.transform.position = new Vector3(baseChunkPos.x + GameConfig.CHUNK_SIZE * GameConfig.CHUNK_SCALE * GameConfig.CHUNK_CELL / 2 + 10, baseChunkPos.y + 2, baseChunkPos.z);
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
            for (int i = -(probeSize + 1); i <= probeSize + 1; i++)
            {
                for (int j = -(probeSize + 1); j <= probeSize + 1; j++)
                {
                    if (Mathf.Abs(i) <= probeSize && Mathf.Abs(j) <= probeSize)
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

            for (int currSize = 1; currSize <= probeSize; currSize++)
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