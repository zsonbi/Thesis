using System.Collections;
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
using System;

public class MainWindowController : ThreadSafeMonoBehaviour
{
    [SerializeField]
    private TMP_Text UsernameInputText;

    [SerializeField]
    private TMP_Text CurrencyText;

    [SerializeField]
    private GameObject TaskParent;

    [SerializeField]
    private GameObject TaskPrefab;

    [SerializeField]
    private TaskOpenPanelController taskOpenPanelController;

    [SerializeField]
    public ModalWindow ModalWindow;

    [SerializeField]
    private TMP_Dropdown TaskSortDropdown;

    public Dictionary<long, TaskDisplayHandler> Tasks { get; private set; } = new Dictionary<long, TaskDisplayHandler>();

    // Start is called before the first frame update
    private async void Start()
    {
        while (!UserData.Instance.LoggedIn)
        {
            await Task.Delay(100);
        }
        UsernameInputText.text = UserData.Instance.Username;
        CurrencyText.text = UserData.Instance.CurrentTaskScore.ToString();
        LoadTasks();
    }

    public void UpdateUserData(Thesis_backend.Data_Structures.User user)
    {
        UserData.Instance.Init(user);
        CurrencyText.text = UserData.Instance.CurrentTaskScore.ToString();
    }

    // Update is called once per frame
    private void Update()
    {
    }

    public void RemoveTask(long taskId)
    {
        if (Tasks.ContainsKey(taskId))
        {
            TaskDisplayHandler TaskToRemove = Tasks[taskId];
            Tasks.Remove(taskId);
            Destroy(TaskToRemove.gameObject);
        }
    }

    public void UpdateTask(long taskId)
    {
        if (Tasks.ContainsKey(taskId))
        {
            Tasks[taskId].UpdateLabels();
        }
    }

    public void SortingChanged()
    {
        Dictionary<long, TaskDisplayHandler> newTasks = new Dictionary<long, TaskDisplayHandler>();
        switch ((TaskSortType)TaskSortDropdown.value)
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

        Tasks = newTasks;

        if (taskOpenPanelController.CurrentTask.TaskType)
        {
            LoadBadHabits();
        }
        else
        {
            LoadGoodTasks();
        }
    }

    public void LoadGoodTasks(bool updateDisplay = true)
    {
        foreach (var task in Tasks)
        {
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
        TaskParent.GetComponentInChildren<TMP_Text>().text = "Good tasks";
        if (updateDisplay)
            taskOpenPanelController.MakeItGoodTask();
    }

    public void LoadBadHabits(bool updateDisplay = true)
    {
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
        TaskParent.GetComponentInChildren<TMP_Text>().text = "Bad habits";
        if (updateDisplay)
            taskOpenPanelController.MakeItBadHabit();
    }

    public GameObject CreateTask(PlayerTask taskContainer)
    {
        try
        {
            GameObject task = Instantiate(TaskPrefab, TaskParent.transform);
            TaskDisplayHandler taskComponent = task.GetComponent<TaskDisplayHandler>();

            taskComponent.InitValues(taskContainer, taskOpenPanelController, this);

            Tasks.Add(taskContainer.ID, taskComponent);
            return task;
        }
        catch (MissingReferenceException e)
        {
            return null;
        }
    }

    private void LoadTasks()
    {
        CoroutineRunner.RunCoroutine(Server.SendGetRequest<List<Thesis_backend.Data_Structures.PlayerTask>>(ServerConfig.PATHFORTASKSQUERY, CreateTaskPrefabs, onFailedAction: ShowRequestFail));
    }

    private void ShowRequestFail(string content)
    {
        ModalWindow.Show("Request fail", content);
    }

    public void ShowTaskFail(string content)
    {
        ModalWindow.Show("Task error", content);
    }

    public void LoadGameScene()
    {
        SceneManager.LoadScene("GameScene", LoadSceneMode.Single);
    }

    private void CreateTaskPrefabs(List<Thesis_backend.Data_Structures.PlayerTask> requestResult)
    {
        foreach (var item in requestResult)
        {
            CreateTask(item);
        }
        LoadGoodTasks();
    }
}