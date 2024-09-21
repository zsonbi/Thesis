using Newtonsoft.Json;

using System.Diagnostics.CodeAnalysis;

namespace Thesis_backend.Data_Structures
{
    public record UserSettings : DbElement
    {
        public User User { get; set; }
        public long UserId { get; set; }

        public string ProfilePic { get; set; }
        public long privacy { get; set; }
    }
}