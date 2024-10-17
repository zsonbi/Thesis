using Config;
using Thesis_backend.Data_Structures;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using User;
using Utility;

namespace Game
{
    namespace UI
    {
        /// <summary>
        /// A single shop item
        /// </summary>
        public class ShopItem : ThreadSafeMonoBehaviour
        {
            /// <summary>
            /// Reference to the name display
            /// </summary>
            [SerializeField]
            private TMP_Text ProductName;

            /// <summary>
            /// Reference to the shop item image
            /// </summary>
            [SerializeField]
            private Image ProductImage;

            /// <summary>
            /// Reference to the price display
            /// </summary>
            [SerializeField]
            private TMP_Text Price;

            /// <summary>
            /// Reference to the buy button
            /// </summary>
            [SerializeField]
            private Button BuyButton;

            /// <summary>
            /// Reference to the parent shop window
            /// </summary>
            private ShopWindow shopWindow;

            /// <summary>
            /// The shop id of the item
            /// </summary>
            public long Id { get; private set; }

            /// <summary>
            /// Is this item already owned
            /// </summary>
            public bool Owned { get; private set; }

            /// <summary>
            /// Initializes this shop item
            /// </summary>
            /// <param name="parentWindow">The parent shop window</param>
            /// <param name="item">What it is selling</param>
            /// <param name="image">The item's image</param>
            /// <param name="owned">Is is already owned</param>
            public void Init(ShopWindow parentWindow, Shop item, Sprite image, bool owned)
            {
                this.shopWindow = parentWindow;
                this.ProductName.text = item.ProductName;
                this.ProductImage.sprite = image;
                this.Price.text = item.Cost.ToString();
                this.Id = item.ID;
                this.BuyButton.gameObject.SetActive(!owned);
                this.Owned = owned;
            }

            /// <summary>
            /// Buy this shop item
            /// </summary>
            public void Buy()
            {
                CoroutineRunner.RunCoroutine(Server.SendPatchRequest<Thesis_backend.Data_Structures.Game>(ServerConfig.PATH_FOR_BUY_CAR(Id), new WWWForm(), Bought));
            }

            /// <summary>
            /// Callback when it is bought
            /// </summary>
            /// <param name="game">Response from the server with the updates owned car list and remaining currency</param>
            private void Bought(Thesis_backend.Data_Structures.Game game)
            {
                UserData.Instance.Game.OwnedCars = game.OwnedCars;
                UserData.Instance.Game.Currency = game.Currency;
                this.Owned = true;
                shopWindow.UpdateShop();
            }

            #region TestGettters

            /// <summary>
            /// Get the displayed price to check in the test
            /// </summary>
            /// <returns>The displayed price</returns>
            public string GetDisplayedPrice()
            {
                return Price.text;
            }

            #endregion TestGettters
        }
    }
}