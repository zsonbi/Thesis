using Assets.Tests;
using Config;
using Game.World;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
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
                CoroutineRunner.RunCoroutine(Server.SendDeleteRequest<string>(ServerConfig.PATHFORLOGOUT));
                //TestHandler.Instance.ServerConnection.StartCoroutine(Server.SendDeleteRequest<string>(ServerConfig.PATHFORLOGOUT));

                this.userController = GameObject.Instantiate(userControllerPrefab).transform.Find("UserController").GetComponent<UserController>();
            }

            [TearDown]
            public void Shutdown()
            {
                User.UserData.Instance.Logout();

                if (userController != null)
                {
                    userController.StartCoroutine(Server.SendDeleteRequest<string>(ServerConfig.PATHFORLOGOUT));
                    GameObject.Destroy(this.userController.transform.parent.gameObject);
                }
            }

            private void Login(string userName, string password)
            {
                TMPro.TMP_InputField[] fields = this.userController.LoginPanel.GetComponentsInChildren<TMPro.TMP_InputField>();
                fields[0].text = userName;
                fields[1].text = password;

                this.userController.SendLogin();
            }

            [UnityTest]
            public IEnumerator LoginTestUsername()
            {
                yield return new WaitForSeconds(TestConfig.ANSWER_TOLERANCE);

                Login(TestConfig.UserName, TestConfig.Password);
                yield return new WaitForSeconds(TestConfig.ANSWER_TOLERANCE);
                Assert.IsTrue(User.UserData.Instance.LoggedIn);
                Assert.AreEqual(TestConfig.UserName, User.UserData.Instance.Username);
            }

            [UnityTest]
            public IEnumerator LoginTestEmail()
            {
                yield return new WaitForSeconds(TestConfig.ANSWER_TOLERANCE);

                Login(TestConfig.Email, TestConfig.Password);
                yield return new WaitForSeconds(TestConfig.ANSWER_TOLERANCE);
                Assert.IsTrue(User.UserData.Instance.LoggedIn);
                Assert.AreEqual(TestConfig.Email, User.UserData.Instance.Email);
            }
        }
    }
}