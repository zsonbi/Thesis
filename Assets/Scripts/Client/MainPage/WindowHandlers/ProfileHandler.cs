using Config;
using System;
using System.Collections;
using System.Collections.Generic;
using Thesis_backend.Data_Structures;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using User;

namespace MainPage
{
    public class ProfileHandler : ThreadSafeMonoBehaviour
    {
        [SerializeField]
        private TMP_Text totalTaskCount;

        [SerializeField]
        private TMP_Text goodTaskCount;

        [SerializeField]
        private TMP_Text badTaskCount;

        [SerializeField]
        private TMP_Text totalScore;

        public void Show()
        {
            this.gameObject.SetActive(true);
            LoadFromApi();
            this.totalTaskCount.text = (UserData.Instance.CompletedGoodTasks + UserData.Instance.CompletedBadTasks).ToString();
            this.badTaskCount.text = UserData.Instance.CompletedBadTasks.ToString();
            this.goodTaskCount.text = UserData.Instance.CompletedGoodTasks.ToString();
            this.totalScore.text = UserData.Instance.TotalScore.ToString();
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