using Config;
using System.Collections.Generic;
using Thesis_backend.Data_Structures;
using TMPro;
using UnityEngine;
using User;

public class ShopWindow : MonoBehaviour
{
    [SerializeField]
    private GameObject ShopParent;

    [SerializeField]
    private GameObject ShopItemPrefab;

    [SerializeField]
    private List<Sprite> ShopItemSprites = new List<Sprite>();

    [SerializeField]
    private TMP_Text CoinText;

    private void Start()
    {
        StartCoroutine(Server.SendGetRequest<List<Shop>>(ServerConfig.PATH_FOR_SHOP_GET_ALL, LoadedShopItems));
        CoinText.text = UserData.Instance.Game.Currency.ToString();
    }

    private void LoadedShopItems(List<Shop> shopItems)
    {
        foreach (var item in shopItems)
        {
            ShopItem shopItem = Instantiate(ShopItemPrefab, ShopParent.transform).GetComponent<ShopItem>();
            if (ShopItemSprites.Count <= (item.ID - 1))
            {
                shopItem.Init(item, ShopItemSprites[0], false);
                Debug.LogError("No sprite for this shop item");
            }
            else
            {
                shopItem.Init(item, ShopItemSprites[(int)(item.ID - 1)], false);
            }
        }
    }
}