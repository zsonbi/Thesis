using Config;
using Game;
using Thesis_backend.Data_Structures;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using User;

public class GameUI : MonoBehaviour
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
    public bool Doubled { get; private set; } = false;

    public int SelectedSkinIndex { get; private set; } = 0;

    private void Update()
    {
        if (gameController.Running)
        {
            this.InfamyInGameText.text = "Infamy: " + gameController.Score;
            this.CoinInGameText.text = Mathf.RoundToInt(gameController.Coins).ToString();
        }
    }

    private void SaveCoins()
    {
        StartCoroutine(Server.SendPatchRequest<Thesis_backend.Data_Structures.Game>(ServerConfig.PATH_FOR_SAVE_COINS, (int)(Doubled ? gameController.Coins * 2 : gameController.Coins), SavedCoins, onFailedAction: ShowRequestFail));
    }

    private void SavedCoins(Thesis_backend.Data_Structures.Game game)
    {
        UserData.Instance.Game.Currency = game.Currency;
    }

    public void DoubleCoins()
    {
        if (UserData.Instance.CurrentTaskScore >= 1000)
        {
            StartCoroutine(Server.SendPatchRequest<Thesis_backend.Data_Structures.User>(ServerConfig.PATH_FOR_DOUBLE_COINS, new WWWForm(), DoubledCoins, onFailedAction: ShowRequestFail));
        }
    }

    public void BuyImmunity()
    {
        if (UserData.Instance.CurrentTaskScore >= 500)
        {
            StartCoroutine(Server.SendPatchRequest<Thesis_backend.Data_Structures.User>(ServerConfig.PATH_FOR_BUY_IMMUNITY, new WWWForm(), BoughtImmunity, onFailedAction: ShowRequestFail));
        }
    }

    public void BuyTurbo()
    {
        if (UserData.Instance.CurrentTaskScore >= 250)
        {
            StartCoroutine(Server.SendPatchRequest<Thesis_backend.Data_Structures.User>(ServerConfig.PATH_FOR_BUY_TURBO, new WWWForm(), BoughtTurbo, onFailedAction: ShowRequestFail));
        }
    }

    private void BoughtImmunity(Thesis_backend.Data_Structures.User user)
    {
        UserData.Instance.UpdateTaskScore(user.CurrentTaskScore);
        gameController.Player.ApplyImmunity();
        TaskScoreInGameText.text = UserData.Instance.CurrentTaskScore.ToString();
    }

    private void BoughtTurbo(Thesis_backend.Data_Structures.User user)
    {
        UserData.Instance.UpdateTaskScore(user.CurrentTaskScore);
        gameController.Player.ApplyTurbo();
        TaskScoreInGameText.text = UserData.Instance.CurrentTaskScore.ToString();
    }

    private void DoubledCoins(Thesis_backend.Data_Structures.User user)
    {
        this.Doubled = true;
        this.DoubleCoinButon.interactable = false;
        CoinGameOverText.text = "Coins: " + gameController.Coins * 2;
        TaskScoreInGameText.text = UserData.Instance.CurrentTaskScore.ToString();
        UserData.Instance.UpdateTaskScore(user.CurrentTaskScore);
    }

    public void LeftRotateSkin()
    {
        SelectedSkinIndex = (SelectedSkinIndex + UserData.Instance.Game.OwnedCars.Count - 1) % UserData.Instance.Game.OwnedCars.Count;
        SkinDisplay.sprite = ShopWindow.ShopItemSprites[(int)UserData.Instance.Game.OwnedCars[SelectedSkinIndex].ShopId - 1];
        Debug.Log(SelectedSkinIndex);
    }

    public void RightRotateSkin()
    {
        SelectedSkinIndex = (SelectedSkinIndex + 1) % UserData.Instance.Game.OwnedCars.Count;
        SkinDisplay.sprite = ShopWindow.ShopItemSprites[(int)UserData.Instance.Game.OwnedCars[SelectedSkinIndex].ShopId - 1];
    }

    public void Init(GameController gameController)
    {
        this.gameController = gameController;
    }

    public async void NewGame()
    {
        HideGameOverScreen();
        ingameContainer.SetActive(true);
        mainMenuContainer.SetActive(false);
        TaskScoreInGameText.text = UserData.Instance.CurrentTaskScore.ToString();

        await gameController?.NewGame();
    }

    private void ShowRequestFail(string content)
    {
        ModalWindow.Show("Request fail", content);
    }

    public void ChangeDifficulyDisplay(int difficulty)
    {
        this.starHandler.ChangeDifficulty(difficulty);
    }

    public void ShowEndGameScreen()
    {
        InfamyGameOverText.text = "Infamy: " + gameController.Score;
        CoinGameOverText.text = "Coins: " + gameController.Coins;
        ingameContainer.SetActive(false);
        gameOverContainer.SetActive(true);
        Doubled = false;
        this.DoubleCoinButon.interactable = UserData.Instance.CurrentTaskScore >= 1000;
    }

    public void HideGameOverScreen()
    {
        gameOverContainer.SetActive(false);
        ingameContainer.SetActive(true);
        SaveCoins();
    }

    public void ShowShopWindow()
    {
        this.ShopWindow.Show();
    }

    public void HideShopWindow()
    {
        this.ShopWindow.Hide();
    }

    public void BackToTasks()
    {
        HideGameOverScreen();
        SceneManager.LoadScene("MainScene", LoadSceneMode.Single);
    }

    public void BackToGameMenu()
    {
        HideGameOverScreen();
        ingameContainer.SetActive(false);
        mainMenuContainer.SetActive(true);
    }
}