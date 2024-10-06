using System;
using Thesis_backend.Data_Structures;

namespace User
{
    /// <summary>
    /// Singleton class for storing the user's data
    /// </summary>
    public class UserData
    {
        /// <summary>
        /// Singleton private instance
        /// </summary>
        private static UserData instance = null;

        /// <summary>
        /// Instance getter
        /// </summary>
        public static UserData Instance { get => GetOrCreate(); }

        /// <summary>
        /// The user's username
        /// </summary>
        public string Username { get; private set; }

        /// <summary>
        /// The user's email
        /// </summary>
        public string Email { get; private set; }

        /// <summary>
        /// The user's id
        /// </summary>
        public long Id { get; private set; }

        /// <summary>
        /// The user's settings
        /// </summary>
        public UserSettings UserSettings { get; private set; }

        /// <summary>
        /// The user's game data
        /// </summary>
        public Thesis_backend.Data_Structures.Game Game { get; private set; }

        /// <summary>
        /// When did the user last logged in
        /// </summary>
        public DateTime LastLoggedIn { get; private set; }

        /// <summary>
        /// When did the user registered
        /// </summary>
        public DateTime Registered { get; private set; }

        /// <summary>
        /// Is the user currently logged in
        /// </summary>
        public bool LoggedIn { get; private set; } = false;

        /// <summary>
        /// Total accumulation of task score
        /// </summary>
        public long TotalScore { get; private set; } = 0;

        /// <summary>
        /// Current spendable task score amount
        /// </summary>
        public long CurrentTaskScore { get; private set; } = 0;

        /// <summary>
        /// How many good tasks did the user completed since creation
        /// </summary>
        public int CompletedGoodTasks { get; set; } = 0;

        /// <summary>
        /// How many bad habits the user completed since creation
        /// </summary>
        public int CompletedBadTasks { get; set; } = 0;

        /// <summary>
        /// Initialises the user instance with proper data
        /// </summary>
        /// <param name="loggedInUser">The logged in user's data from the server</param>
        public void Init(Thesis_backend.Data_Structures.User loggedInUser)
        {
            Username = loggedInUser.Username;
            Email = loggedInUser.Email;
            Id = loggedInUser.ID;
            UserSettings = loggedInUser.UserSettings;
            Game = loggedInUser.Game;
            LastLoggedIn = loggedInUser.LastLoggedIn;
            Registered = loggedInUser.Registered;
            TotalScore = loggedInUser.TotalScore;
            CurrentTaskScore = loggedInUser.CurrentTaskScore;
            LoggedIn = true;
            this.CompletedGoodTasks = loggedInUser.CompletedGoodTasks;
            this.CompletedBadTasks = loggedInUser.CompletedBadTasks;
        }

        /// <summary>
        /// Get's the instance or create it if it doesn't exist yet
        /// </summary>
        /// <returns>The instance</returns>
        private static UserData GetOrCreate()
        {
            if (instance is null)
            {
                instance = new UserData();
            }

            return instance;
        }

        /// <summary>
        /// Update the user's task score
        /// </summary>
        /// <param name="newAmount">The new amount to update to</param>
        public void UpdateTaskScore(long newAmount)
        {
            this.CurrentTaskScore = newAmount;
        }

        /// <summary>
        /// Creates the user data
        /// </summary>
        private UserData()
        {
        }

        /// <summary>
        /// Signal's that the user is logged out
        /// </summary>
        public void Logout()
        {
            LoggedIn = false;
        }
    }
}