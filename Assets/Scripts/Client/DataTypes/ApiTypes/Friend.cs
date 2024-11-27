using Newtonsoft.Json;
using System;

namespace Thesis_backend.Data_Structures
{
    /// <summary>
    /// A friend record
    /// </summary>
    public record Friend : DbElement
    {
        /// <summary>
        /// The user who sent the friend request
        /// </summary>
        public User Sender { get; set; }
        /// <summary>
        /// The user who recieved the friend request
        /// </summary>
        public User Receiver { get; set; }

        /// <summary>
        /// The time the friend request was sent
        /// </summary>
        public DateTime SentTime { get; set; } = DateTime.UtcNow;
        
        /// <summary>
        /// Is the friend request still pending
        /// </summary>
        public bool Pending { get; set; } = true;

        [JsonIgnore]
        public override object Serialize => new { ID, sender = Sender?.ID, reciever = Receiver?.ID, SentTime, Pending };
    }
}