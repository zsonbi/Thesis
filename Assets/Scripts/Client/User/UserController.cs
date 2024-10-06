using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using Config;
using System.Net.Mail;
using User;
using Thesis_backend.Data_Structures;

namespace User
{
    /// <summary>
    /// Handles the user actions such as registering and login
    /// </summary>
    public class UserController : ThreadSafeMonoBehaviour
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
        private GameObject registerPanel;

        /// <summary>
        /// Modal window to alert the user
        /// </summary>
        [SerializeField]
        private ModalWindow modalWindow;

        /// <summary>
        /// Start is called before the first frame update
        /// </summary>
        private void Start()
        {
            //Cap the fps to 60
            Application.targetFrameRate = 60;
            QualitySettings.vSyncCount = 0;

            //Check if the user is logged in, so the user doesn't need to log in if not necessary
            CheckLoggedIn();
        }

        /// <summary>
        /// Send a request to check if the user has logged in already
        /// </summary>
        private void CheckLoggedIn()
        {
            //If the user is still cached in the RAM move him to the main page
            if (UserData.Instance.LoggedIn)
            {
                MoveToMainScence();
                return;
            }

            CoroutineRunner.RunCoroutine(Server.SendGetRequest<Thesis_backend.Data_Structures.User>(ServerConfig.PATHFORCHECKLOGGEDIN, LoggedIn));
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

            CoroutineRunner.RunCoroutine(Server.SendPostRequest<Thesis_backend.Data_Structures.User>(ServerConfig.PATHFORLOGIN, userLoginRequest, LoggedIn, onFailedAction: ShowLoginError));
        }

        /// <summary>
        /// When the login request's was responded log the user in
        /// </summary>
        /// <param name="result">The server's response</param>
        private void LoggedIn(Thesis_backend.Data_Structures.User result)
        {
            UserData.Instance.Init(result);
            CoroutineRunner.RunCoroutine(MoveToMainScence());
        }

        /// <summary>
        /// Send a register request with the register panel's fields
        /// </summary>
        public void SendRegister()
        {
            TMPro.TMP_InputField[] fields = registerPanel.GetComponentsInChildren<TMPro.TMP_InputField>();
            //Validate the register inputs locally
            if (!IsValidEmail(fields[0].text))
            {
                modalWindow.Show("Register error", "This is not a valid email!");

                return;
            }
            if (fields[2].text != fields[3].text)
            {
                modalWindow.Show("Register error", "Passwords does not match!");
                return;
            }
            UserRequest userRequest = new UserRequest()
            {
                UserName = fields[1].text,
                Email = fields[0].text,
                Password = fields[2].text,
            };
            //Send the request
            CoroutineRunner.RunCoroutine(Server.SendPostRequest<Thesis_backend.Data_Structures.User>(ServerConfig.PATHFORREGISTER, userRequest, Registered, onFailedAction: ShowRegisterError));
        }

        /// <summary>
        /// Show the register error on the modal window
        /// </summary>
        /// <param name="content">What to show</param>
        private void ShowRegisterError(string content)
        {
            modalWindow.Show("Register error", content);
        }

        /// <summary>
        /// Show the login error on the modal window
        /// </summary>
        /// <param name="content">What to show</param>
        private void ShowLoginError(string content)
        {
            modalWindow.Show("Login error", content);
        }

        /// <summary>
        /// When the register request was responded successfully register the user and log him in
        /// </summary>
        /// <param name="loggedInUser">The registered user's data</param>
        private void Registered(Thesis_backend.Data_Structures.User loggedInUser)
        {
            UserData.Instance.Init(loggedInUser);
            CoroutineRunner.RunCoroutine(MoveToMainScence());
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
            registerPanel.SetActive(true);
            LoginPanel.SetActive(false);
        }

        /// <summary>
        /// Change panels
        /// </summary>
        public void MoveToLoginWindow()
        {
            registerPanel.SetActive(false);
            LoginPanel.SetActive(true);
        }
    }
}