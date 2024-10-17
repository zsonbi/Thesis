using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using MainPage;
using User;
using Utility;

namespace Tests
{
    namespace MainWindowTests
    {
        public class ProfileWindowTest : MainWindowTestsParent<MainWindowController>
        {
            [UnityTest]
            public IEnumerator LogoutTest()
            {
                yield return LoadScene();
                yield return new WaitForSeconds(TestConfig.ANSWER_TOLERANCE);

                profileHandler.Show();
                profileHandler.SendLogout();

                yield return WaitForCondition(() => UserData.Instance.LoggedIn);
                yield return WaitForCondition(() => SceneManager.GetActiveScene().name == TestConfig.LOGIN_SCENE_NAME);
                Assert.False(User.UserData.Instance.LoggedIn);
                Assert.AreEqual(TestConfig.LOGIN_SCENE_NAME, SceneManager.GetActiveScene().name);
            }
        }
    }
}