using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using User;
using TMPro;
using Config;
using Thesis_backend.Data_Structures;
using DataTypes;
using System.Linq;
using System.Threading.Tasks;
using Utility;

namespace MainPage
{
    /// <summary>
    /// Handles the main window's controls
    /// </summary>
    public class MainWindowController : ThreadSafeMonoBehaviour
    {
        /// <summary>
        /// Text to display the username of the currently logged in user
        /// </summary>
        [SerializeField]
        private TMP_Text usernameText;

        /// <summary>
        /// Text to display the current task score of the player
        /// </summary>
        [SerializeField]
        private TMP_Text currentTaskScoreText;

        /// <summary>
        /// Container to load the tasks into
        /// </summary>
        [SerializeField]
        private GameObject taskParent;

        /// <summary>
        /// Task prefab to load for each task
        /// </summary>
        [SerializeField]
        private GameObject taskPrefab;

        /// <summary>
        /// Task open panel for task edit and new task creation
        /// </summary>
        [SerializeField]
        private TaskOpenPanelController taskOpenPanelController;

        /// <summary>
        /// Modal window for api fail feedback
        /// </summary>
        [SerializeField]
        private ModalWindow modalWindow;

        /// <summary>
        /// Task sort type dropdown
        /// </summary>
        [SerializeField]
        private TMP_Dropdown taskSortDropdown;

        /// <summary>
        /// Currently loaded tasks
        /// </summary>
        public Dictionary<long, TaskDisplayHandler> Tasks { get; private set; } = new Dictionary<long, TaskDisplayHandler>();

        /// <summary>
        /// Currently what are the task being sorted by
        /// </summary>
        public TaskSortType TaskSortType => (TaskSortType)taskSortDropdown.value;

        /// <summary>
        /// Start is called before the first frame update
        /// </summary>
        private async void Start()
        {
            //Wait for the user to be logged in
            while (!UserData.Instance.LoggedIn)
            {
                await Task.Delay(100);
            }
            usernameText.text = UserData.Instance.Username;
            currentTaskScoreText.text = UserData.Instance.CurrentTaskScore.ToString();
            LoadTasks();
        }

        /// <summary>
        /// Update the user with the updated value
        /// </summary>
        /// <param name="user">User data recieved from the server</param>
        public void UpdateUserData(Thesis_backend.Data_Structures.User user)
        {
            UserData.Instance.Init(user);
            currentTaskScoreText.text = UserData.Instance.CurrentTaskScore.ToString();
        }

        /// <summary>
        /// Remove the task with the given id
        /// </summary>
        /// <param name="taskId">Task id to remove</param>
        public void RemoveTask(long taskId)
        {
            //Making sure that there is such task id
            if (Tasks.ContainsKey(taskId))
            {
                TaskDisplayHandler TaskToRemove = Tasks[taskId];
                Tasks.Remove(taskId);
                Destroy(TaskToRemove.gameObject);
            }
        }

        /// <summary>
        /// Update the task prebab's label
        /// </summary>
        /// <param name="taskId">The id of the task to change</param>
        public void UpdateTaskLabel(long taskId)
        {
            //Making sure that there is such task id
            if (Tasks.ContainsKey(taskId))
            {
                Tasks[taskId].UpdateLabels();
            }
        }

        /// <summary>
        /// Refreshes the tasks with the proper sorting order
        /// </summary>
        public void SortingChanged()
        {
            //Create a temporary dictionary which will store the sorted values
            Dictionary<long, TaskDisplayHandler> newTasks = new Dictionary<long, TaskDisplayHandler>();
            //Reorder the tasks with the appropriate sorting predicate
            switch ((TaskSortType)taskSortDropdown.value)
            {
                case TaskSortType.Added:
                    foreach (var item in Tasks.OrderBy(x => x.Key))
                    {
                        newTasks.Add(item.Key, item.Value);
                    }
                    break;

                case TaskSortType.Name:
                    foreach (var item in Tasks.OrderBy(x => x.Value.CurrentTask.TaskName))
                    {
                        newTasks.Add(item.Key, item.Value);
                    }
                    break;

                case TaskSortType.Type:
                    foreach (var item in Tasks.OrderBy(x => x.Value.CurrentTask.PeriodRate))
                    {
                        newTasks.Add(item.Key, item.Value);
                    }
                    break;

                case TaskSortType.Available:
                    foreach (var item in Tasks.OrderBy(x => x.Value.CurrentTask.Completed))
                    {
                        newTasks.Add(item.Key, item.Value);
                    }
                    break;

                default:
                    Debug.LogError("No such sorting way is configured");
                    break;
            }
            //Override the previous order
            Tasks = newTasks;
            //Reload the tasks
            if (taskOpenPanelController.CurrentTask.TaskType)
            {
                LoadBadHabits();
            }
            else
            {
                LoadGoodTasks();
            }
        }

        /// <summary>
        /// Load the good tasks
        /// </summary>
        /// <param name="updateDisplay">Should it also update the display (make the open panel display good task)</param>
        public void LoadGoodTasks(bool updateDisplay = true)
        {
            //Protect from errors during testing
            try
            {
                //Iterate the tasks
                foreach (var task in Tasks)
                {
                    //Enable the good tasks and disable the bad ones
                    if (!task.Value.CurrentTask.TaskType)
                    {
                        task.Value.gameObject.SetActive(true);
                        task.Value.gameObject.transform.SetAsLastSibling();
                    }
                    else
                    {
                        task.Value.gameObject.SetActive(false);
                    }
                }
                taskParent.GetComponentInChildren<TMP_Text>().text = "Good tasks";
                if (updateDisplay)
                    taskOpenPanelController.MakeItGoodTask();
            }
            catch (MissingReferenceException)
            {
            }
        }

        /// <summary>
        /// Load the good habits
        /// </summary>
        /// <param name="updateDisplay">Should it also update the display (make the open panel display bad habit)</param>
        public void LoadBadHabits(bool updateDisplay = true)
        {
            //Protect from errors during testing
            try
            {
                //Enable the bad habits and disable the good ones
                foreach (var task in Tasks)
                {
                    if (task.Value.CurrentTask.TaskType)
                    {
                        task.Value.gameObject.SetActive(true);
                        task.Value.gameObject.transform.SetAsLastSibling();
                    }
                    else
                    {
                        task.Value.gameObject.SetActive(false);
                    }
                }
                taskParent.GetComponentInChildren<TMP_Text>().text = "Bad habits";
                if (updateDisplay)
                    taskOpenPanelController.MakeItBadHabit();
            }
            catch (MissingReferenceException)
            {
            }
        }

        /// <summary>
        /// Creates a new task prefab to display
        /// </summary>
        /// <param name="taskContainer">The task to display</param>
        /// <returns>The created gameobject</returns>
        public GameObject CreateTask(PlayerTask taskContainer)
        {
            //Protect from errors during testing
            try
            {
                GameObject task = Instantiate(taskPrefab, taskParent.transform);
                TaskDisplayHandler taskComponent = task.GetComponent<TaskDisplayHandler>();
                taskComponent.InitValues(taskContainer, taskOpenPanelController, this);
                Tasks.Add(taskContainer.ID, taskComponent);
                return task;
            }
            catch (MissingReferenceException)
            {
                Debug.LogWarning("Task create missing reference");
                return null;
            }
        }

        /// <summary>
        /// Load the tasks from the server
        /// </summary>
        private void LoadTasks()
        {
            CoroutineRunner.RunCoroutine(Server.SendGetRequest<List<Thesis_backend.Data_Structures.PlayerTask>>(ServerConfig.PATHFORTASKSQUERY, CreateTaskPrefabs, onFailedAction: ShowRequestFail));
        }

        /// <summary>
        /// Show the server request fail in the modal window
        /// </summary>
        /// <param name="content">The server response</param>
        private void ShowRequestFail(string content)
        {
            modalWindow.Show("Request fail", content);
        }

        /// <summary>
        /// Show the task request fail in the modal window
        /// </summary>
        /// <param name="content">The server response</param>
        public void ShowTaskFail(string content)
        {
            modalWindow.Show("Task error", content);
        }

        /// <summary>
        /// Loads the game scene
        /// </summary>
        public void LoadGameScene()
        {
            SceneManager.LoadScene("GameScene", LoadSceneMode.Single);
        }

        /// <summary>
        /// Create the task prefabs
        /// </summary>
        /// <param name="requestResult">Collection of tasks from the server</param>
        private void CreateTaskPrefabs(List<Thesis_backend.Data_Structures.PlayerTask> requestResult)
        {
            foreach (var item in requestResult)
            {
                CreateTask(item);
            }
            LoadGoodTasks();
        }
    }
}