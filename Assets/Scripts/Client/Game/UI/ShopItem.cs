using System.Collections;
using System.Collections.Generic;
using Thesis_backend.Data_Structures;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

    public void Init(Shop item, Sprite image, bool owned)
    {
        this.ProductName.text = item.ProductName;
        this.ProductImage.sprite = image;
        this.Price.text = item.Cost.ToString();

        this.BuyButton.gameObject.SetActive(!owned);
    }

    public void Buy()
    {
    }

    private void Bought()
    {
    }
}