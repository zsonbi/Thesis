using Config;
using System.Collections;
using System.Collections.Generic;
using Thesis_backend.Data_Structures;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using User;

public class ShopItem : MonoBehaviour
{
    [SerializeField]
    private TMP_Text ProductName;

    [SerializeField]
    private Image ProductImage;

    [SerializeField]
    private TMP_Text Price;

    [SerializeField]
    private Button BuyButton;

    private ShopWindow shopWindow;

    public long Id { get; private set; }

    public void Init(ShopWindow parentWindow, Shop item, Sprite image, bool owned)
    {
        this.shopWindow = parentWindow;
        this.ProductName.text = item.ProductName;
        this.ProductImage.sprite = image;
        this.Price.text = item.Cost.ToString();
        this.Id = item.ID;
        this.BuyButton.gameObject.SetActive(!owned);
    }

    public void Buy()
    {
        StartCoroutine(Server.SendPatchRequest<Thesis_backend.Data_Structures.Game>(ServerConfig.PATH_FOR_BUY_CAR(Id), new WWWForm(), Bought));
    }

    private void Bought(Thesis_backend.Data_Structures.Game game)
    {
        UserData.Instance.Game.OwnedCars = game.OwnedCars;
        UserData.Instance.Game.Currency = game.Currency;
        shopWindow.UpdateShop();
    }
}