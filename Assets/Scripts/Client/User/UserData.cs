using System;
using System.Collections.Generic;
using Thesis_backend.Data_Structures;
using UnityEngine;

namespace User
{
    public static class UserData
    {
        public static string Username { get; private set; }
        public static string Email { get; private set; }
        public static long Id { get; private set; }
        public static UserSettings SettingsId { get; private set; }
        public static long GameId { get; private set; }
        public static DateTime LastLoggedIn { get; private set; }
        public static DateTime Registered { get; private set; }
        public static bool LoggedIn { get; private set; } = false;

        public static void Init(Thesis_backend.Data_Structures.User loggedInUser)
        {
            Username = loggedInUser.Username;
            Email = loggedInUser.Email;
            Id = loggedInUser.ID;
            SettingsId = loggedInUser.UserSettings;
            GameId = loggedInUser.GameId;
            LastLoggedIn = loggedInUser.LastLoggedIn;
            Registered = loggedInUser.Registered;

            LoggedIn = true;
        }

        public static void Logout()
        {
            LoggedIn = false;
            Debug.Log(Server.SendDeleteRequest<string>(Config.ServerConfig.PATHFORLOGOUT));
        }
    }
}