using Newtonsoft.Json;
using System;

namespace Thesis_backend.Data_Structures
{
    /// <summary>
    /// The task history record
    /// </summary>
    public record TaskHistory : DbElement
    {
        /// <summary>
        /// The owner of the task
        /// </summary>
        public User Owner { get; set; }
        /// <summary>
        /// The completed task
        /// </summary>
        public PlayerTask CompletedTask { get; set; }
        /// <summary>
        /// Id of the user
        /// </summary>
        public long OwnerId { get; set; }
        /// <summary>
        /// Id of the task
        /// </summary>
        public long TaskId { get; set; }
        /// <summary>
        /// When it was completed
        /// </summary>
        public DateTime Completed { get; set; }
        [JsonIgnore]
        public override object Serialize => new { ID, Owner = OwnerId, Completed, CompletedTask.Serialize };
    }
}