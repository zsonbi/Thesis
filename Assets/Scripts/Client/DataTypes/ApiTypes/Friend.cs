using System;

namespace Thesis_backend.Data_Structures
{
    public record Friend : DbElement
    {
        public User Sender { get; set; }
        public User Reciever { get; set; }

        public DateTime SentTime { get; set; } = DateTime.UtcNow;

        public bool Pending { get; set; } = true;

        public override object Serialize => new { ID, sender = Sender?.ID, reciever = Reciever?.ID, SentTime, Pending };
    }
}