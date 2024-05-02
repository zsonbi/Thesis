using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public static class Server
{
    /// <summary>
    /// Send a request to the server
    /// </summary>
    /// <typeparam name="T">What type should the response be deserialized (the onComplete function's parameter should be also this)</typeparam>
    /// <param name="url">Which url do we want to access should use the Serverconfig.PATH infix constants</param>
    /// <param name="form">The data to send for the server refer to the serverconfig what it needs</param>
    /// <param name="onComplete">When the response arrives what to do</param>
    /// <param name="beforeComplete">What to do before the completion so we can show it before that to the user and validate it later</param>
    public static IEnumerator SendRequest<T>(string url, WWWForm form, Action<T> onComplete, Action beforeComplete = null)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Post(url, form))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log(webRequest.error);
            }
            else
            {
                if (beforeComplete != null)
                {
                    beforeComplete();
                }

                Debug.Log(webRequest.downloadHandler.text);

                T result = JsonConvert.DeserializeObject<T>(webRequest.downloadHandler.text);
                if (result is not null)
                {
                    onComplete(result);
                }
                else
                {
                    Debug.Log("Server crashed");
                }
            }
        }
    }
}