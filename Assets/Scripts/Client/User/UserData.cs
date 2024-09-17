using Config;
using System;
using System.Collections.Generic;
using Thesis_backend.Data_Structures;
using UnityEngine;

namespace User
{
    public class UserData : MonoBehaviour
    {
        private static UserData instance = null;
        public static UserData Instance { get => instance is null ? Create() : instance; }

        public string Username { get; private set; }
        public string Email { get; private set; }
        public long Id { get; private set; }
        public UserSettings SettingsId { get; private set; }
        public long GameId { get; private set; }
        public DateTime LastLoggedIn { get; private set; }
        public DateTime Registered { get; private set; }
        public bool LoggedIn { get; private set; } = false;

        public void Init(Thesis_backend.Data_Structures.User loggedInUser)
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

        private static UserData Create()
        {
            instance = new UserData();
            return instance;
        }

        private UserData()
        {
        }

        public void Logout()
        {
            LoggedIn = false;
            //StartCoroutine(Server.SendDeleteRequest<string>(ServerConfig.PATHFORLOGOUT));
        }
    }
}