using Game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUI : MonoBehaviour
{
    private GameController gameController;

    public void Init(GameController gameController)
    {
        this.gameController = gameController;
    }

    public void NewGame()
    {
        gameController?.NewGame();
    }

    public void ShowEndGameScreen()
    {
    }

    public void ShowShop()
    {
    }

    public void BackToTasks()
    {
    }
}