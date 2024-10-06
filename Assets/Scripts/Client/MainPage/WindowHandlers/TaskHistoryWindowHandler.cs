using Config;
using System;
using System.Collections.Generic;
using Thesis_backend.Data_Structures;
using TMPro;
using UnityEngine;
using User;

namespace MainPage
{
    public class TaskHistoryWindowHandler : ThreadSafeMonoBehaviour
    {
        [SerializeField]
        private GameObject taskHistoriesParent;

        [SerializeField]
        private ModalWindow modalWindow;

        [SerializeField]
        private GameObject taskHistoryItemPrefab;

        public void Show()
        {
            this.gameObject.SetActive(true);
            LoadTaskHistories();
        }

        public void Hide()
        {
            this.gameObject.SetActive(false);
        }

        public void LoadTaskHistories()
        {
            CoroutineRunner.RunCoroutine(Server.SendGetRequest<List<TaskHistory>>(ServerConfig.PATH_FOR_GET_TASK_HISTORIES, LoadedHistories, onFailedAction: ShowRequestFail));
        }

        private void ShowRequestFail(string content)
        {
            modalWindow.Show("Task history error", content);
        }

        private void LoadedHistories(List<TaskHistory> taskHistories)
        {
            if (taskHistoriesParent == null || this.taskHistoriesParent.transform == null)
            {
                // Exit or handle the case when the Score parent is destroyed
                Debug.LogWarning("Score parent has been destroyed or is missing.");
                return;
            }

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
    }
}