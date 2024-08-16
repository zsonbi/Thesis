using System;
using System.Text.Json.Serialization;

namespace Thesis_backend.Data_Structures
{
    public record Game : DbElement
    {
        public int Lvl { get; set; }

        public long CurrentXP { get; set; }
        public int NextLVLXP { get; set; }
        public DateTime Currency { get; set; }
    }
}