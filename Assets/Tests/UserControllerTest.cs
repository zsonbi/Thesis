using Game.World;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TestTools;
using User;

namespace Tests
{
    namespace UITests
    {
        public class UserControllerTests
        {
            private GameObject userControllerPrefab = Resources.Load<GameObject>("Prefabs/UserControllerPrefab");
            private UserController userController;

            [SetUp]
            public void Init()
            {
                //GameObject temp = GameObject.Instantiate(userControllerPrefab);

                this.userController = GameObject.Instantiate(userControllerPrefab).transform.Find("UserController").GetComponent<UserController>();
            }

            [TearDown]
            public void Shutdown()
            {
                User.UserData.Instance.Logout();

                if (userController != null)
                    GameObject.Destroy(this.userController.transform.parent.gameObject);
            }

            private void Login(string userName, string password)
            {
                TMPro.TMP_InputField[] fields = this.userController.LoginPanel.GetComponentsInChildren<TMPro.TMP_InputField>();
                fields[0].text = userName;
                fields[1].text = password;

                this.userController.SendLogin();
            }

            [UnityTest]
            public IEnumerator LoginTestEmail()

            {
                Login("test2", "test");
                yield return new WaitForSeconds(TestConfig.ANSWER_TOLERANCE);
                Assert.IsTrue(User.UserData.Instance.LoggedIn);

                //   Assert.AreEqual("test@gmail.com", User.UserData.Email);
            }

            [UnityTest]
            public IEnumerator LoginTestUserName()
            {
                Login("test2", "test");
                yield return new WaitForSeconds(TestConfig.ANSWER_TOLERANCE);
                Assert.IsTrue(User.UserData.Instance.LoggedIn);
                Assert.AreEqual("test2", User.UserData.Instance.Username);
            }
        }
    }
}