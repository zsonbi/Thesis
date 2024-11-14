using Config;
using System.Collections.Generic;
using Thesis_backend.Data_Structures;
using TMPro;
using UnityEngine;
using User;
using Utility;

namespace Game
{
    namespace UI
    {
        /// <summary>
        /// Handles the shop window's content display
        /// </summary>
        public class ShopWindow : ThreadSafeMonoBehaviour
        {
            /// <summary>
            /// The parent of the shop items
            /// </summary>
            [SerializeField]
            private GameObject ShopParent;

            /// <summary>
            /// A single shop item's prefab
            /// </summary>
            [SerializeField]
            private GameObject ShopItemPrefab;

            /// <summary>
            /// Shop item sprites (needs to be in order with the shop ids)
            /// </summary>
            [SerializeField]
            public List<Sprite> ShopItemSprites = new List<Sprite>();

            /// <summary>
            /// Reference to the coin text display
            /// </summary>
            [SerializeField]
            private TMP_Text CoinText;

            /// <summary>
            /// Reference to the modal window in the scene
            /// </summary>
            [SerializeField]
            private ModalWindow ModalWindow;

            /// <summary>
            /// Cached shop items so we don't have to refresh every time since it is static mostly
            /// </summary>
            private List<Shop> shopItemsCache;

            /// <summary>
            /// Show the shop window
            /// </summary>
            public void Show()
            {
                this.gameObject.SetActive(true);

                UpdateShop();
            }

            /// <summary>
            /// Hide the shop window
            /// </summary>
            public void Hide()
            {
                this.gameObject.SetActive(false);
            }

            /// <summary>
            /// Show if the request failed on the modal window
            /// </summary>
            /// <param name="content">The content to show</param>
            public void ShowRequestFail(string content)
            {
                ModalWindow.Show("Shop error", content);
            }

            /// <summary>
            /// Update the shop items
            /// </summary>
            public void UpdateShop()
            {
                if (shopItemsCache is null)
                {
                    CoroutineRunner.RunCoroutine(Server.SendGetRequest<List<Shop>>(ServerConfig.PATH_FOR_SHOP_GET_ALL, LoadedShopItems, onFailedAction: ShowRequestFail));
                }
                else
                {
                    LoadedShopItems(shopItemsCache);
                }
                CoinText.text = UserData.Instance.Game.Currency.ToString();
            }

            /// <summary>
            /// Display the loaded shop items
            /// </summary>
            /// <param name="shopItems"></param>
            private void LoadedShopItems(List<Shop> shopItems)
            {
                //Failsafe for the tests
                if (this.ShopParent == null || this.ShopParent.transform == null)
                {
                    // Exit or handle the case when the ShopParent is destroyed
                    Debug.LogWarning("ShopParent has been destroyed or is missing.");
                    return;
                }

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

            /// <summary>
            /// Get's the coin's displayed text
            /// </summary>
            /// <returns>The displayed textt</returns>
            public string GetCoinText()
            {
                return this.CoinText.text;
            }

            /// <summary>
            /// Gets the shop's parent
            /// </summary>
            /// <returns>Reference to the parent</returns>
            public GameObject GetShopParent()
            {
                return this.ShopParent;
            }

            #endregion TestGettters
        }
    }
}