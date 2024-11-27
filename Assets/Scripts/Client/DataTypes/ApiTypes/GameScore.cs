using Newtonsoft.Json;
using System;

namespace Thesis_backend.Data_Structures
{
    /// <summary>
    /// Game score record
    /// </summary>
    public record GameScore : DbElement
    {
        /// <summary>
        /// The owner of the score
        /// </summary>
        public User Owner { get; set; }
        /// <summary>
        /// The id of the owner
        /// </summary>
        public long OwnerId { get; set; }
        /// <summary>
        /// The score amount
        /// </summary>
        public int Score { get; set; }
        /// <summary>
        /// The name of the owner
        /// </summary>
        public string OwnerName { get; set; }
        /// <summary>
        /// When it was achieved
        /// </summary>
        public DateTime AchievedTime { get; set; }
        [JsonIgnore]
        public override object Serialize => new { ID, Owner = OwnerId, AchievedTime, Score };
    }
}