using Game;
using NUnit.Framework;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using User;
using static UnityEditor.Progress;

namespace Tests
{
    namespace GameTests
    {
        public class ShopTests : GameTestsParent<GameController>
        {
            [UnityTest]
            public IEnumerator DisplayAndHideShop()
            {
                yield return LoadScene();

                GameUI.ShowShopWindow();

                Assert.IsTrue(ShopWindow.gameObject.activeInHierarchy);

                GameUI.HideShopWindow();

                Assert.IsFalse(ShopWindow.gameObject.activeInHierarchy);
            }

            [UnityTest]
            public IEnumerator CoinDisplayAmountTest()
            {
                yield return LoadScene();

                GameUI.ShowShopWindow();

                Assert.IsTrue(ShopWindow.gameObject.activeInHierarchy);
                Assert.AreEqual(UserData.Instance.Game.Currency.ToString(), ShopWindow.GetCoinText());
            }

            [Order(0)]
            [UnityTest]
            public IEnumerator BuyTest()
            {
                yield return LoadScene();

                GameUI.ShowShopWindow();

                Assert.IsTrue(ShopWindow.gameObject.activeInHierarchy);
                yield return new WaitForSeconds(TestConfig.ANSWER_TOLERANCE);

                int shopItemCount = GameObject.FindObjectsByType<ShopItem>(FindObjectsSortMode.None).Length;
                if (shopItemCount == 0)
                {
                    Assert.Fail("Can't load shop items");
                }
                int initialMoney = UserData.Instance.Game.Currency;
                int spent = 0;
                for (int i = 0; i < shopItemCount; i++)
                {
                    ShopItem shopItem = GameObject.FindObjectsByType<ShopItem>(FindObjectsSortMode.None)[0];

                    if (shopItem.Owned)
                    {
                        continue;
                    }

                    int ownedCarsCount = UserData.Instance.Game.OwnedCars.Count;
                    spent += Convert.ToInt32(shopItem.GetDisplayedPrice());

                    shopItem.Buy();

                    while (!shopItem.Owned)
                    {
                        {
                            yield return new WaitForSeconds(0.1f);
                        }
                    }
                    Assert.AreEqual(initialMoney - spent, UserData.Instance.Game.Currency);
                }
                yield return new WaitForSeconds(TestConfig.ANSWER_TOLERANCE);

                Assert.AreEqual(shopItemCount, UserData.Instance.Game.OwnedCars.Count);

                GameUI.HideShopWindow();

                Assert.IsFalse(ShopWindow.gameObject.activeInHierarchy);

                GameUI.ShowShopWindow();

                Assert.IsTrue(ShopWindow.gameObject.activeInHierarchy);

                foreach (var item in GameObject.FindObjectsByType<ShopItem>(FindObjectsSortMode.None))
                {
                    Assert.True(item.Owned);
                }
            }
        }
    }
}