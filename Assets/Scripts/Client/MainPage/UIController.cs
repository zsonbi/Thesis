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
    private TMP_Text CurrencyText;

    [SerializeField]
    private GameObject TaskParent;

    [SerializeField]
    private GameObject TaskPrefab;

    [SerializeField]
    private TaskOpenPanelController taskOpenPanelController;

    [SerializeField]
    public ModalWindow ModalWindow;

    private Dictionary<long, TaskDisplayHandler> tasks = new Dictionary<long, TaskDisplayHandler>();

    // Start is called before the first frame update
    private void Start()
    {
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
        if (tasks.ContainsKey(taskId))
        {
            TaskDisplayHandler TaskToRemove = tasks[taskId];
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

    public void LoadGoodTasks(bool updateDisplay = true)
    {
        foreach (var task in tasks)
        {
            if (!task.Value.CurrentTask.TaskType)
            {
                task.Value.gameObject.SetActive(true);
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
        foreach (var task in tasks)
        {
            if (task.Value.CurrentTask.TaskType)
            {
                task.Value.gameObject.SetActive(true);
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
        GameObject task = Instantiate(TaskPrefab, TaskParent.transform);
        TaskDisplayHandler taskComponent = task.GetComponent<TaskDisplayHandler>();

        taskComponent.InitValues(taskContainer, taskOpenPanelController, this);

        tasks.Add(taskContainer.ID, taskComponent);
        return task;
    }

    private void LoadTasks()
    {
        StartCoroutine(Server.SendGetRequest<List<Thesis_backend.Data_Structures.PlayerTask>>(ServerConfig.PATHFORTASKSQUERY, CreateTaskPrefabs, onFailedAction: ShowRequestFail));
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