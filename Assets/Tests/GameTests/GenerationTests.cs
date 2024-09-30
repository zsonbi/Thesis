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

                yield return WaitForCondition(() => MainController.Running);

                Assert.NotNull(MainController.Player);
                int currentScore = MainController.Score;

                yield return new WaitForSecondsRealtime(5);

                Assert.AreEqual(currentScore + 5, MainController.Score);
            }
        }
    }
}