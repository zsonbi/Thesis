using Newtonsoft.Json;
using System;

namespace Thesis_backend.Data_Structures
{
    public record PlayerTask : DbElement
    {
        public User TaskOwner { get; set; }
        public string TaskName { get; set; }
        public string? Description { get; set; }
        public bool TaskType { get; set; }
        public int PeriodRate { get; set; }
        public DateTime Updated { get; set; }
        public DateTime LastCompleted { get; set; }
        public bool Completed { get; set; }
        [JsonIgnore]
        public override object Serialize => new { ID, TaskName, Description, TaskType, PeriodRate, Updated, LastCompleted, Completed };

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

        public void ChangeType(TaskType newType)
        {
            this.TaskType = newType == global::TaskType.BadHabit;
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