using Codice.CM.Common.Merge;
using System.Collections;
using System.Collections.Generic;
using Thesis_backend.Data_Structures;
using TMPro;
using UnityEngine;

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
    }
}