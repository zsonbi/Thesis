using Config;
using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace Tests
{
    namespace LoginWindow
    {
        public class UserControllerTests : LoginWindowTestsParent<UserController>
        {
            private void Login(string userName, string password)
            {
                TMPro.TMP_InputField[] fields = this.MainController.LoginPanel.GetComponentsInChildren<TMPro.TMP_InputField>();
                fields[0].text = userName;
                fields[1].text = password;

                this.MainController.SendLogin();
            }

            [UnityTest]
            public IEnumerator LoginTestUsername()
            {
                yield return LoadScene(false, true);
                Login(TestConfig.UserName, TestConfig.Password);
                yield return new WaitForSeconds(TestConfig.ANSWER_TOLERANCE);
                Assert.IsTrue(User.UserData.Instance.LoggedIn);
                Assert.AreEqual(TestConfig.UserName, User.UserData.Instance.Username);
                Assert.AreEqual(TestConfig.MAIN_SCENE_NAME, SceneManager.GetActiveScene().name);
            }

            [UnityTest]
            public IEnumerator LoginTestEmail()
            {
                yield return LoadScene(false, true);

                Login(TestConfig.Email, TestConfig.Password);
                yield return new WaitForSeconds(TestConfig.ANSWER_TOLERANCE);
                Assert.IsTrue(User.UserData.Instance.LoggedIn);
                Assert.AreEqual(TestConfig.Email, User.UserData.Instance.Email);
                Assert.AreEqual(TestConfig.MAIN_SCENE_NAME, SceneManager.GetActiveScene().name);
            }
        }
    }
}