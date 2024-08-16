using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using User;
using TMPro;
using Config;
using Thesis_backend.Data_Structures;

public class UIController : MonoBehaviour
{
    [SerializeField]
    private TMP_Text UsernameInputText;

    [SerializeField]
    private GameObject TaskParent;

    [SerializeField]
    private GameObject TaskPrefab;

    [SerializeField]
    private TaskOpenPanelController taskOpenPanelController;

    [SerializeField]
    public ModalWindow ModalWindow;

    private Dictionary<long, Task> tasks = new Dictionary<long, Task>();

    // Start is called before the first frame update
    private void Start()
    {
        if (!UserData.LoggedIn)
        {
            StartCoroutine(MoveToLoginScene());
        }
        else
        {
            UsernameInputText.text = UserData.Username;
            LoadTasks();
        }
    }

    // Update is called once per frame
    private void Update()
    {
    }

    public void RemoveTask(long taskId)
    {
        if (tasks.ContainsKey(taskId))
        {
            Task TaskToRemove = tasks[taskId];
            tasks.Remove(taskId);
            Destroy(TaskToRemove.gameObject);
        }
    }

    public void UpdateTask(long taskId)
    {
        if (tasks.ContainsKey(taskId))
        {
            tasks[taskId].UpdateLabels();
        }
    }

    public void LoadGoodTasks()
    {
        foreach (var task in tasks)
        {
            if (task.Value.TaskContainer.TaskType == TaskType.GoodTask)
            {
                task.Value.gameObject.SetActive(true);
            }
            else
            {
                task.Value.gameObject.SetActive(false);
            }
        }
        TaskParent.GetComponentInChildren<TMP_Text>().text = "Good tasks";
        taskOpenPanelController.MakeItGoodTask();
    }

    public void LoadBadHabits()
    {
        foreach (var task in tasks)
        {
            if (task.Value.TaskContainer.TaskType == TaskType.BadHabit)
            {
                task.Value.gameObject.SetActive(true);
            }
            else
            {
                task.Value.gameObject.SetActive(false);
            }
        }
        TaskParent.GetComponentInChildren<TMP_Text>().text = "Bad habits";
        taskOpenPanelController.MakeItBadHabit();
    }

    public GameObject CreateTask(TaskContainer taskContainer)
    {
        GameObject task = Instantiate(TaskPrefab, TaskParent.transform);
        Task taskComponent = task.GetComponent<Task>();
        taskComponent.InitValues(taskContainer, taskOpenPanelController);
        tasks.Add(taskContainer.Id, taskComponent);
        return task;
    }

    private IEnumerator MoveToLoginScene()
    {
        AsyncOperation loading = SceneManager.LoadSceneAsync("LoginScene", LoadSceneMode.Single);

        while (true)
        {
            Debug.Log(loading.progress);

            yield return null;
        }
    }

    private void LoadTasks()
    {
        StartCoroutine(Server.SendGetRequest<List<Thesis_backend.Data_Structures.Task>>(ServerConfig.PATHFORTASKSQUERY, CreateTaskPrefabs));
    }

    private void CreateTaskPrefabs(List<Thesis_backend.Data_Structures.Task> requestResult)
    {
        foreach (var item in requestResult)
        {
            CreateTask(new TaskContainer(item.ID, item.TaskName, System.Convert.ToInt32(item.TaskType), item.PeriodRate, item.Description, item.Updated, item.LastCompleted, item.Completed));
        }
        LoadGoodTasks();
    }
}