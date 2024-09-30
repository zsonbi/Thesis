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
    public List<Sprite> ShopItemSprites = new List<Sprite>();

    [SerializeField]
    private TMP_Text CoinText;

    [SerializeField]
    private ModalWindow ModalWindow;

    private List<Shop> shopItemsCache;

    public void Show()
    {
        this.gameObject.SetActive(true);

        UpdateShop();
    }

    public void Hide()
    {
        this.gameObject.SetActive(false);
    }

    private void ShowRequestFail(string content)
    {
        ModalWindow.Show("Shop error", content);
    }

    public void UpdateShop()
    {
        if (shopItemsCache is null)
        {
            StartCoroutine(Server.SendGetRequest<List<Shop>>(ServerConfig.PATH_FOR_SHOP_GET_ALL, LoadedShopItems, onFailedAction: ShowRequestFail));
        }
        else
        {
            LoadedShopItems(shopItemsCache);
        }
        CoinText.text = UserData.Instance.Game.Currency.ToString();
    }

    private void LoadedShopItems(List<Shop> shopItems)
    {
        //Delete the previous ones
        for (int i = 0; i < this.ShopParent.transform.childCount; i++)
        {
            Destroy(this.ShopParent.transform.GetChild(i).gameObject);
        }
        this.ShopParent.transform.DetachChildren();

        this.shopItemsCache = shopItems;
        foreach (var item in shopItems)
        {
            ShopItem shopItem = Instantiate(ShopItemPrefab, ShopParent.transform).GetComponent<ShopItem>();
            bool owned = UserData.Instance.Game.OwnedCars.Find(x => x.ShopId == item.ID) != null;
            if (ShopItemSprites.Count <= (item.ID - 1))
            {
                shopItem.Init(this, item, ShopItemSprites[0], owned);
                Debug.LogError("No sprite for this shop item");
            }
            else
            {
                shopItem.Init(this, item, ShopItemSprites[(int)(item.ID - 1)], owned);
            }
        }
    }

    #region TestGettters

    public string GetCoinText()
    {
        return this.CoinText.text;
    }

    public GameObject GetShopParent()
    {
        return this.ShopParent;
    }

    #endregion TestGettters
}