using Game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    private GameController gameController;

    public void Init(GameController gameController)
    {
        this.gameController = gameController;
    }

    private void OnCollisionEnter(Collision collision)
    {
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("coin");
        this.gameObject.SetActive(false);
    }
}