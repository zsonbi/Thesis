using Config;
using System;
using Thesis_backend.Data_Structures;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MainPage
{
    /// <summary>
    /// Controller for displaying, editing and creating tasks
    /// </summary>
    public class TaskOpenPanelController : ThreadSafeMonoBehaviour
    {
        /// <summary>
        /// The current task which is being displayed/edited
        /// </summary>
        [HideInInspector]
        public PlayerTask CurrentTask { get; private set; } = new PlayerTask() { ID = -1 };

        /// <summary>
        /// Event for when the window is closed
        /// </summary>
        [HideInInspector]
        public EventHandler<TaskClosedEventArgs> TaskClosedEventHandler;

        /// <summary>
        /// Input for the task's name
        /// </summary>
        [SerializeField]
        private TMP_InputField taskNameInput;

        /// <summary>
        /// Input dropdown for the task intervals
        /// </summary>
        [SerializeField]
        private TMP_Dropdown taskIntervals;

        /// <summary>
        /// Input for the task's description
        /// </summary>
        [SerializeField]
        private TMP_InputField descriptionInput;

        /// <summary>
        /// Make the task good
        /// </summary>
        [SerializeField]
        private Button goodTaskButton;

        /// <summary>
        /// Make the task bad
        /// </summary>
        [SerializeField]
        private Button badHabitButton;

        /// <summary>
        /// Reference to the ui controller
        /// </summary>
        [SerializeField]
        private MainWindowController uIController;

        /// <summary>
        /// Delete button for the task
        /// </summary>
        [SerializeField]
        private Button deleteTaskButton;

        /// <summary>
        /// Is the task a new one
        /// </summary>
        private bool isNewTask = false;

        /// <summary>
        /// Task intervals
        /// </summary>
        public static readonly int[] TASKINTERVALS = { 60, 120, 240, 1440, 2880, 10080, 20160, 40320 };

        /// <summary>
        /// What was the task when the task was opened
        /// so we can revert back on cancel
        /// </summary>
        private PlayerTask playerTaskOnOpen;

        /// <summary>
        /// Open up the panel
        /// </summary>
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

        /// <summary>
        /// Open's the panel with the task's values added also
        /// </summary>
        /// <param name="taskContainer">The task to load</param>
        /// <param name="type">The type of the task</param>
        public void OpenUp(PlayerTask taskContainer = null, TaskType type = TaskType.GoodTask)
        {
            try
            {


                //If the task is null create a dummy otherwise load it
                if (taskContainer is not null)
                {
                    this.CurrentTask = taskContainer;
                    this.isNewTask = false;
                    //Create a deep copy
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
                //Show/Hide the delete button
                deleteTaskButton.gameObject.SetActive(!isNewTask);
                this.gameObject.SetActive(true);
                UpdateButtons();
            }
            catch (MissingReferenceException)
            {
                Debug.LogWarning("Task open panel had a destroyed game object");
            }
        }

        /// <summary>
        /// Make this task a bad habit
        /// </summary>
        public void MakeItBadHabit()
        {
            this.CurrentTask?.ChangeType(TaskType.BadHabit);
            UpdateButtons();

            uIController.LoadBadHabits(false);
        }

        /// <summary>
        /// Make it a good task
        /// </summary>
        public void MakeItGoodTask()
        {
            this.CurrentTask?.ChangeType(TaskType.GoodTask);
            UpdateButtons();

            uIController.LoadGoodTasks(false);
        }

        /// <summary>
        /// Cancel this edit/task creation
        /// </summary>
        public void Cancel()
        {
            try
            {


                //If it wasn't a new task revert it to the previous state
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
                this.gameObject.SetActive(false);
            }
            catch (MissingReferenceException)
            {
                Debug.LogWarning("Missing reference task open panel in cancel");
            }
        }

        /// <summary>
        /// Save the task to the server
        /// </summary>
        public void Save()
        {
            //Collect the values from the inputs
            CollectFromFields();

            TaskRequest taskRequest = new TaskRequest()
            {
                TaskName = this.CurrentTask.TaskName,
                Description = this.CurrentTask.Description,
                PeriodRate = (int)CurrentTask.PeriodRate,
                TaskType = Convert.ToBoolean(CurrentTask.TaskType)
            };
            //If it is a new one call the create endpoint for the task otherwise the update one
            if (isNewTask)
            {
                CoroutineRunner.RunCoroutine(Server.SendPostRequest<Thesis_backend.Data_Structures.PlayerTask>(ServerConfig.PATHFORTASKCREATE, taskRequest, SavedTask, onFailedAction: uIController.ShowTaskFail));
            }
            else
            {
                CoroutineRunner.RunCoroutine(Server.SendPatchRequest<Thesis_backend.Data_Structures.PlayerTask>(ServerConfig.PATHFORTASKUPDATE(CurrentTask.ID), taskRequest, SavedTask, onFailedAction: uIController.ShowTaskFail));
            }
        }

        /// <summary>
        /// When the task was stored on the server show it also visually and close the panel
        /// </summary>
        /// <param name="savedTask">The Task which was recieved from the server</param>
        private void SavedTask(Thesis_backend.Data_Structures.PlayerTask savedTask)
        {
            this.TaskClosedEventHandler?.Invoke(this, new TaskClosedEventArgs(true));
            this.gameObject.SetActive(false);
            this.CurrentTask = savedTask;
            //If it was a new task create it's prefab otherwise just update it
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

        /// <summary>
        /// Send a delete task request to the server
        /// </summary>
        public void DeleteTask()
        {
            CoroutineRunner.RunCoroutine(Server.SendDeleteRequest<string>(ServerConfig.PATHFORTASKDELETE(CurrentTask.ID), onComplete: DeletedTask, onFailedAction: uIController.ShowTaskFail));
        }

        /// <summary>
        /// After the task were deleted from the server delete it also from the client
        /// </summary>
        /// <param name="result">The result of the deletion</param>
        private void DeletedTask(string result)
        {
            uIController.RemoveTask(CurrentTask.ID);
            Cancel();
        }

        /// <summary>
        /// Collect the values from the inputs
        /// </summary>
        private void CollectFromFields()
        {
            string taskName = taskNameInput.text;
            string description = descriptionInput.text;
            TaskIntervals taskInterval = (TaskIntervals)TASKINTERVALS[taskIntervals.value];
            CurrentTask.UpdateValues(taskName, CurrentTask.TaskType, taskInterval, description);
        }

        /// <summary>
        /// Update the buttons with the appropriate color based on the current task
        /// </summary>
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