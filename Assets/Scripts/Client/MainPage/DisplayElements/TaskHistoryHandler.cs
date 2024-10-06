using System.Collections;
using System.Collections.Generic;
using Thesis_backend.Data_Structures;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class TaskHistoryHandler : MonoBehaviour
{
    [SerializeField]
    private TMP_Text taskNameText;

    [SerializeField]
    private TMP_Text taskIntervalText;

    [SerializeField]
    private TMP_Text completeDateText;

    public void Init(TaskHistory taskHistory)
    {
        taskNameText.text = taskHistory.CompletedTask.TaskName;

        completeDateText.text = string.Format("{0}. {1:D2}. {2:D2}.", (int)taskHistory.Completed.Year, taskHistory.Completed.Month, taskHistory.Completed.Day);

        taskIntervalText.text = ((TaskIntervals)(taskHistory.CompletedTask.PeriodRate)).ToString();

        if (taskHistory.CompletedTask.TaskType)
        {
            this.GetComponent<Image>().color = new Color(83f / 255f, 19f / 255f, 0);
        }
        else
        {
            this.GetComponent<Image>().color = new Color(0f / 255f, 83f / 255f, 0);
        }
    }
}