using Config;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using Thesis_backend.Data_Structures;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using User;

public class TaskOpenPanelController : MonoBehaviour
{
    [HideInInspector]
    public PlayerTask TaskContainer = new PlayerTask();

    [HideInInspector]
    public EventHandler<TaskClosedEventArgs> TaskClosedEventHandler;

    [SerializeField]
    private GameObject tasksOpenPanel;

    [SerializeField]
    private TMP_InputField taskNameInput;

    [SerializeField]
    private TMP_Dropdown taskIntervals;

    [SerializeField]
    private TMP_InputField descriptionInput;

    [SerializeField]
    private Button goodTaskButton;

    [SerializeField]
    private Button badHabitButton;

    [SerializeField]
    private UIController UIController;

    [SerializeField]
    private Button deleteTaskButton;

    private bool isNewTask = false;

    private static readonly int[] TASKINTERVALS = { 60, 120, 240, 1440, 2880, 10080, 20160, 40320 };

    private void Awake()
    {
    }

    // Start is called before the first frame update
    private void Start()
    {
    }

    public void OpenUp()
    {
        if (this.TaskContainer is not null)
        {
            OpenUp(type: this.TaskContainer.TaskType ? TaskType.BadHabit : TaskType.GoodTask);
        }
        else
        {
            OpenUp(null);
        }
    }

    public void OpenUp(PlayerTask taskContainer = null, TaskType type = TaskType.GoodTask)
    {
        if (taskContainer is not null)
        {
            this.TaskContainer = taskContainer;
            this.isNewTask = false;
        }
        else
        {
            this.TaskContainer = new PlayerTask();
            this.TaskContainer.ChangeType(type);
            this.isNewTask = true;
        }

        taskNameInput.text = this.TaskContainer.TaskName;

        descriptionInput.text = this.TaskContainer.Description;

        for (int i = 0; i < TASKINTERVALS.Length; i++)
        {
            if (TASKINTERVALS[i] == (int)this.TaskContainer.PeriodRate)
            {
                taskIntervals.SetValueWithoutNotify(i);
                break;
            }
        }

        deleteTaskButton.gameObject.SetActive(!isNewTask);

        tasksOpenPanel.SetActive(true);

        UpdateButtons();
    }

    public void MakeItBadHabit()
    {
        this.TaskContainer?.ChangeType(TaskType.BadHabit);
        UpdateButtons();

        UIController.LoadBadHabits(false);
    }

    public void MakeItGoodTask()
    {
        this.TaskContainer?.ChangeType(TaskType.GoodTask);
        UpdateButtons();

        UIController.LoadGoodTasks(false);
    }

    public void Cancel()
    {
        this.TaskClosedEventHandler?.Invoke(this, new TaskClosedEventArgs(false));
        tasksOpenPanel.SetActive(false);
        this.TaskContainer = new PlayerTask();
    }

    public void Save()
    {
        CollectFromFields();

        TaskRequest taskRequest = new TaskRequest()
        {
            TaskName = this.TaskContainer.TaskName,
            Description = this.TaskContainer.Description,
            PeriodRate = (int)TaskContainer.PeriodRate,
            TaskType = Convert.ToBoolean(TaskContainer.TaskType)
        };

        if (isNewTask)
        {
            StartCoroutine(Server.SendPostRequest<Thesis_backend.Data_Structures.PlayerTask>(ServerConfig.PATHFORTASKCREATE, taskRequest, SavedTask));
        }
        else
        {
            StartCoroutine(Server.SendPatchRequest<Thesis_backend.Data_Structures.PlayerTask>(ServerConfig.PATHFORTASKUPDATE(TaskContainer.ID), taskRequest, SavedTask));
        }
    }

    private void SavedTask(Thesis_backend.Data_Structures.PlayerTask savedTask)
    {
        this.TaskClosedEventHandler?.Invoke(this, new TaskClosedEventArgs(true));
        tasksOpenPanel.SetActive(false);
        bool isNewTask = TaskContainer.ID == -1;

        this.TaskContainer = savedTask;

        if (isNewTask)
        {
            UIController.CreateTask(this.TaskContainer);
        }
        else
        {
            UIController.UpdateTask(TaskContainer.ID);
        }
        this.TaskContainer = new PlayerTask();
    }

    public void DeleteTask()
    {
        StartCoroutine(Server.SendDeleteRequest<string>(ServerConfig.PATHFORTASKDELETE(TaskContainer.ID)));
    }

    private void DeletedTask(Dictionary<string, string> result)
    {
        if (result.ContainsKey("res") && result["res"] == "success")
        {
            Debug.Log($"Deleted the id:{TaskContainer.ID} task");

            UIController.RemoveTask(TaskContainer.ID);
        }
        else
        {
            UIController.ModalWindow.Show("Error", "Failed to delete the given task");
        }
        Cancel();
    }

    private void CollectFromFields()
    {
        string taskName = taskNameInput.text;
        string description = descriptionInput.text;
        TaskIntervals taskInterval = (TaskIntervals)TASKINTERVALS[taskIntervals.value];

        TaskContainer.UpdateValues(taskName, TaskContainer.TaskType, taskInterval, description);
    }

    private void UpdateButtons()
    {
        if (!this.TaskContainer.TaskType)
        {
            goodTaskButton.image.color = new Color(134f / 255f, 1f, 90f / 255f, 1.0f);
            badHabitButton.image.color = new Color(185 / 255f, 185f / 255f, 185f / 255f, 1f);
        }
        else
        {
            goodTaskButton.image.color = new Color(185f / 255f, 185f / 255f, 185f / 255f, 1f);
            badHabitButton.image.color = new Color(248f / 255f, 34f / 255f, 44f / 255f, 1f);
        }
    }
}