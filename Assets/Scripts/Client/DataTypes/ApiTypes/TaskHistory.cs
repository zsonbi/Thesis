using Newtonsoft.Json;
using System;

namespace Thesis_backend.Data_Structures
{
    public record TaskHistory : DbElement
    {
        public User Owner { get; set; }

        public PlayerTask CompletedTask { get; set; }

        public long OwnerId { get; set; }

        public long TaskId { get; set; }

        public DateTime Completed { get; set; }
        [JsonIgnore]
        public override object Serialize => new { ID, Owner = OwnerId, Completed, CompletedTask.Serialize };
    }
}