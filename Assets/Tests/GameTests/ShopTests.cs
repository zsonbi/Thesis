using Game;
using NUnit.Framework;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using User;

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

                yield return WaitForCondition(() => ShopWindow is not null);

                Assert.IsTrue(ShopWindow.gameObject.activeInHierarchy);
                Assert.AreEqual(UserData.Instance.Game.Currency.ToString(), ShopWindow.GetCoinText());
            }

            [Order(1)]
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
                    ShopItem shopItem = GameObject.FindObjectsByType<ShopItem>(FindObjectsSortMode.InstanceID)[i];

                    if (shopItem.Owned)
                    {
                        continue;
                    }

                    int ownedCarsCount = UserData.Instance.Game.OwnedCars.Count;
                    spent += Convert.ToInt32(shopItem.GetDisplayedPrice());

                    shopItem.Buy();

                    yield return WaitForCondition(() => shopItem.Owned);

                    Assert.AreEqual(initialMoney - spent, UserData.Instance.Game.Currency);
                }

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

            [Order(2)]
            [UnityTest]
            public IEnumerator SkinRotateLeftTest()
            {
                yield return LoadScene();
                for (int j = 0; j < 10; j++)
                {
                    int currIndex = GameUI.SelectedSkinIndex;
                    for (int i = 0; i < 10; i++)
                    {
                        GameUI.LeftRotateSkin();
                        yield return null;
                    }
                    Assert.AreEqual((currIndex + User.UserData.Instance.Game.OwnedCars.Count - (10 % User.UserData.Instance.Game.OwnedCars.Count)) % User.UserData.Instance.Game.OwnedCars.Count, GameUI.SelectedSkinIndex);
                }
            }

            [Order(3)]
            [UnityTest]
            public IEnumerator SkinRotateRightTest()
            {
                yield return LoadScene();
                for (int j = 0; j < 10; j++)
                {
                    int currIndex = GameUI.SelectedSkinIndex;

                    for (int i = 0; i < 10; i++)
                    {
                        GameUI.RightRotateSkin();
                        yield return null;
                    }
                    Assert.AreEqual((currIndex + (10 % User.UserData.Instance.Game.OwnedCars.Count)) % User.UserData.Instance.Game.OwnedCars.Count, GameUI.SelectedSkinIndex);
                }
            }

            [Order(4)]
            [UnityTest]
            public IEnumerator UseSkinTest()
            {
                yield return LoadScene();

                while (GameUI.SelectedSkinIndex != 2)
                {
                    GameUI.RightRotateSkin();
                    yield return null;
                }

                yield return new WaitForSeconds(TestConfig.ANSWER_TOLERANCE);

                GameUI.NewGame();

                yield return WaitForCondition(() => MainController.Running);

                Assert.NotNull(MainController.Player);
                Assert.AreEqual(UserData.Instance.Game.OwnedCars[GameUI.SelectedSkinIndex].ShopId, MainController.Player.SkinId);
                yield return new WaitForSeconds(TestConfig.ANSWER_TOLERANCE);
            }
        }
    }
}