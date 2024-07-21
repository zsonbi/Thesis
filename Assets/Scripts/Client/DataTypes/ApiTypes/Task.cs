using Newtonsoft.Json;
using System;
using System.Text.Json.Serialization;

namespace Thesis_backend.Data_Structures
{
    public record Task : DbElement
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
    }
}