#define TESTING

using Config;
using System;
using System.Collections;
using Utility;
using UnityEngine.SceneManagement;
using UnityEngine;
using User;
using Thesis_backend.Data_Structures;
using UnityEngine.InputSystem;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace Tests
{
    public abstract class UnityTestParent<T> : InputTestFixture
    {
        protected T MainController;

        protected virtual IEnumerator LoadSceneBase(string sceneName, string controllerObjectName, bool login = true, bool logout = false)
        {
            SceneManager.LoadScene(TestConfig.EMPTY_SCENE_NAME, LoadSceneMode.Single);
            yield return WaitForCondition(() => SceneManager.GetActiveScene().name == TestConfig.EMPTY_SCENE_NAME);
            if (logout)
            {
                User.UserData.Instance.Logout();

                yield return CoroutineRunner.RunCoroutine(Server.SendDeleteRequest<string>(ServerConfig.PATHFORLOGOUT));
                yield return new WaitForSeconds(TestConfig.ANSWER_TOLERANCE);
            }
            if (login && !UserData.Instance.LoggedIn)
            {
                yield return this.Login();
                yield return new WaitForSeconds(TestConfig.ANSWER_TOLERANCE);
            }

            // Load the scene asynchronously
            SceneManager.LoadScene(sceneName, LoadSceneMode.Single);

            Scene loadedScene = SceneManager.GetSceneByName(sceneName);
            while (!loadedScene.isLoaded || SceneManager.GetActiveScene().name != sceneName)
            {
                // Wait until the scene is both loaded and active
                yield return null;
            }

            // Get the UserController component once it's found
            this.MainController = GameObject.Find(controllerObjectName).GetComponent<T>();
            yield return null;
        }

        protected IEnumerator Login(string username = null, string password = null)
        {
            if (username == null)
            {
                username = TestConfig.UserName;
            }
            if (password == null)
            {
                password = TestConfig.Password;
            }

            UserLoginRequest userLoginRequest = new UserLoginRequest()
            {
                UserIdentification = username,
                Password = password,
            };

            yield return CoroutineRunner.RunCoroutine(Server.SendPostRequest<Thesis_backend.Data_Structures.User>(ServerConfig.PATHFORLOGIN, userLoginRequest, onComplete: UserData.Instance.Init));
            yield return WaitForCondition(() => UserData.Instance.LoggedIn);
            yield return WaitForCondition(() => UserData.Instance.Username == username || UserData.Instance.Email == username);
        }

        protected IEnumerator WaitForCondition(Func<Boolean> condition)
        {
            for (int j = 0; j < 300; j++)
            {
                if (condition.Invoke())
                {
                    break;
                }
                yield return new WaitForSeconds(0.1f);
            }
            yield return null;
        }

        protected IEnumerator WaitForFewFrames(int frameCount = 60)
        {
            for (int i = 0; i < frameCount; i++)
            {
                yield return new WaitForFixedUpdate();
            }
        }

        [UnityTearDown]
        public new IEnumerator TearDown()
        {
            base.TearDown();

            CoroutineRunner.StopAllCoroutinesGlobal();

            SceneManager.LoadScene(TestConfig.EMPTY_SCENE_NAME, LoadSceneMode.Single);

            yield return WaitForCondition(() => SceneManager.GetActiveScene().name == TestConfig.EMPTY_SCENE_NAME);
        }
    }
}