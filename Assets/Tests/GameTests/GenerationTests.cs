using Game;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UIElements;

namespace Tests
{
    namespace GameTests
    {
        public class GenerationTests : GameTestsParent<GameController>
        {
            private List<Building> buildings = new List<Building>();

            [UnityTest]
            public IEnumerator NewGameTest()
            {
                yield return LoadScene();

                yield return new WaitForSeconds(TestConfig.ANSWER_TOLERANCE);

                GameUI.NewGame();

                yield return WaitForCondition(() => MainController.Running);

                Assert.NotNull(MainController.Player);
                int currentScore = MainController.Score;

                yield return new WaitForSecondsRealtime(5);

                Assert.AreEqual(currentScore + 5, MainController.Score);
            }

            [UnityTest]
            public IEnumerator GameOverWithZeroInputTest()
            {
                yield return LoadScene();

                yield return new WaitForSeconds(TestConfig.ANSWER_TOLERANCE);

                GameUI.NewGame();
                yield return WaitForCondition(() => MainController.Running);

                for (int i = 0; i < 10; i++)
                {
                    yield return WaitForCondition(() => !MainController.Running);
                }
                Assert.False(MainController.Running);
                Assert.NotNull(GameObject.Find("GameOver"));
            }

            [UnityTest]
            public IEnumerator BuildingCollisionTest()
            {
                var handle = Addressables.LoadAssetsAsync<GameObject>("Buildings", SpawnBuilding);
                yield return handle.Task;
                yield return new WaitForSeconds(1f);

                foreach (var item in buildings)
                {
                    yield return LoadScene();
                    yield return new WaitForSeconds(TestConfig.ANSWER_TOLERANCE);

                    GameUI.NewGame();
                    yield return WaitForCondition(() => MainController.Running);
                    Vector3 BuildingPos = MainController.Player.gameObject.transform.position + MainController.Player.gameObject.transform.forward * 96f;
                    BuildingPos = new Vector3(BuildingPos.x + 4, BuildingPos.y, BuildingPos.z);
                    Addressables.InstantiateAsync(item.AddressableKey, BuildingPos, Quaternion.Euler(0, 0, 0), this.MainController.World.gameObject.transform);

                    yield return WaitForCondition(() => MainController.Player.Health != MainController.Player.MAX_HEALTH);

                    Assert.True(MainController.Player.Health != MainController.Player.MAX_HEALTH);
                }
            }

            private void SpawnBuilding(GameObject building)
            {
                buildings.Add(building.GetComponent<Building>());
                buildings.Last().SetAddressableKey($"Buildings/{building.name}.prefab");
            }
        }
    }
}