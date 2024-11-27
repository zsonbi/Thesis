#nullable enable

using Newtonsoft.Json;
using System;
using DataTypes;

namespace Thesis_backend.Data_Structures
{
    /// <summary>
    /// The player task record
    /// </summary>
    public record PlayerTask : DbElement
    {
        /// <summary>
        /// Reference to the owner
        /// </summary>
        public User? TaskOwner { get; set; }
        /// <summary>
        /// The task's name
        /// </summary>
        public string? TaskName { get; set; }
        /// <summary>
        /// The task's description
        /// </summary>
        public string? Description { get; set; }
        /// <summary>
        /// The task type
        /// </summary>
        public bool TaskType { get; set; }
        /// <summary>
        /// The task's period rate
        /// </summary>
        public int PeriodRate { get; set; }
        /// <summary>
        /// When the task was updated
        /// </summary>
        public DateTime Updated { get; set; }
        /// <summary>
        /// The last time the task was completed
        /// </summary>
        public DateTime LastCompleted { get; set; }
        /// <summary>
        /// Has the task been completed
        /// </summary>
        public bool Completed { get; set; }
        /// <summary>
        /// Has the task been deleted
        /// </summary>
        public bool Deleted { get; set; } = false;

        [JsonIgnore]
        public override object Serialize => new { ID, TaskName, Description, TaskType, PeriodRate, Updated, LastCompleted, Completed, Deleted };

        /// <summary>
        /// Create a new player task
        /// </summary>
        /// <param name="playerTask">The player task to copy</param>
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
        /// <summary>
        /// Update the task values from parameters
        /// </summary>
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
        /// <summary>
        /// Update the task's values
        /// </summary>
        /// <param name="playerTask">From an another task</param>
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
        /// <summary>
        /// Change the task's type
        /// </summary>
        /// <param name="newType">The new type of the task</param>
        public void ChangeType(TaskType newType)
        {
            this.TaskType = newType == DataTypes.TaskType.BadHabit;
        }

        /// <summary>
        /// Mark task as completed
        /// </summary>
        public void Complete()
        {
            if (this.Completed)
            {
                return;
            }
            this.LastCompleted = DateTime.UtcNow;
            this.Completed = true;
        }
        /// <summary>
        /// Reset the complete state
        /// </summary>
        public void ResetComplete()
        {
            this.Completed = false;
        }
    }
}