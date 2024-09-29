using Game;
using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace Tests
{
    namespace GameTests
    {
        public class GenerationTests : GameTestsParent<GameController>
        {
            [UnityTest]
            public IEnumerator NewGameTest()
            {
                yield return LoadScene();

                yield return new WaitForSeconds(TestConfig.ANSWER_TOLERANCE);

                GameUI.NewGame();

                for (int j = 0; j < 300; j++)
                {
                    if (MainController.Running)
                    {
                        break;
                    }
                    yield return new WaitForSeconds(0.1f);
                }

                Assert.NotNull(MainController.Player);
                int currentScore = MainController.Score;

                yield return new WaitForSecondsRealtime(5f);

                Assert.AreEqual(currentScore + 5, MainController.Score);
            }
        }
    }
}