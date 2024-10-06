using Config;
using System;
using Thesis_backend.Data_Structures;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MainPage
{
    public class TaskOpenPanelController : ThreadSafeMonoBehaviour
    {
        [HideInInspector]
        public PlayerTask CurrentTask { get; private set; } = new PlayerTask() { ID = -1 };

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
        private MainWindowController uIController;

        [SerializeField]
        private Button deleteTaskButton;

        private bool isNewTask = false;

        public static readonly int[] TASKINTERVALS = { 60, 120, 240, 1440, 2880, 10080, 20160, 40320 };

        private PlayerTask playerTaskOnOpen;

        public void OpenUp()
        {
            if (this.CurrentTask is not null)
            {
                OpenUp(type: this.CurrentTask.TaskType ? TaskType.BadHabit : TaskType.GoodTask);
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
                this.CurrentTask = taskContainer;
                this.isNewTask = false;
                this.playerTaskOnOpen = new PlayerTask(taskContainer);
            }
            else
            {
                this.CurrentTask = new PlayerTask() { ID = -1 };
                this.CurrentTask.ChangeType(type);
                this.isNewTask = true;
                this.playerTaskOnOpen = null;
            }

            taskNameInput.text = this.CurrentTask.TaskName;

            descriptionInput.text = this.CurrentTask.Description;

            for (int i = 0; i < TASKINTERVALS.Length; i++)
            {
                if (TASKINTERVALS[i] == (int)this.CurrentTask.PeriodRate)
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
            this.CurrentTask?.ChangeType(TaskType.BadHabit);
            UpdateButtons();

            uIController.LoadBadHabits(false);
        }

        public void MakeItGoodTask()
        {
            this.CurrentTask?.ChangeType(TaskType.GoodTask);
            UpdateButtons();

            uIController.LoadGoodTasks(false);
        }

        public void Cancel()
        {
            this.TaskClosedEventHandler?.Invoke(this, new TaskClosedEventArgs(false));
            tasksOpenPanel.SetActive(false);
            if (this.CurrentTask.ID != -1)
            {
                if (playerTaskOnOpen is not null)
                {
                    CurrentTask.UpdateValues(playerTaskOnOpen);
                    if (playerTaskOnOpen.TaskType)
                    {
                        uIController.LoadBadHabits();
                    }
                    else
                    {
                        uIController.LoadGoodTasks();
                    }

                    this.CurrentTask = playerTaskOnOpen;
                }
            }
            this.CurrentTask = new PlayerTask() { ID = -1 };
        }

        public void Save()
        {
            CollectFromFields();

            TaskRequest taskRequest = new TaskRequest()
            {
                TaskName = this.CurrentTask.TaskName,
                Description = this.CurrentTask.Description,
                PeriodRate = (int)CurrentTask.PeriodRate,
                TaskType = Convert.ToBoolean(CurrentTask.TaskType)
            };

            if (isNewTask)
            {
                CoroutineRunner.RunCoroutine(Server.SendPostRequest<Thesis_backend.Data_Structures.PlayerTask>(ServerConfig.PATHFORTASKCREATE, taskRequest, SavedTask, onFailedAction: uIController.ShowTaskFail));
            }
            else
            {
                CoroutineRunner.RunCoroutine(Server.SendPatchRequest<Thesis_backend.Data_Structures.PlayerTask>(ServerConfig.PATHFORTASKUPDATE(CurrentTask.ID), taskRequest, SavedTask, onFailedAction: uIController.ShowTaskFail));
            }
        }

        private void SavedTask(Thesis_backend.Data_Structures.PlayerTask savedTask)
        {
            this.TaskClosedEventHandler?.Invoke(this, new TaskClosedEventArgs(true));
            tasksOpenPanel.SetActive(false);
            bool isNewTask = CurrentTask.ID == -1;

            this.CurrentTask = savedTask;

            if (isNewTask)
            {
                uIController.CreateTask(this.CurrentTask);
            }
            else
            {
                uIController.UpdateTaskLabel(CurrentTask.ID);
            }
            uIController.SortingChanged();

            this.CurrentTask = new PlayerTask() { ID = -1 };
        }

        public void DeleteTask()
        {
            CoroutineRunner.RunCoroutine(Server.SendDeleteRequest<string>(ServerConfig.PATHFORTASKDELETE(CurrentTask.ID), onComplete: DeletedTask, onFailedAction: uIController.ShowTaskFail));
        }

        private void DeletedTask(string result)
        {
            uIController.RemoveTask(CurrentTask.ID);
            Cancel();
        }

        private void CollectFromFields()
        {
            string taskName = taskNameInput.text;
            string description = descriptionInput.text;
            TaskIntervals taskInterval = (TaskIntervals)TASKINTERVALS[taskIntervals.value];

            CurrentTask.UpdateValues(taskName, CurrentTask.TaskType, taskInterval, description);
        }

        private void UpdateButtons()
        {
            if (!this.CurrentTask.TaskType)
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
}