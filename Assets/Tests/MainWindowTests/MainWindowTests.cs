using Assets.Tests;
using Assets.Tests.MainWindowTests.MainWindowTests;
using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace Tests
{
    namespace MainWindowTests
    {
        public class MainWindowTests : MainWindowTestsParent<MainWindowController>
        {
            private void InitScene()
            {
            }

            [UnityTest]
            public IEnumerator LogoutTest()
            {
                yield return LoadScene();

                profileHandler.Show();
                profileHandler.SendLogout();

                yield return new WaitForSeconds(TestConfig.ANSWER_TOLERANCE);

                Assert.False(User.UserData.Instance.LoggedIn);
                Assert.AreEqual(TestConfig.LOGIN_SCENE_NAME, SceneManager.GetActiveScene().name);
            }

            [UnityTest]
            public IEnumerator DummyTest()
            {
                yield return null;
                Assert.IsTrue(true);
            }
        }
    }
}