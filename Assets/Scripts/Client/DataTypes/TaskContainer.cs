using Config;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskContainer
{
    public long Id { get; private set; }
    public string TaskName { get; private set; }
    public TaskType TaskType { get; private set; }
    public TaskIntervals TaskInterval { get; private set; }
    public string Description { get; private set; }
    public DateTime Added { get; private set; }
    public DateTime LastCompleted { get; private set; }
    public bool Completed { get; private set; }

    [Newtonsoft.Json.JsonConstructor]
    public TaskContainer(long id, string taskName, int taskType, int periodRate, string description, DateTime added, DateTime lastCompleted, bool completed)
    {
        Id = id;
        TaskName = taskName;
        TaskType = (TaskType)taskType;
        TaskInterval = (TaskIntervals)periodRate;
        Description = description;
        Added = added;
        LastCompleted = lastCompleted;
        Completed = completed;
    }

    public TaskContainer(Thesis_backend.Data_Structures.PlayerTask savedTask)
    {
        this.TaskName = savedTask.TaskName;
        this.TaskType = savedTask.TaskType ? TaskType.BadHabit : TaskType.GoodTask;
        this.TaskInterval = (TaskIntervals)savedTask.PeriodRate;
        this.Completed = savedTask.Completed;
        this.Id = savedTask.ID;
        this.Description = savedTask.Description;
        this.LastCompleted = savedTask.LastCompleted;
        this.Added = savedTask.Updated;
    }

    public TaskContainer()
    {
        this.Id = -1;
        this.TaskName = "";
        this.Description = "";
        this.TaskType = ServerConfig.DEFAULT_TASKTYPE;
        this.TaskInterval = ServerConfig.DEFAULT_TASKINTERVALS;
        this.Added = new DateTime(1970, 01, 11);
        this.LastCompleted = new DateTime(1970, 01, 11);
        this.Completed = false;
    }

    public void UpdateValues(string taskName, TaskType taskType, TaskIntervals taskInterval, string description, DateTime? lastCompleted = null, bool? completed = null)
    {
        this.TaskName = taskName;
        this.TaskType = taskType;
        this.TaskInterval = (TaskIntervals)taskInterval;
        this.Description = description;

        if (lastCompleted is not null)
        {
            this.LastCompleted = lastCompleted.Value;
        }
        if (completed is not null)
        {
            this.Completed = completed.Value;
        }
    }

    public void ChangeType(TaskType newType)
    {
        this.TaskType = newType;
    }

    public void Complete()
    {
        if (this.Completed)
        {
            return;
        }
        this.LastCompleted = DateTime.Now;
        this.Completed = true;
    }

    public void ResetComplete()
    {
        this.Completed = false;
    }
}