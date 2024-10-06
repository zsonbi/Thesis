using Config;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using Thesis_backend.Data_Structures;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using MainPage;

namespace MainPage
{
    public class TaskDisplayHandler : ThreadSafeMonoBehaviour
    {
        [SerializeField]
        private TMP_Text taskNameLabel;

        [SerializeField]
        private TMP_Text taskIntervalsLabel;

        [SerializeField]
        private TMP_Text remainingTimeLabel;

        [SerializeField]
        private Button completeButton;

        private MainWindowController UIController;

        private TaskOpenPanelController taskOpenPanelController;

        public PlayerTask CurrentTask { get; private set; }

        public void Update()
        {
            if (CurrentTask.Completed)
            {
                UpdateTimeRemaining();
            }
        }

        public void InitValues(PlayerTask taskContainer, TaskOpenPanelController taskOpenPanelController, MainWindowController uIController)
        {
            this.CurrentTask = taskContainer;

            if (!CalculateIfCompleteable())
            {
                CompleteStateChange(true);
            }
            this.taskOpenPanelController = taskOpenPanelController;
            this.UIController = uIController;
            UpdateLabels();
        }

        public void CompleteTask()
        {
            WWWForm form = new WWWForm();

            CoroutineRunner.RunCoroutine(Server.SendPatchRequest<Thesis_backend.Data_Structures.PlayerTask>(ServerConfig.PATHFORTASKCOMPLETE(CurrentTask.ID), onComplete: TaskCompleted, onFailedAction: UIController.ShowTaskFail));
        }

        public void OpenEditor()
        {
            taskOpenPanelController.OpenUp(this.CurrentTask);
        }

        private void TaskCompleted(Thesis_backend.Data_Structures.PlayerTask result)
        {
            CompleteStateChange(true);
            CoroutineRunner.RunCoroutine(Server.SendGetRequest<Thesis_backend.Data_Structures.User>(ServerConfig.PATHFORCHECKLOGGEDIN, (Thesis_backend.Data_Structures.User u) => { UIController.UpdateUserData(u); UIController.SortingChanged(); }, onFailedAction: UIController.ShowTaskFail));
        }

        public void UpdateLabels()
        {
            taskNameLabel.text = CurrentTask.TaskName;
            taskIntervalsLabel.text = ((TaskIntervals)CurrentTask.PeriodRate).ToString();
            UpdateTimeRemaining();
        }

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
                UIController.SortingChanged();

                this.CurrentTask.ResetComplete();
                completeButton.gameObject.SetActive(true);
                return;
            }
        }

        private void UpdateTimeRemaining()
        {
            if (CurrentTask.Completed)
            {
                TimeSpan difference = TimeSpan.FromMinutes(CurrentTask.PeriodRate) - (DateTime.UtcNow - CurrentTask.LastCompleted);

                if (difference.TotalMinutes <= 0)
                {
                    CompleteStateChange(false);
                    return;
                }
                string formatted = "";
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

        private bool CalculateIfCompleteable()
        {
            TimeSpan difference = TimeSpan.FromMinutes(CurrentTask.PeriodRate) - (DateTime.UtcNow - CurrentTask.LastCompleted);
            return difference.TotalMinutes <= 0;
        }
    }
}