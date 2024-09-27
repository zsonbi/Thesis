using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using Config;
using System.Net.Mail;
using User;
using Thesis_backend.Data_Structures;

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
        if (UserData.Instance.LoggedIn)
        {
            MoveToMainScence();
            return;
        }

        WWWForm form = new WWWForm();
        StartCoroutine(Server.SendGetRequest<Thesis_backend.Data_Structures.User>(ServerConfig.PATHFORCHECKLOGGEDIN, LoggedIn));
    }

    /// <summary>
    /// Send a login request with the login panel's fields
    /// </summary>
    public void SendLogin()
    {
        TMPro.TMP_InputField[] fields = LoginPanel.GetComponentsInChildren<TMPro.TMP_InputField>();

        UserLoginRequest userLoginRequest = new UserLoginRequest()
        {
            UserIdentification = fields[0].text,
            Password = fields[1].text,
        };

        StartCoroutine(Server.SendPostRequest<Thesis_backend.Data_Structures.User>(ServerConfig.PATHFORLOGIN, userLoginRequest, LoggedIn, onFailedAction: ShowLoginError));
    }

    /// <summary>
    /// When the login request's was responded log the user in
    /// </summary>
    /// <param name="result">The server's response</param>
    private void LoggedIn(Thesis_backend.Data_Structures.User result)
    {
        UserData.Instance.Init(result);
        StartCoroutine(MoveToMainScence());
    }

    /// <summary>
    /// Send a register request with the register panel's fields
    /// </summary>
    public void SendRegister()
    {
        TMPro.TMP_InputField[] fields = RegisterPanel.GetComponentsInChildren<TMPro.TMP_InputField>();

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
        UserRequest userRequest = new UserRequest()
        {
            UserName = fields[1].text,
            Email = fields[0].text,
            Password = fields[2].text,
        };

        StartCoroutine(Server.SendPostRequest<Thesis_backend.Data_Structures.User>(ServerConfig.PATHFORREGISTER, userRequest, Registered, onFailedAction: ShowRegisterError));
    }

    private void ShowRegisterError(string content)
    {
        ModalWindow.Show("Register error", content);
    }

    private void ShowLoginError(string content)
    {
        ModalWindow.Show("Login error", content);
    }

    /// <summary>
    /// When the login request's was responded register the user and log him in
    /// </summary>
    /// <param name="result">The server's response</param>
    private void Registered(Thesis_backend.Data_Structures.User loggedInUser)
    {
        UserData.Instance.Init(loggedInUser);
        StartCoroutine(MoveToMainScence());
    }

    /// <summary>
    /// Move from the user creation and login scene
    /// </summary>
    private IEnumerator MoveToMainScence()
    {
        AsyncOperation loading = SceneManager.LoadSceneAsync("MainScene", LoadSceneMode.Single);

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
            if (emailaddress == "")
            {
                return false;
            }

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