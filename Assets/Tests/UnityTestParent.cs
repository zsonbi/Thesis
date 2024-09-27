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

namespace Assets.Tests
{
    public abstract class UnityTestParent<T>
    {
        protected T MainController;

        protected IEnumerator LoadScene(string sceneName, string controllerObjectName, bool logout = false)
        {
            if (logout)
            {
                User.UserData.Instance.Logout();

                yield return CoroutineRunner.RunCoroutine(Server.SendDeleteRequest<string>(ServerConfig.PATHFORLOGOUT));
                yield return new WaitForSeconds(TestConfig.ANSWER_TOLERANCE);
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
        }
    }
}