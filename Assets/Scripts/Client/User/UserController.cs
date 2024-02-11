using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using Config;
using System.Net.Mail;
using User;

public class UserController : MonoBehaviour
{
    /// <summary>
    /// The login controls
    /// </summary>
    [SerializeField]
    public GameObject LoginPanel;

    /// <summary>
    /// The register controls
    /// </summary>
    [SerializeField]
    public GameObject RegisterPanel;

    [SerializeField]
    public ModalWindow ModalWindow;

    // Start is called before the first frame update
    private void Start()
    {
        //Cap the fps to 60
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0;

        //So the user doesn't need to log in if not necessary
        CheckLoggedIn();
    }

    /// <summary>
    /// Send a request to check if the user has logged in already
    /// </summary>
    private void CheckLoggedIn()
    {
        if (UserData.LoggedIn)
        {
            MoveToMainScence();
            return;
        }

        WWWForm form = new WWWForm();
        StartCoroutine(Server.SendRequest<Dictionary<string, string>>(ServerConfig.PATHFORCHECKLOGGEDIN, form, LoggedIn));
    }

    /// <summary>
    /// Send a login request with the login panel's fields
    /// </summary>
    public void SendLogin()
    {
        WWWForm form = new WWWForm();
        TMPro.TMP_InputField[] fields = LoginPanel.GetComponentsInChildren<TMPro.TMP_InputField>();
        foreach (var item in fields)
        {
            if (item.text == "")
            {
                ModalWindow.Show("Login error", $"{item.name} is empty!");
                return;
            }
            else
            {
                form.AddField(item.name, item.text);
            }
        }

        StartCoroutine(Server.SendRequest<Dictionary<string, string>>(ServerConfig.PATHFORLOGIN, form, LoggedIn));
    }

    /// <summary>
    /// When the login request's was responded log the user in
    /// </summary>
    /// <param name="result">The server's response</param>
    private void LoggedIn(Dictionary<string, string> result)
    {
        if (result.ContainsKey("res") && result["res"] == "success")
        {
            UserData.Init(result["username"], result["email"], Convert.ToInt32(result["id"]), Convert.ToInt32(result["settingsId"]), -1, DateTime.Parse(result["lastLoggedIn"]), DateTime.Parse(result["registered"]));
            StartCoroutine(MoveToMainScence());
        }
        else if (result.ContainsKey("error"))
        {
            Debug.Log(result["error"]);

            ModalWindow.Show("Login error", result["error"]);
        }
    }

    /// <summary>
    /// Send a register request with the register panel's fields
    /// </summary>
    public void SendRegister()
    {
        WWWForm form = new WWWForm();
        TMPro.TMP_InputField[] fields = RegisterPanel.GetComponentsInChildren<TMPro.TMP_InputField>();
        foreach (var item in fields)
        {
            if (item.text == "")
            {
                ModalWindow.Show("Register error", $"{item.name} is empty!");
                return;
            }
            else
            {
                form.AddField(item.name, item.text);
            }
        }

        if (!IsValidEmail(fields[0].text))
        {
            ModalWindow.Show("Register error", "This is not a valid email!");

            return;
        }
        if (fields[2].text != fields[3].text)
        {
            ModalWindow.Show("Register error", "Passwords does not match!");
            return;
        }

        StartCoroutine(Server.SendRequest<Dictionary<string, string>>(ServerConfig.PATHFORREGISTER, form, Registered));
    }

    /// <summary>
    /// When the login request's was responded register the user and log him in
    /// </summary>
    /// <param name="result">The server's response</param>
    private void Registered(Dictionary<string, string> result)
    {
        if (result.ContainsKey("res") && result["res"] == "success")
        {
            UserData.Init(result["username"], result["email"], Convert.ToInt32(result["id"]), Convert.ToInt32(result["settingsId"]), -1, DateTime.Parse(result["lastLoggedIn"]), DateTime.Parse(result["registered"]));

            StartCoroutine(MoveToMainScence());
        }
        else if (result.ContainsKey("error"))
        {
            Debug.Log(result["error"]);
            ModalWindow.Show("Register error", result["error"]);
        }
    }

    /// <summary>
    /// Move from the user creation and login scene
    /// </summary>
    private IEnumerator MoveToMainScence()
    {
        AsyncOperation loading = SceneManager.LoadSceneAsync("MainMobileFrame", LoadSceneMode.Single);

        while (!loading.isDone)
        {
            yield return null;
        }
    }

    /// <summary>
    /// Check if the email is valid
    /// </summary>
    /// <param name="emailaddress">The email to validate</param>
    /// <returns>true-correct false-sus</returns>
    public bool IsValidEmail(string emailaddress)
    {
        try
        {
            MailAddress m = new MailAddress(emailaddress);

            return true;
        }
        catch (FormatException)
        {
            return false;
        }
    }

    /// <summary>
    /// Change panels
    /// </summary>
    public void MoveToRegisterWindow()
    {
        RegisterPanel.SetActive(true);
        LoginPanel.SetActive(false);
    }

    /// <summary>
    /// Change panels
    /// </summary>
    public void MoveToLoginWindow()
    {
        RegisterPanel.SetActive(false);
        LoginPanel.SetActive(true);
    }
}