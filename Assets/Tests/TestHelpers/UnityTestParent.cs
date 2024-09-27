using Config;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tests;
using UnityEngine.SceneManagement;
using UnityEngine;
using User;
using Thesis_backend.Data_Structures;

namespace Assets.Tests
{
    public abstract class UnityTestParent<T>
    {
        protected T MainController;

        protected virtual IEnumerator LoadSceneBase(string sceneName, string controllerObjectName, bool login = true, bool logout = false)
        {
            if (logout)
            {
                User.UserData.Instance.Logout();

                yield return CoroutineRunner.RunCoroutine(Server.SendDeleteRequest<string>(ServerConfig.PATHFORLOGOUT));
                yield return new WaitForSeconds(TestConfig.ANSWER_TOLERANCE);
            }
            if (login && UserData.Instance.LoggedIn)
            {
                this.Login();
            }

            // Load the scene asynchronously
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);

            // Wait for the scene to load
            while (!asyncLoad.isDone)
            {
                yield return null;
            }

            Scene loadedScene = SceneManager.GetSceneByName(sceneName);
            while (!loadedScene.isLoaded || SceneManager.GetActiveScene().name != sceneName)
            {
                // Wait until the scene is both loaded and active
                yield return null;
            }

            // Get the UserController component once it's found
            this.MainController = GameObject.Find(controllerObjectName).GetComponent<T>();
            yield return new WaitForEndOfFrame();
        }

        private IEnumerator Login()
        {
            UserLoginRequest userLoginRequest = new UserLoginRequest()
            {
                UserIdentification = TestConfig.UserName,
                Password = TestConfig.Password,
            };

            yield return CoroutineRunner.RunCoroutine(Server.SendPostRequest<Thesis_backend.Data_Structures.User>(ServerConfig.PATHFORLOGIN, userLoginRequest));
            yield return new WaitForSeconds(TestConfig.ANSWER_TOLERANCE);
        }
    }
}