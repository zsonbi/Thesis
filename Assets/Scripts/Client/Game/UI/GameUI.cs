using Config;
using Game;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using User;
using Utility;

namespace Game
{
    namespace UI
    {
        /// <summary>
        /// Controller for the game's UI
        /// </summary>
        public class GameUI : ThreadSafeMonoBehaviour
        {
            [SerializeField]
            private GameObject gameOverContainer;

            [SerializeField]
            private GameObject ingameContainer;

            [SerializeField]
            private ShopWindow ShopWindow;

            [SerializeField]
            private GameObject mainMenuContainer;

            [SerializeField]
            private Image SkinDisplay;

            [SerializeField]
            private LeaderboardWindow leaderboardWindow;

            [SerializeField]
            private TMP_Text InfamyGameOverText;

            [SerializeField]
            private TMP_Text CoinGameOverText;

            [SerializeField]
            private TMP_Text InfamyInGameText;

            [SerializeField]
            private TMP_Text TaskScoreInGameText;

            [SerializeField]
            private TMP_Text CoinInGameText;

            [SerializeField]
            private StarHandler starHandler;

            [SerializeField]
            private Button DoubleCoinButon;

            [SerializeField]
            private ModalWindow ModalWindow;

            private GameController gameController;

            /// <summary>
            /// Is the coins doubled now
            /// </summary>
            public bool Doubled { get; private set; } = false;

            /// <summary>
            /// The currently selected skin's index
            /// </summary>
            public int SelectedSkinIndex { get; private set; } = 0;

            /// <summary>
            /// Called every frame
            /// </summary>
            private void Update()
            {
                if (gameController.Running)
                {
                    this.InfamyInGameText.text = "Infamy: " + gameController.Score;
                    this.CoinInGameText.text = Mathf.RoundToInt(gameController.Coins).ToString();

                    Keyboard keyboard = Keyboard.current;
                    // Make the powerups useable with keyboard
                    if (keyboard is not null)
                    {
                        InputSystem.Update();
                        if (keyboard.qKey.isPressed && !gameController.Player.Turbo)
                        {
                            BuyTurbo();
                        }
                        if (keyboard.eKey.isPressed && !gameController.Player.Immune)
                        {
                            BuyImmunity();
                        }
                    }
                }
            }

            /// <summary>
            /// Save the game's result
            /// </summary>
            public void SaveGameResult()
            {
                CoroutineRunner.RunCoroutine(Server.SendPatchRequest<Thesis_backend.Data_Structures.Game>(ServerConfig.PATH_TO_ADD_COINS, (int)(Doubled ? gameController.Coins * 2 : gameController.Coins), SavedCoins, onFailedAction: ShowRequestFail));
                CoroutineRunner.RunCoroutine(Server.SendPostRequest<Thesis_backend.Data_Structures.GameScore>(ServerConfig.PATH_FOR_STORE_GAME_SCORE, gameController.Score, onFailedAction: ShowRequestFail));
            }

            /// <summary>
            /// Callback after the coins amount were saved
            /// </summary>
            /// <param name="game">The responded game with the updated coin amount</param>
            private void SavedCoins(Thesis_backend.Data_Structures.Game game)
            {
                UserData.Instance.Game.Currency = game.Currency;
            }

            /// <summary>
            /// Buy the double coins bonus
            /// </summary>
            public void DoubleCoins()
            {
                if (UserData.Instance.CurrentTaskScore >= GameConfig.DOUBLE_COIN_COST)
                {
                    CoroutineRunner.RunCoroutine(Server.SendPatchRequest<Thesis_backend.Data_Structures.User>(ServerConfig.PATH_FOR_DOUBLE_COINS, new WWWForm(), DoubledCoins, onFailedAction: ShowRequestFail));
                }
            }

            /// <summary>
            /// Buy the immunity power up
            /// </summary>
            public void BuyImmunity()
            {
                if (UserData.Instance.CurrentTaskScore >= GameConfig.IMMUNITY_COST)
                {
                    CoroutineRunner.RunCoroutine(Server.SendPatchRequest<Thesis_backend.Data_Structures.User>(ServerConfig.PATH_FOR_BUY_IMMUNITY, new WWWForm(), BoughtImmunity, onFailedAction: (string res) => { ShowRequestFail(res); gameController.Player.CancelImmunity(); }));
                    gameController.Player.ApplyImmunity();
                }
            }

            /// <summary>
            /// Buy the turbo power up
            /// </summary>
            public void BuyTurbo()
            {
                if (UserData.Instance.CurrentTaskScore >= GameConfig.TURBO_COST)
                {
                    CoroutineRunner.RunCoroutine(Server.SendPatchRequest<Thesis_backend.Data_Structures.User>(ServerConfig.PATH_FOR_BUY_TURBO, new WWWForm(), BoughtTurbo, onFailedAction: (string res) => { ShowRequestFail(res); gameController.Player.CancelTurbo(); }));
                    gameController.Player.ApplyTurbo();
                }
            }

            /// <summary>
            /// Callback after the player bought the immunity
            /// </summary>
            /// <param name="user">The user with the updated task score amount</param>
            private void BoughtImmunity(Thesis_backend.Data_Structures.User user)
            {
                UserData.Instance.UpdateTaskScore(user.CurrentTaskScore);
                TaskScoreInGameText.text = UserData.Instance.CurrentTaskScore.ToString();
            }

            /// <summary>
            /// Callback after the player bought the turbo
            /// </summary>
            /// <param name="user">The user with the updated task score amount</param>
            private void BoughtTurbo(Thesis_backend.Data_Structures.User user)
            {
                UserData.Instance.UpdateTaskScore(user.CurrentTaskScore);
                TaskScoreInGameText.text = UserData.Instance.CurrentTaskScore.ToString();
            }

            /// <summary>
            /// Callback after the coins were doubled
            /// </summary>
            /// <param name="user">The user with the updated task score amount</param>
            private void DoubledCoins(Thesis_backend.Data_Structures.User user)
            {
                this.Doubled = true;
                this.DoubleCoinButon.interactable = false;
                CoinGameOverText.text = "Coins: " + gameController.Coins * 2;
                TaskScoreInGameText.text = UserData.Instance.CurrentTaskScore.ToString();
                UserData.Instance.UpdateTaskScore(user.CurrentTaskScore);
            }

            /// <summary>
            /// Show the leaderboard window
            /// </summary>
            public void ShowLeaderboard()
            {
                leaderboardWindow.Show();
            }

            /// <summary>
            /// Rotate the skin display to the left
            /// </summary>
            public void LeftRotateSkin()
            {
                SelectedSkinIndex = (SelectedSkinIndex + UserData.Instance.Game.OwnedCars.Count - 1) % UserData.Instance.Game.OwnedCars.Count;
                SkinDisplay.sprite = ShopWindow.ShopItemSprites[(int)UserData.Instance.Game.OwnedCars[SelectedSkinIndex].ShopId - 1];
            }

            /// <summary>
            /// Rotate the skin display to the right
            /// </summary>
            public void RightRotateSkin()
            {
                SelectedSkinIndex = (SelectedSkinIndex + 1) % UserData.Instance.Game.OwnedCars.Count;
                SkinDisplay.sprite = ShopWindow.ShopItemSprites[(int)UserData.Instance.Game.OwnedCars[SelectedSkinIndex].ShopId - 1];
            }

            /// <summary>
            /// Initializes the game use
            /// </summary>
            /// <param name="gameController">Reference to the game controller</param>
            public void Init(GameController gameController)
            {
                this.gameController = gameController;
            }

            /// <summary>
            /// Start a new game
            /// </summary>
            /// <param name="save">Should it save the gamecontroller's values</param>
            public async void NewGame(bool save = true)
            {
                HideGameOverScreen(save);
                ingameContainer.SetActive(true);
                mainMenuContainer.SetActive(false);
                TaskScoreInGameText.text = UserData.Instance.CurrentTaskScore.ToString();
                await gameController?.NewGame();
            }

            /// <summary>
            /// Show if the request failed on the modal window
            /// </summary>
            /// <param name="content">The content to display</param>
            private void ShowRequestFail(string content)
            {
                ModalWindow.Show("Request fail", content);
            }

            /// <summary>
            /// Change the difficulty and update its display
            /// </summary>
            /// <param name="difficulty">The new difficulty</param>
            public void ChangeDifficulyDisplay(int difficulty)
            {
                this.starHandler.ChangeDifficulty(difficulty);
            }

            /// <summary>
            /// Show the game over screen
            /// </summary>
            public void ShowEndGameScreen()
            {
                InfamyGameOverText.text = "Infamy: " + gameController.Score;
                CoinGameOverText.text = "Coins: " + gameController.Coins;
                ingameContainer.SetActive(false);
                gameOverContainer.SetActive(true);
                Doubled = false;
                this.DoubleCoinButon.interactable = UserData.Instance.CurrentTaskScore >= 1000;
            }

            /// <summary>
            /// Hide the game over screen
            /// </summary>
            /// <param name="saveGameResults">Should it save the score and the currencies</param>
            public void HideGameOverScreen(bool saveGameResults = true)
            {
                gameOverContainer.SetActive(false);
                ingameContainer.SetActive(true);
                if (saveGameResults)
                {
                    SaveGameResult();
                }
            }

            /// <summary>
            /// Display the shop window
            /// </summary>
            public void ShowShopWindow()
            {
                this.ShopWindow.Show();
            }

            /// <summary>
            /// Hides the shop window
            /// </summary>
            public void HideShopWindow()
            {
                this.ShopWindow.Hide();
            }

            /// <summary>
            /// Move back to the tasks
            /// </summary>
            public void BackToTasks()
            {
                HideGameOverScreen(false);
                SceneManager.LoadScene("MainScene", LoadSceneMode.Single);
            }

            /// <summary>
            /// Show the main menu, but don't save the scores
            /// </summary>
            public void BackToGameMenuWithoutSave()
            {
                gameController.Player.Kill();
                HideGameOverScreen(false);
                ingameContainer.SetActive(false);
                mainMenuContainer.SetActive(true);
            }

            /// <summary>
            /// Show the main menu
            /// </summary>
            public void BackToGameMenu()
            {
                HideGameOverScreen();
                ingameContainer.SetActive(false);
                mainMenuContainer.SetActive(true);
            }
        }
    }
}