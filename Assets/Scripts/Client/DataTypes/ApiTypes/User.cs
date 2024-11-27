#nullable enable

using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Thesis_backend.Data_Structures
{
    /// <summary>
    /// A single user record
    /// </summary>
    public record User : DbElement
    {
        /// <summary>
        /// Username of the user
        /// </summary>
        public string Username { get; set; } = string.Empty;
        /// <summary>
        /// Hash value of the user's password
        /// </summary>
        public string PasswordHash { get; set; } = string.Empty;
        /// <summary>
        /// Email of the user
        /// </summary>
        public string Email { get; set; } = string.Empty;
        /// <summary>
        /// The user's user settings
        /// </summary>
        public UserSettings? UserSettings { get; set; }
        /// <summary>
        /// The user's game record
        /// </summary>
        public Game? Game { get; set; }
        /// <summary>
        /// Last time the user has logged in
        /// </summary>
        public DateTime LastLoggedIn { get; set; }
        /// <summary>
        /// The time the user has registered
        /// </summary>
        public DateTime Registered { get; set; }
        /// <summary>
        /// The user's tasks
        /// </summary>
        public List<PlayerTask>? UserTasks { get; set; }
        /// <summary>
        /// The number of good tasks the user has completed
        /// </summary>
        public int CompletedGoodTasks { get; set; } = 0;
        /// <summary>
        /// The number of bad tasks the user has completed
        /// </summary>
        public int CompletedBadTasks { get; set; } = 0;
        /// <summary>
        /// The total score of the user
        /// </summary>
        public long TotalScore { get; set; }
        /// <summary>
        /// The current score of the user
        /// </summary>
        public long CurrentTaskScore { get; set; }
        [JsonIgnore]
        public override object Serialize => new { ID, Username, PasswordHash, Email, userSettings = UserSettings?.Serialize, game = Game?.Serialize, LastLoggedIn, Registered, UserTasks, TotalScore, CurrentTaskScore };
    }
}