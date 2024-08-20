using System.Diagnostics.CodeAnalysis;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Thesis_backend.Data_Structures
{
    public record User : DbElement
    {
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public UserSettings? UserSettings { get; set; }
        public long GameId { get; set; }
        public DateTime LastLoggedIn { get; set; }
        public DateTime Registered { get; set; }
        public List<Thesis_backend.Data_Structures.PlayerTask>? UserTasks { get; set; }
        [JsonIgnore]
        public override object Serialize => new { Username, PasswordHash, Email, UserSettings?.Serialize, GameId, LastLoggedIn, Registered, UserTasks };
    }
}