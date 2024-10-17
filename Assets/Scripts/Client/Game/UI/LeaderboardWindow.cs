using Config;
using DataTypes;
using System;
using System.Collections.Generic;
using Thesis_backend.Data_Structures;
using TMPro;
using UnityEngine;
using User;
using Utility;

namespace Game
{
    namespace UI
    {
        /// <summary>
        /// Handles the leaderboard display window
        /// </summary>
        public class LeaderboardWindow : ThreadSafeMonoBehaviour
        {
            /// <summary>
            /// Reference to where to put the items
            /// </summary>
            [SerializeField]
            private GameObject scoreParent;

            /// <summary>
            /// Drop down of how recent scores should be shown
            /// </summary>
            [SerializeField]
            private TMP_Dropdown filterTypeDropdown;

            /// <summary>
            /// Reference to the scene's modal window
            /// </summary>
            [SerializeField]
            private ModalWindow ModalWindow;

            /// <summary>
            /// The prefab of a single leaderboard item
            /// </summary>
            [SerializeField]
            private GameObject LeaderboardItemPrefab;

            /// <summary>
            /// Show the window
            /// </summary>
            public void Show()
            {
                this.gameObject.SetActive(true);
                LoadScores();
            }

            /// <summary>
            /// Hide the window
            /// </summary>
            public void Hide()
            {
                this.gameObject.SetActive(false);
            }

            /// <summary>
            /// Load the scores
            /// </summary>
            public void LoadScores()
            {
                DateTime filterSince = DateTime.Now;
                //Determine what date was selected
                switch ((LeaderboardFilterType)filterTypeDropdown.value)
                {
                    case LeaderboardFilterType.AllTime:
                        filterSince = DateTime.MinValue;
                        break;

                    case LeaderboardFilterType.ThisYear:
                        filterSince = DateTime.UtcNow.AddYears(-1);
                        break;

                    case LeaderboardFilterType.ThisMonth:
                        filterSince = DateTime.UtcNow.AddMonths(-1);
                        break;

                    case LeaderboardFilterType.ThisWeek:
                        filterSince = DateTime.UtcNow.AddDays(-7);
                        break;

                    case LeaderboardFilterType.Today:
                        filterSince = DateTime.UtcNow.AddDays(-1);
                        break;

                    default:
                        Debug.LogError("No such leaderboard filter type is known");
                        break;
                }

                CoroutineRunner.RunCoroutine(Server.SendGetRequest<List<GameScore>>(ServerConfig.PATH_FOR_GET_GAME_SCORES(filterSince), LoadedScores, onFailedAction: ShowRequestFail));
            }

            /// <summary>
            /// Show if the request failed in the modal
            /// </summary>
            /// <param name="content">The content to display</param>
            private void ShowRequestFail(string content)
            {
                ModalWindow.Show("Leaderboard error", content);
            }

            /// <summary>
            /// Callback after the scores were loaded
            /// </summary>
            /// <param name="gameScores">The game scores</param>
            private void LoadedScores(List<GameScore> gameScores)
            {
                if (scoreParent == null || this.scoreParent.transform == null)
                {
                    // Exit or handle the case when the Score parent is destroyed
                    Debug.LogWarning("Score parent has been destroyed or is missing.");
                    return;
                }

                //Delete the previous ones
                for (int i = 0; i < this.scoreParent.transform.childCount; i++)
                {
                    Destroy(this.scoreParent.transform.GetChild(i).gameObject);
                }
                this.scoreParent.transform.DetachChildren();

                foreach (var item in gameScores)
                {
                    LeaderboardItem leaderboardItem = Instantiate(LeaderboardItemPrefab, scoreParent.transform).GetComponent<LeaderboardItem>();
                    leaderboardItem.Init(item.OwnerName, item.Score);
                }
            }
        }
    }
}