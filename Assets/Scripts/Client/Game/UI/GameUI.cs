using Game;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    private TMP_Text InfamyGameOverText;

    [SerializeField]
    private TMP_Text CoinGameOverText;

    [SerializeField]
    private TMP_Text InfamyInGameText;

    [SerializeField]
    private TMP_Text CoinInGameText;

    [SerializeField]
    private StarHandler starHandler;

    private GameController gameController;

    private void Update()
    {
        if (gameController.Running)
        {
            this.InfamyInGameText.text = "Infamy: " + gameController.Score;
            this.CoinInGameText.text = Mathf.RoundToInt(gameController.Coins).ToString();
        }
    }

    public bool CanDouble { get; private set; } = true;

    public void Init(GameController gameController)
    {
        this.gameController = gameController;
    }

    public void NewGame()
    {
        HideGameOverScreen();
        ingameContainer.SetActive(true);
        mainMenuContainer.SetActive(false);

        gameController?.NewGame();
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
    }

    public void HideGameOverScreen()
    {
        gameOverContainer.SetActive(false);
        ingameContainer.SetActive(true);
        CanDouble = true;
    }

    public void ShowShopWindow()
    {
        this.ShopWindow.gameObject.SetActive(true);
    }

    public void HideShopWindow()
    {
        this.ShopWindow.gameObject.SetActive(false);
    }

    public void BackToTasks()
    {
        HideGameOverScreen();
        SceneManager.LoadScene("MainMobileFrame", LoadSceneMode.Single);
    }

    public void BackToGameMenu()
    {
        HideGameOverScreen();
        ingameContainer.SetActive(false);
        mainMenuContainer.SetActive(true);
    }

    public void DoubleScore()
    {
    }
}