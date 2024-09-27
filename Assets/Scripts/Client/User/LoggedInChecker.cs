using Config;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoggedIn : MonoBehaviour
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
            StartCoroutine(Server.SendDeleteRequest<string>(ServerConfig.PATHFORLOGOUT, onFailedAction: LoggedOut));

            // Wait for 60 seconds before sending the next request
            yield return new WaitForSeconds(120f);
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
}