using Config;
using System;
using System.Collections;
using System.Collections.Generic;
using Thesis_backend.Data_Structures;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using User;

public class ProfileHandler : MonoBehaviour
{






    public void Show()
    {

        this.gameObject.SetActive(true);
        LoadFromApi();
    }


    public void SendFriendRequest()
    {

    }

    public void Hide()
    {
        this.gameObject.SetActive(false);
    }

    private void LoadFromApi()
    {

    }

    /// <summary>
    /// Send a login request with the login panel's fields
    /// </summary>
    public void SendLogout()
    {
        StartCoroutine(Server.SendDeleteRequest<string>(ServerConfig.PATHFORLOGIN, LoggedOut));
    }

    /// <summary>
    /// When the logout was a success
    /// </summary>
    /// <param name="result">The server's response</param>
    private void LoggedOut(string result)
    {
        StartCoroutine(MoveToLoginScene());
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
