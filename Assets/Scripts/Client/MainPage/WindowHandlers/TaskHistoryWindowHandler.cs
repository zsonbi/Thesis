using Config;
using System.Collections.Generic;
using Thesis_backend.Data_Structures;
using UnityEngine;

namespace MainPage
{
    /// <summary>
    /// Task histories window controller
    /// </summary>
    public class TaskHistoryWindowHandler : ThreadSafeMonoBehaviour
    {
        /// <summary>
        /// The container where to put the task histories
        /// </summary>
        [SerializeField]
        private GameObject taskHistoriesParent;

        /// <summary>
        /// Reference to the modal window to alert the user for api fails
        /// </summary>
        [SerializeField]
        private ModalWindow modalWindow;

        /// <summary>
        /// Task history prefab
        /// </summary>
        [SerializeField]
        private GameObject taskHistoryItemPrefab;

        /// <summary>
        /// Shows the task history window
        /// </summary>
        public void Show()
        {
            this.gameObject.SetActive(true);
            LoadTaskHistories();
        }

        /// <summary>
        /// Hides the task history window
        /// </summary>
        public void Hide()
        {
            this.gameObject.SetActive(false);
        }

        /// <summary>
        /// Loads the task histories from the server
        /// </summary>
        public void LoadTaskHistories()
        {
            CoroutineRunner.RunCoroutine(Server.SendGetRequest<List<TaskHistory>>(ServerConfig.PATH_FOR_GET_TASK_HISTORIES, LoadedHistories, onFailedAction: ShowRequestFail));
        }

        /// <summary>
        /// Show the server response errors
        /// </summary>
        /// <param name="content">The server response</param>
        private void ShowRequestFail(string content)
        {
            modalWindow.Show("Task history error", content);
        }

        /// <summary>
        /// When the server response is recieved load the histories
        /// </summary>
        /// <param name="taskHistories"></param>
        private void LoadedHistories(List<TaskHistory> taskHistories)
        {
            //Protect from errors during testing
            try
            {
                //Delete the previous ones
                for (int i = 0; i < this.taskHistoriesParent.transform.childCount; i++)
                {
                    Destroy(this.taskHistoriesParent.transform.GetChild(i).gameObject);
                }
                this.taskHistoriesParent.transform.DetachChildren();

                foreach (var item in taskHistories)
                {
                    TaskHistoryHandler leaderboardItem = Instantiate(taskHistoryItemPrefab, taskHistoriesParent.transform).GetComponent<TaskHistoryHandler>();
                    leaderboardItem.Init(item);
                }
            }
            catch (MissingReferenceException)
            {
                throw;
            }
        }
    }
}