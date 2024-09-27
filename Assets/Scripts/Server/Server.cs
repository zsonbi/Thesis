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
    public static IEnumerator SendPostRequest<T>(string url, object dataToSend, Action<T> onComplete = null, Action beforeComplete = null, Action<string> onFailedAction = null)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Post(url, JsonConvert.SerializeObject(dataToSend), "application/json"))
        {
            webRequest.SetRequestHeader("Access-Control-Allow-Credentials", "true");  // Ensures credentials are allowed if needed

            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(webRequest.error);
                if (webRequest.downloadHandler.text is not null)
                {
                    Debug.Log(webRequest.downloadHandler.text);
                    onFailedAction?.Invoke(webRequest.downloadHandler.text);
                }
            }
            else
            {
                if (beforeComplete != null)
                {
                    beforeComplete();
                }

                Debug.Log(webRequest.downloadHandler.text);
                T? result = default;
                try
                {
                    if (typeof(T) == typeof(string))
                    {
                        result = (T)(object)webRequest.downloadHandler.text;
                    }
                    else
                        result = JsonConvert.DeserializeObject<T>(webRequest.downloadHandler.text);
                }
                catch (Exception e)
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

    public static IEnumerator SendPatchRequest<T>(string url, object dataToSend = null, Action<T> onComplete = null, Action beforeComplete = null, Action<string> onFailedAction = null)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Put(url, JsonConvert.SerializeObject(dataToSend)))
        {
            webRequest.method = "PATCH";
            webRequest.SetRequestHeader("Content-Type", "application/json");
            webRequest.SetRequestHeader("Access-Control-Allow-Credentials", "true");  // Ensures credentials are allowed if needed

            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(webRequest.error);
                if (webRequest.downloadHandler.text is not null)
                {
                    onFailedAction?.Invoke(webRequest.downloadHandler.text);

                    Debug.Log(webRequest.downloadHandler.text);
                }
            }
            else
            {
                if (beforeComplete != null)
                {
                    beforeComplete();
                }

                Debug.Log(webRequest.downloadHandler.text);
                T? result = default;
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

    public static IEnumerator SendGetRequest<T>(string url, Action<T> onComplete = null, Action beforeComplete = null, Action<string> onFailedAction = null)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            webRequest.SetRequestHeader("Access-Control-Allow-Credentials", "true");  // Ensures credentials are allowed if needed

            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(webRequest.error);
                if (webRequest.downloadHandler.text is not null)
                {
                    Debug.Log(webRequest.downloadHandler.text);
                    onFailedAction?.Invoke(webRequest.downloadHandler.text);
                }
            }
            else
            {
                if (beforeComplete != null)
                {
                    beforeComplete();
                }

                Debug.Log(webRequest.downloadHandler.text);
                T? result = default;
                try
                {
                    if (typeof(T) == typeof(string))
                    {
                        result = (T)(object)webRequest.downloadHandler.text;
                    }
                    else
                        result = JsonConvert.DeserializeObject<T>(webRequest.downloadHandler.text);
                }
                catch (Exception e)
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

    public static IEnumerator SendDeleteRequest<T>(string url, Action<T> onComplete = null, Action beforeComplete = null, Action<string> onFailedAction = null)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Put(url, new byte[0]))
        {
            webRequest.method = "DELETE";
            webRequest.SetRequestHeader("Content-Type", "application/json");
            webRequest.SetRequestHeader("Access-Control-Allow-Credentials", "true");  // Ensures credentials are allowed if needed

            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(webRequest.error);
                if (webRequest.downloadHandler.text is not null)
                {
                    Debug.Log(webRequest.downloadHandler.text);
                    onFailedAction?.Invoke(webRequest.downloadHandler.text);
                }
            }
            else
            {
                if (beforeComplete != null)
                {
                    beforeComplete();
                }

                Debug.Log(webRequest.downloadHandler.text);
                T? result = default;
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
}