using Config;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using User;

namespace User
{
    /// <summary>
    /// Checks if the user has a session stored in the backend
    /// </summary>
    public class LoggedInChecker : ThreadSafeMonoBehaviour
    {
        /// <summary>
        /// Called when the script is loaded
        /// </summary>
        private void Awake()
        {
            StartCoroutine(CheckLoggedInPeriodically());
        }

        /// <summary>
        /// Coroutine which calls the api endpoint to check if the user is still logged in
        /// </summary>
        private IEnumerator CheckLoggedInPeriodically()
        {
            //Run it while the gameobject is active
            while (true)
            {
                // Send the API request
                yield return CoroutineRunner.RunCoroutine(Server.SendGetRequest<Thesis_backend.Data_Structures.User>(ServerConfig.PATHFORCHECKLOGGEDIN, LoggedIn, onFailedAction: LoggedOut));

                // Wait for 60 seconds before sending the next request
                yield return new WaitForSeconds(60f);
            }
        }

        /// <summary>
        /// Handles if the user is not logged in
        /// </summary>
        /// <param name="reason">Why the api request failed</param>
        private void LoggedOut(string reason)
        {
            UserData.Instance.Logout();
            AsyncOperation loading = SceneManager.LoadSceneAsync("LoginScene", LoadSceneMode.Single);
        }

        /// <summary>
        /// Move the user to the login screen
        /// </summary>
        private IEnumerator MoveToLoginScene()
        {
            AsyncOperation loading = SceneManager.LoadSceneAsync("LoginScene", LoadSceneMode.Single);

            while (true)
            {
                Debug.Log(loading.progress);

                yield return null;
            }
        }

        /// <summary>
        /// Update the user data from the server data
        /// </summary>
        /// <param name="result">The server's response</param>
        private void LoggedIn(Thesis_backend.Data_Structures.User result)
        {
            if (UserData.Instance.LoggedIn)
            {
                return;
            }

            UserData.Instance.Init(result);
        }
    }
}