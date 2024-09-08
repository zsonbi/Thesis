using Game;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameUI : MonoBehaviour
{
    [SerializeField]
    private GameObject gameOverContainer;

    [SerializeField]
    private TMP_Text InfamyGameOverText;


    [SerializeField]
    private TMP_Text InfamyInGameText;

    private GameController gameController;


    private void Update()
    {
        if (gameController.Running)
        {
            this.InfamyInGameText.text = "Infamy: "+gameController.Score;
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
        gameController?.NewGame();
    }

    public void ShowEndGameScreen()
    {
        InfamyGameOverText.text = "Infamy: " + gameController.Score;
        gameOverContainer.SetActive(true);
    }

    public void HideGameOverScreen()
    {
        gameOverContainer.SetActive(false);
        CanDouble = true;
    }

    public void ShowShop()
    {
    }

    public void BackToTasks()
    {
        HideGameOverScreen();
    }

    public void BackToGameMenu()
    {
        HideGameOverScreen();

    }

    public void DoubleScore()
    {

    }
}