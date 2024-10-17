using Config;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using User;
using Utility;

namespace MainPage
{
    /// <summary>
    /// Handles the user's profile such as statistics and logout button
    /// </summary>
    public class ProfileHandler : ThreadSafeMonoBehaviour
    {
        /// <summary>
        /// Label for how many tasks had the user completed since registering
        /// </summary>
        [SerializeField]
        private TMP_Text totalTaskCount;

        /// <summary>
        /// Label for how many good tasks had the user completed since registering
        /// </summary>
        [SerializeField]
        private TMP_Text goodTaskCount;

        /// <summary>
        /// Label for how many bad tasks had the user completed since registering
        /// </summary>
        [SerializeField]
        private TMP_Text badTaskCount;

        /// <summary>
        /// Label for how many task scores did the user accumulate since registering
        /// </summary>
        [SerializeField]
        private TMP_Text totalScore;

        /// <summary>
        /// Shows the profile window
        /// </summary>
        public void Show()
        {
            this.gameObject.SetActive(true);
            this.totalTaskCount.text = (UserData.Instance.CompletedGoodTasks + UserData.Instance.CompletedBadTasks).ToString();
            this.badTaskCount.text = UserData.Instance.CompletedBadTasks.ToString();
            this.goodTaskCount.text = UserData.Instance.CompletedGoodTasks.ToString();
            this.totalScore.text = UserData.Instance.TotalScore.ToString();
        }

        /// <summary>
        /// Hides the profile window
        /// </summary>
        public void Hide()
        {
            this.gameObject.SetActive(false);
        }

        /// <summary>
        /// Send a login request with the login panel's fields
        /// </summary>
        public void SendLogout()
        {
            CoroutineRunner.RunCoroutine(Server.SendDeleteRequest<string>(ServerConfig.PATHFORLOGOUT, LoggedOut));
        }

        /// <summary>
        /// When the logout was a success
        /// </summary>
        /// <param name="result">The server's response</param>
        private void LoggedOut(string result)
        {
            UserData.Instance.Logout();

            CoroutineRunner.RunCoroutine(MoveToLoginScene());
        }

        /// <summary>
        /// Move to the login screen after the user has logged out
        /// </summary>
        private IEnumerator MoveToLoginScene()
        {
            AsyncOperation loading = SceneManager.LoadSceneAsync("LoginScene", LoadSceneMode.Single);

            while (!loading.isDone)
            {
                Debug.Log(loading.progress);

                yield return null;
            }
        }
    }
}