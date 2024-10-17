#nullable enable

using Newtonsoft.Json;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Handles the request sending to the backend server
/// </summary>
public static class Server
{
    /// <summary>
    /// Send a post request to the server
    /// </summary>
    /// <typeparam name="T">What type should the response be deserialized (the onComplete function's parameter should be also this)</typeparam>
    /// <param name="url">Which url do we want to access should use the Serverconfig.PATH constants</param>
    /// <param name="dataToSend">The data to send for the server refer to the server documentation what it needs</param>
    /// <param name="onComplete">When the response arrives what to do</param>
    /// <param name="beforeComplete">What to do before the completion so we can show it before that to the user and validate it later</param>
    /// <param name="onFailedAction">What to do if the request failed</param>
    public static IEnumerator SendPostRequest<T>(string url, object dataToSend, Action<T>? onComplete = null, Action? beforeComplete = null, Action<string>? onFailedAction = null)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Post(url, JsonConvert.SerializeObject(dataToSend), "application/json"))
        {
            // Ensures credentials are allowed if needed
            webRequest.SetRequestHeader("Access-Control-Allow-Credentials", "true");

            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();
            //Process the result
            HandleRequestResult(webRequest, onComplete, beforeComplete, onFailedAction);
        }
    }

    /// <summary>
    /// Send a patch request to the server
    /// </summary>
    /// <typeparam name="T">What type should the response be deserialized (the onComplete function's parameter should be also this)</typeparam>
    /// <param name="url">Which url do we want to access should use the Serverconfig.PATH constants</param>
    /// <param name="dataToSend">The data to send for the server refer to the server documentation what it needs</param>
    /// <param name="onComplete">When the response arrives what to do</param>
    /// <param name="beforeComplete">What to do before the completion so we can show it before that to the user and validate it later</param>
    /// <param name="onFailedAction">What to do if the request failed</param>
    public static IEnumerator SendPatchRequest<T>(string url, object? dataToSend = null, Action<T>? onComplete = null, Action? beforeComplete = null, Action<string>? onFailedAction = null)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Put(url, JsonConvert.SerializeObject(dataToSend)))
        {
            webRequest.method = "PATCH";
            webRequest.SetRequestHeader("Content-Type", "application/json");
            webRequest.SetRequestHeader("Access-Control-Allow-Credentials", "true");  // Ensures credentials are allowed if needed

            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();
            //Process the result
            HandleRequestResult(webRequest, onComplete, beforeComplete, onFailedAction);
        }
    }

    /// <summary>
    /// Send a patch request to the server
    /// </summary>
    /// <typeparam name="T">What type should the response be deserialized (the onComplete function's parameter should be also this)</typeparam>
    /// <param name="url">Which url do we want to access should use the Serverconfig.PATH constants</param>
    /// <param name="onComplete">When the response arrives what to do</param>
    /// <param name="beforeComplete">What to do before the completion so we can show it before that to the user and validate it later</param>
    /// <param name="onFailedAction">What to do if the request failed</param>
    public static IEnumerator SendGetRequest<T>(string url, Action<T>? onComplete = null, Action? beforeComplete = null, Action<string>? onFailedAction = null)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            webRequest.SetRequestHeader("Access-Control-Allow-Credentials", "true");  // Ensures credentials are allowed if needed

            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            //Process the result
            HandleRequestResult(webRequest, onComplete, beforeComplete, onFailedAction);
        }
    }

    /// <summary>
    /// Send a patch request to the server
    /// </summary>
    /// <typeparam name="T">What type should the response be deserialized (the onComplete function's parameter should be also this)</typeparam>
    /// <param name="url">Which url do we want to access should use the Serverconfig.PATH constants</param>
    /// <param name="onComplete">When the response arrives what to do</param>
    /// <param name="beforeComplete">What to do before the completion so we can show it before that to the user and validate it later</param>
    /// <param name="onFailedAction">What to do if the request failed</param>
    public static IEnumerator SendDeleteRequest<T>(string url, Action<T>? onComplete = null, Action? beforeComplete = null, Action<string>? onFailedAction = null)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Put(url, new byte[0]))
        {
            webRequest.method = "DELETE";
            webRequest.SetRequestHeader("Content-Type", "application/json");
            webRequest.SetRequestHeader("Access-Control-Allow-Credentials", "true");  // Ensures credentials are allowed if needed

            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();
            //Process the result
            HandleRequestResult(webRequest, onComplete, beforeComplete, onFailedAction);
        }
    }

    /// <summary>
    /// Handles the response from the server
    /// </summary>
    /// <param name="webRequest">The sent request</param>
    /// <param name="onComplete">What to do if the request was successful</param>
    /// <param name="beforeComplete">What to do before the response is processed</param>
    /// <param name="onFailedAction">What to do if the request failed</param>
    private static void HandleRequestResult<T>(UnityWebRequest webRequest, Action<T>? onComplete = null, Action? beforeComplete = null, Action<string>? onFailedAction = null)
    {
        //Determine if the request was successful
        if (webRequest.result != UnityWebRequest.Result.Success)
        {
            //If failed write out the error and the reason
            Debug.Log(webRequest.error);
            if (webRequest.downloadHandler.text is not null)
            {
                onFailedAction?.Invoke(webRequest.downloadHandler.text);

                Debug.Log(webRequest.downloadHandler.text);
            }
        }
        //Otherwise process it
        else
        {
            //Before the request is fully complete invoke the necessary function
            if (beforeComplete != null)
            {
                beforeComplete();
            }

            Debug.Log(webRequest.downloadHandler.text);
            T? result = default;
            //Process the result fully
            try
            {
                if (typeof(T) == typeof(string))
                {
                    result = (T)(object)webRequest.downloadHandler.text;
                }
                else
                    result = JsonConvert.DeserializeObject<T>(webRequest.downloadHandler.text);
            }
            catch
            {
                Debug.Log($"Couldn't serialize response: {webRequest.downloadHandler.text}");
            }
            if (result is not null)
            {
                onComplete?.Invoke(result);
            }
        }
    }
}