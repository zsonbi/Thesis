#nullable enable

using Newtonsoft.Json;
using System;
using DataTypes;

namespace Thesis_backend.Data_Structures
{
    public record PlayerTask : DbElement
    {
        public User? TaskOwner { get; set; }
        public string? TaskName { get; set; }
        public string? Description { get; set; }
        public bool TaskType { get; set; }
        public int PeriodRate { get; set; }
        public DateTime Updated { get; set; }
        public DateTime LastCompleted { get; set; }
        public bool Completed { get; set; }
        [JsonIgnore]
        public override object Serialize => new { ID, TaskName, Description, TaskType, PeriodRate, Updated, LastCompleted, Completed };

        public PlayerTask(PlayerTask playerTask) : base(playerTask)
        {
            this.ID = playerTask.ID;
            this.TaskOwner = playerTask.TaskOwner;
            this.TaskName = playerTask.TaskName;
            this.Description = playerTask.Description;
            this.TaskType = playerTask.TaskType;
            this.PeriodRate = playerTask.PeriodRate;
            this.Updated = playerTask.Updated;
            this.Updated = playerTask.Updated;
            this.LastCompleted = playerTask.LastCompleted;
            this.Completed = playerTask.Completed;
        }

        public void UpdateValues(string taskName, bool taskType, TaskIntervals taskInterval, string description, DateTime? lastCompleted = null, bool? completed = null)
        {
            this.TaskName = taskName;
            this.TaskType = taskType;
            this.PeriodRate = (int)taskInterval;
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
        public void UpdateValues(PlayerTask playerTask)
        {
            if (playerTask is null)
            {
                return;
            }

            this.TaskName = playerTask.TaskName;
            this.TaskType = playerTask.TaskType;
            this.PeriodRate = playerTask.PeriodRate;
            this.Description = playerTask.Description;
            this.LastCompleted = playerTask.LastCompleted;
            this.Completed = playerTask.Completed;
        }

        public void ChangeType(TaskType newType)
        {
            this.TaskType = newType == DataTypes.TaskType.BadHabit;
        }

        public void Complete()
        {
            if (this.Completed)
            {
                return;
            }
            this.LastCompleted = DateTime.UtcNow;
            this.Completed = true;
        }

        public void ResetComplete()
        {
            this.Completed = false;
        }
    }
}