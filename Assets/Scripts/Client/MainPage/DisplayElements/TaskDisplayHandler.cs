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

public class TaskDisplayHandler : MonoBehaviour
{
    private static Color AVAILIBLE_FOR_COMPLETE_COLOR = new Color(70 / 255f, 1f, 0, 1f);
    private static Color DISABLED_FOR_COMPLETE_COLOR = new Color(73 / 255f, 59 / 255f, 71 / 255f, 1f);

    [SerializeField]
    private TMP_Text TaskNameLabel;

    [SerializeField]
    private TMP_Text TaskIntervalsLabel;

    [SerializeField]
    private TMP_Text RemainingTimeLabel;

    [SerializeField]
    private Button CompleteButton;

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

        StartCoroutine(Server.SendPatchRequest<Thesis_backend.Data_Structures.PlayerTask>(ServerConfig.PATHFORTASKCOMPLETE(CurrentTask.ID), onComplete: TaskCompleted, onFailedAction: UIController.ShowTaskFail));
    }

    public void OpenEditor()
    {
        taskOpenPanelController.OpenUp(this.CurrentTask);
    }

    private void TaskCompleted(Thesis_backend.Data_Structures.PlayerTask result)
    {
        CompleteStateChange(true);
        StartCoroutine(Server.SendGetRequest<Thesis_backend.Data_Structures.User>(ServerConfig.PATHFORCHECKLOGGEDIN, UIController.UpdateUserData, onFailedAction: UIController.ShowTaskFail));
    }

    public void UpdateLabels()
    {
        TaskNameLabel.text = CurrentTask.TaskName;
        TaskIntervalsLabel.text = ((TaskIntervals)CurrentTask.PeriodRate).ToString();
        UpdateTimeRemaining();
    }

    private void CompleteStateChange(bool state)
    {
        RemainingTimeLabel.gameObject.SetActive(state);
        if (state)
        {
            this.CurrentTask.Complete();
            CompleteButton.GetComponent<Image>().color = DISABLED_FOR_COMPLETE_COLOR;
            UpdateTimeRemaining();
        }
        else
        {
            this.CurrentTask.ResetComplete();
            CompleteButton.GetComponent<Image>().color = AVAILIBLE_FOR_COMPLETE_COLOR;
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

            string formatted = string.Format("{0} days {1:D2}:{2:D2}:{3:D2}",
            (int)difference.TotalDays, difference.Hours, difference.Minutes, difference.Seconds);

            RemainingTimeLabel.text = formatted;
        }
    }

    private bool CalculateIfCompleteable()
    {
        TimeSpan difference = TimeSpan.FromMinutes(CurrentTask.PeriodRate) - (DateTime.UtcNow - CurrentTask.LastCompleted);
        return difference.TotalMinutes <= 0;
    }
}