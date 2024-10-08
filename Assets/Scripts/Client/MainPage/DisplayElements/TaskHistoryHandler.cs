using DataTypes;
using Thesis_backend.Data_Structures;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MainPage
{
    /// <summary>
    /// Handles the task history display
    /// </summary>
    [RequireComponent(typeof(Image))]
    public class TaskHistoryHandler : ThreadSafeMonoBehaviour
    {
        /// <summary>
        /// Completed task's name
        /// </summary>
        [SerializeField]
        private TMP_Text taskNameText;

        /// <summary>
        /// Completed task's interval
        /// </summary>
        [SerializeField]
        private TMP_Text taskIntervalText;

        /// <summary>
        /// Completed task's complete date
        /// </summary>
        [SerializeField]
        private TMP_Text completeDateText;

        /// <summary>
        /// Init the task history's with the data from the server
        /// </summary>
        /// <param name="taskHistory">The task history form the server</param>
        public void Init(TaskHistory taskHistory)
        {
            taskNameText.text = taskHistory.CompletedTask.TaskName;
            //Format the date in (YYYY. MM. DD.)
            completeDateText.text = string.Format("{0}. {1:D2}. {2:D2}.", (int)taskHistory.Completed.Year, taskHistory.Completed.Month, taskHistory.Completed.Day);
            taskIntervalText.text = ((TaskIntervals)(taskHistory.CompletedTask.PeriodRate)).ToString();
            //Color it based on the type
            if (taskHistory.CompletedTask.TaskType)
            {
                this.GetComponent<Image>().color = new Color(83f / 255f, 19f / 255f, 0);
            }
            else
            {
                this.GetComponent<Image>().color = new Color(0f / 255f, 83f / 255f, 0);
            }
        }
    }
}