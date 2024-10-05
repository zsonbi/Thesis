using Config;
using System.Collections;
using System.Collections.Generic;
using Thesis_backend.Data_Structures;
using UnityEngine;
using UnityEngine.SceneManagement;
using User;

public class LoggedInChecker : MonoBehaviour
{
    // Start is called before the first frame update
    private void Awake()
    {
        StartCoroutine(CheckLoggedInPeriodically());
    }

    private IEnumerator CheckLoggedInPeriodically()
    {
        while (true)
        {
            // Send the API request
            yield return CoroutineRunner.RunCoroutine(Server.SendGetRequest<Thesis_backend.Data_Structures.User>(ServerConfig.PATHFORCHECKLOGGEDIN, LoggedIn, onFailedAction: LoggedOut));

            // Wait for 60 seconds before sending the next request
            yield return new WaitForSeconds(60f);
        }
    }

    private void LoggedOut(string reason)
    {
        AsyncOperation loading = SceneManager.LoadSceneAsync("LoginScene", LoadSceneMode.Single);
    }

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
    /// When the login request's was responded log the user in
    /// </summary>
    /// <param name="result">The server's response</param>
    private void LoggedIn(Thesis_backend.Data_Structures.User result)
    {
        UserData.Instance.Init(result);
    }
}