using Assets.Tests;
using Config;
using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace Tests
{
    namespace UITests
    {
        public class UserControllerTests : UnityTestParent<UserController>
        {
            // Coroutine to load the scene asynchronously before tests
            //private IEnumerator LoadScene()
            //{
            //    User.UserData.Instance.Logout();

            //    yield return CoroutineRunner.RunCoroutine(Server.SendDeleteRequest<string>(ServerConfig.PATHFORLOGOUT));

            //    // Load the scene asynchronously
            //    AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(TestConfig.LOGIN_SCENE_NAME, LoadSceneMode.Single);

            //    // Wait for the scene to load
            //    while (!asyncLoad.isDone)
            //    {
            //        yield return null;
            //    }

            //    Scene loadedScene = SceneManager.GetSceneByName(TestConfig.LOGIN_SCENE_NAME);
            //    while (!loadedScene.isLoaded || SceneManager.GetActiveScene().name != TestConfig.LOGIN_SCENE_NAME)
            //    {
            //        // Wait until the scene is both loaded and active
            //        yield return null;
            //    }

            //    // Wait for the UserController to be available in the scene
            //    GameObject userControllerObject = null;
            //    while (userControllerObject == null)
            //    {
            //        userControllerObject = GameObject.Find(TestConfig.USER_CONTROLLER_OBJECT_NAME);

            //        yield return null; // Wait for the next frame if not found
            //    }
            //    // Get the UserController component once it's found
            //    this.userController = userControllerObject.GetComponent<UserController>();
            //    yield return null; // Ensure one more frame before proceeding
            //    yield return new WaitForSeconds(TestConfig.SCENE_TOLERANCE);
            //}

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
                yield return LoadScene(TestConfig.LOGIN_SCENE_NAME, TestConfig.USER_CONTROLLER_OBJECT_NAME, true);
                Login(TestConfig.UserName, TestConfig.Password);
                yield return new WaitForSeconds(TestConfig.ANSWER_TOLERANCE);
                Assert.IsTrue(User.UserData.Instance.LoggedIn);
                Assert.AreEqual(TestConfig.UserName, User.UserData.Instance.Username);
                Assert.AreEqual("MainScene", SceneManager.GetActiveScene().name);
            }

            [UnityTest]
            public IEnumerator LoginTestEmail()
            {
                yield return LoadScene(TestConfig.LOGIN_SCENE_NAME, TestConfig.USER_CONTROLLER_OBJECT_NAME, true);

                Login(TestConfig.Email, TestConfig.Password);
                yield return new WaitForSeconds(TestConfig.ANSWER_TOLERANCE);
                Assert.IsTrue(User.UserData.Instance.LoggedIn);
                Assert.AreEqual(TestConfig.Email, User.UserData.Instance.Email);
                Assert.AreEqual("MainScene", SceneManager.GetActiveScene().name);
            }
        }
    }
}