using Config;
using DataTypes;
using System;
using Thesis_backend.Data_Structures;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utility;

namespace MainPage
{
    /// <summary>
    /// Task prefab display handler
    /// </summary>
    public class TaskDisplayHandler : ThreadSafeMonoBehaviour
    {
        /// <summary>
        /// The displayed task's name
        /// </summary>
        [SerializeField]
        private TMP_Text taskNameLabel;

        /// <summary>
        /// The displayed task's interval
        /// </summary>
        [SerializeField]
        private TMP_Text taskIntervalsLabel;

        /// <summary>
        /// The displayed task's remaining time until it can be completed again
        /// </summary>
        [SerializeField]
        private TMP_Text remainingTimeLabel;

        /// <summary>
        /// Complete button for the displayed task
        /// </summary>
        [SerializeField]
        private Button completeButton;

        /// <summary>
        /// Reference to the main window controller
        /// </summary>
        private MainWindowController mainWindowController;

        /// <summary>
        /// Reference to the task edit/create panel
        /// </summary>
        private TaskOpenPanelController taskOpenPanelController;

        /// <summary>
        /// The task which is being displayed
        /// </summary>
        public PlayerTask CurrentTask { get; private set; }

        /// <summary>
        /// Called every frame if it has been completed update the remaining time
        /// </summary>
        public void Update()
        {
            if (CurrentTask.Completed)
            {
                UpdateTimeRemaining();
            }
        }

        /// <summary>
        /// Init it with the task to display and references to the controls
        /// </summary>
        /// <param name="playerTask">The task to display</param>
        /// <param name="taskOpenPanelController">Reference to the task panel</param>
        /// <param name="uIController">Reference to the </param>
        public void InitValues(PlayerTask playerTask, TaskOpenPanelController taskOpenPanelController, MainWindowController uIController)
        {
            this.CurrentTask = playerTask;

            if (!CalculateIfCompleteable())
            {
                CompleteStateChange(true);
            }
            this.taskOpenPanelController = taskOpenPanelController;
            this.mainWindowController = uIController;
            UpdateLabels();
        }

        /// <summary>
        /// Complete the task and send this to the server
        /// </summary>
        public void CompleteTask()
        {
            CoroutineRunner.RunCoroutine(Server.SendPatchRequest<Thesis_backend.Data_Structures.PlayerTask>(ServerConfig.PATHFORTASKCOMPLETE(CurrentTask.ID), onComplete: TaskCompleted, onFailedAction: mainWindowController.ShowTaskFail));
        }

        /// <summary>
        /// Open the task editor
        /// </summary>
        public void OpenEditor()
        {
            taskOpenPanelController.OpenUp(this.CurrentTask);
        }

        /// <summary>
        /// When the complete acknowledge were recieved from the server update the display
        /// </summary>
        /// <param name="result">The completed task</param>
        private void TaskCompleted(Thesis_backend.Data_Structures.PlayerTask result)
        {
            CompleteStateChange(true);
            //Update the user's task score
            CoroutineRunner.RunCoroutine(Server.SendGetRequest<Thesis_backend.Data_Structures.User>
                (ServerConfig.PATHFORCHECKLOGGEDIN,
                (Thesis_backend.Data_Structures.User u) => { mainWindowController.UpdateUserData(u); if (mainWindowController.TaskSortType == TaskSortType.Available) mainWindowController.SortingChanged(); },
                onFailedAction: mainWindowController.ShowTaskFail));
        }

        /// <summary>
        /// Update the task's labels
        /// </summary>
        public void UpdateLabels()
        {
            taskNameLabel.text = CurrentTask.TaskName;
            taskIntervalsLabel.text = ((TaskIntervals)CurrentTask.PeriodRate).ToString();
            UpdateTimeRemaining();
        }

        /// <summary>
        /// When the complete state is changed update the display to match it
        /// </summary>
        /// <param name="state">true-task is being completed, false - task is now available again</param>
        private void CompleteStateChange(bool state)
        {
            remainingTimeLabel.gameObject.SetActive(state);
            if (state)
            {
                this.CurrentTask.Complete();
                completeButton.gameObject.SetActive(false);
                UpdateTimeRemaining();
            }
            else
            {
                if (mainWindowController.TaskSortType == TaskSortType.Available)
                {
                    mainWindowController.SortingChanged();
                }
                this.CurrentTask.ResetComplete();
                completeButton.gameObject.SetActive(true);
                return;
            }
        }

        /// <summary>
        /// Update the time remaining until the task can be completed again
        /// </summary>
        private void UpdateTimeRemaining()
        {
            //Check if this is needed even
            if (CurrentTask.Completed)
            {
                //Calculate how much time till it can be completed again
                TimeSpan difference = TimeSpan.FromMinutes(CurrentTask.PeriodRate) - (DateTime.UtcNow - CurrentTask.LastCompleted);
                //If it can be completed again update the layout
                if (difference.TotalMinutes <= 0)
                {
                    CompleteStateChange(false);
                    return;
                }
                string formatted = "";
                //If it is less then a day away hide the day part
                if ((int)difference.TotalDays > 0)
                {
                    formatted = string.Format("{0} days {1:D2}:{2:D2}:{3:D2}", (int)difference.TotalDays, difference.Hours, difference.Minutes, difference.Seconds);
                }
                else
                {
                    formatted = string.Format("{0:D2}:{1:D2}:{2:D2}", difference.Hours, difference.Minutes, difference.Seconds);
                }

                remainingTimeLabel.text = formatted;
            }
        }

        /// <summary>
        /// Calculate if the task can be completed again
        /// </summary>
        /// <returns>true-can be completed, false-can't be completed</returns>
        private bool CalculateIfCompleteable()
        {
            TimeSpan difference = TimeSpan.FromMinutes(CurrentTask.PeriodRate) - (DateTime.UtcNow - CurrentTask.LastCompleted);
            return difference.TotalMinutes <= 0;
        }
    }
}