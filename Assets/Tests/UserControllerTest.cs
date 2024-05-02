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
                User.UserData.Logout();

                if (userController != null)
                    GameObject.Destroy(this.userController.transform.parent.gameObject);
            }

            private void Login(string userName, string passWord)
            {
                TMPro.TMP_InputField[] fields = this.userController.LoginPanel.GetComponentsInChildren<TMPro.TMP_InputField>();
                fields[0].text = userName;
                fields[1].text = passWord;

                this.userController.SendLogin();
            }

            [UnityTest]
            public IEnumerator LoginTestEmail()

            {
                Login("test", "test");
                Assert.IsTrue(User.UserData.LoggedIn);
                yield return new WaitForSeconds(TestConfig.ANSWER_TOLERANCE);
                //   Assert.AreEqual("test@gmail.com", User.UserData.Email);
            }

            [UnityTest]
            public IEnumerator LoginTestUserName()
            {
                Login("test", "test");
                Assert.IsTrue(User.UserData.LoggedIn);
                Assert.AreEqual("test", User.UserData.Username);
                yield return new WaitForSeconds(TestConfig.ANSWER_TOLERANCE);
            }
        }
    }
}