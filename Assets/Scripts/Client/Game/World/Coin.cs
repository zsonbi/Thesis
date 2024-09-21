using Game;
using UnityEngine;

public class Coin : MonoBehaviour
{
    private void Update()
    {
        this.gameObject.transform.Rotate(0, 0, Time.deltaTime * 45);
    }

    private void OnTriggerEnter(Collider other)
    {
        this.gameObject.SetActive(false);
        other.gameObject.GetComponentInParent<PlayerCar>().PickedUpCoin();
    }
}