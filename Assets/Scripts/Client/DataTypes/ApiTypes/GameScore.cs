using Newtonsoft.Json;
using System;

namespace Thesis_backend.Data_Structures
{
    public record GameScore : DbElement
    {
        public User Owner { get; set; }
        public long OwnerId { get; set; }

        public int Score { get; set; }

        public string OwnerName { get; set; }

        public DateTime AchievedTime { get; set; }
        [JsonIgnore]
        public override object Serialize => new { ID, Owner = OwnerId, AchievedTime, Score };
    }
}