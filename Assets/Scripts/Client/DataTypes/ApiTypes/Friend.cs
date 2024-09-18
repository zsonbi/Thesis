using Newtonsoft.Json;
using System;

namespace Thesis_backend.Data_Structures
{
    public record Friend : DbElement
    {
        public User Sender { get; set; }
        public User Receiver { get; set; }

        public DateTime SentTime { get; set; } = DateTime.UtcNow;

        public bool Pending { get; set; } = true;

        [JsonIgnore]
        public override object Serialize => new { ID, sender = Sender?.ID, reciever = Receiver?.ID, SentTime, Pending };
    }
}