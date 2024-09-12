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
    private GameObject shopContainer;

    [SerializeField]
    private GameObject mainMenuContainer;

    [SerializeField]
    private TMP_Text InfamyGameOverText;

    [SerializeField]
    private TMP_Text InfamyInGameText;

    private GameController gameController;

    private void Update()
    {
        if (gameController.Running)
        {
            this.InfamyInGameText.text = "Infamy: " + gameController.Score;
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

    public void ShowEndGameScreen()
    {
        InfamyGameOverText.text = "Infamy: " + gameController.Score;
        ingameContainer.SetActive(false);
        gameOverContainer.SetActive(true);
    }

    public void HideGameOverScreen()
    {
        gameOverContainer.SetActive(false);
        ingameContainer.SetActive(true);
        CanDouble = true;
    }

    public void ShowShop()
    {
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