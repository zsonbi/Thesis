using System;

namespace User
{
    public static class UserData
    {
        public static string Username { get; private set; }
        public static string Email { get; private set; }
        public static int Id { get; private set; }
        public static int SettingsId { get; private set; }
        public static int GameId { get; private set; }
        public static DateTime LastLoggedIn { get; private set; }
        public static DateTime Registered { get; private set; }
        public static bool LoggedIn { get; private set; } = false;

        public static void Init(string username, string email, int id, int settingsId, int gameId, DateTime lastLoginTime, DateTime registerTime)
        {
            Username = username;
            Email = email;
            Id = id;
            SettingsId = settingsId;
            GameId = gameId;
            LastLoggedIn = lastLoginTime;
            Registered = registerTime;

            LoggedIn = true;
        }

        public static void Logout()
        {
            LoggedIn = false;
        }
    }
}