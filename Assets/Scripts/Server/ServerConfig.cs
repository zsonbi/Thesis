using DataTypes;
using Newtonsoft.Json;
using System;

namespace Config
{
    /// <summary>
    /// Constants for the server
    /// </summary>
    public static class ServerConfig
    {
        /// <summary>
        /// What should be the default task type selected
        /// </summary>
        public const TaskType DEFAULT_TASKTYPE = TaskType.GoodTask;

        /// <summary>
        /// What should be the default task interval selected on create
        /// </summary>
        public const TaskIntervals DEFAULT_TASKINTERVALS = TaskIntervals.Daily;

        /// <summary>
        /// Http version path to the backend
        /// </summary>
        //public const string SERVERPATH = "http://thesis.picidolgok.hu:8000";

        /// <summary>
        /// Https version path to the backend
        /// </summary>
        public const string SERVERPATH = "https://thesis.picidolgok.hu:8001";

        /// <summary>
        /// Needs "UserIdentification", "Password"
        /// </summary>
        public const string PATHFORLOGIN = SERVERPATH + "/api/Users/Login";

        /// <summary>
        /// Needs "Username", "Password", "Email"
        /// </summary>
        public const string PATHFORREGISTER = SERVERPATH + "/api/Users/Register";

        /// <summary>
        /// Just need to be sent
        /// </summary>
        public const string PATHFORCHECKLOGGEDIN = SERVERPATH + "/api/Users/LoggedInUser";

        /// <summary>
        /// Just need to be sent
        /// Cleares the users session
        /// (Still need to reset the userData)
        /// </summary>
        public const string PATHFORLOGOUT = SERVERPATH + "/api/Users/Logout";

        /// <summary>
        /// Path for the friend request api endpoint
        /// </summary>
        public const string PATH_FOR_FRIEND_REQUEST_SEND = SERVERPATH + "/api/Friends/Send";

        /// <summary>
        /// Path for the friend accept api endpoint
        /// </summary>
        /// <param name="friendRequestId">The id of the friend request we want to accept</param>
        public static string PATH_FOR_FRIEND_ACCEPT(long friendRequestId) => SERVERPATH + $"/api/Friends/{friendRequestId}/Accept";

        /// <summary>
        /// Path for the friend delete api endpoint
        /// </summary>
        /// <param name="friendRequestId">The id of the friend we want to delete</param>
        public static string PATH_FOR_FRIEND_DELETE(long friendRequestId) => SERVERPATH + $"/api/Friends/{friendRequestId}/Delete";

        /// <summary>
        /// Path for the api endpoint which returns all of the friends of the currently logged in user
        /// </summary>
        public const string PATH_FOR_FRIEND_GETALL = SERVERPATH + "/api/Friends/GetAll";

        /// <summary>
        /// Path to api endpoint to create a new task
        /// </summary>
        public const string PATHFORTASKCREATE = SERVERPATH + "/api/Tasks/Create";

        /// <summary>
        /// Pah to api endpoin to update a task
        /// </summary>
        /// <param name="id">The id of the selected task</param>
        public static string PATHFORTASKUPDATE(long id) => SERVERPATH + $"/api/Tasks/{id}/Update";

        /// <summary>
        /// Path to api endpoint to get all of the tasks
        /// </summary>
        public const string PATHFORTASKSQUERY = SERVERPATH + "/api/Tasks/GetAll";

        /// <summary>
        /// Path to api endpoint to complete the selected task
        /// </summary>
        /// <param name="id">The id of the task we want to complete</param>
        public static string PATHFORTASKCOMPLETE(long id) => SERVERPATH + $"/api/Tasks/{id}/Complete";

        /// <summary>
        /// Path to the api endpoint to delete the given task
        /// </summary>
        /// <param name="id">The id of the task we want to delete</param>
        public static string PATHFORTASKDELETE(long id) => SERVERPATH + $"/api/Tasks/{id}";

        /// <summary>
        /// Path to the api endpoint to get all of the shop items
        /// </summary>
        public const string PATH_FOR_SHOP_GET_ALL = SERVERPATH + "/api/Shop/GetAll";

        /// <summary>
        /// Path to the api endpoint to double the coins from the game (only spends the task score doubling need to be done locally)
        /// </summary>
        public const string PATH_FOR_DOUBLE_COINS = SERVERPATH + "/api/Game/DoubleCoinsForGame";

        /// <summary>
        /// Path to api endpoint to add coins to the player
        /// </summary>
        public const string PATH_TO_ADD_COINS = SERVERPATH + "/api/Game/AddCoin";

        /// <summary>
        /// Path to api endpoint to buy a given car
        /// </summary>
        /// <param name="id">The shop id of the car</param>
        public static string PATH_FOR_BUY_CAR(long id) => SERVERPATH + $"/api/Shop/Buy/{id}";

        /// <summary>
        /// Path to api endpoint to buy a turbo boost
        /// </summary>
        public const string PATH_FOR_BUY_TURBO = SERVERPATH + "/api/Game/PowerUp/Turbo";

        /// <summary>
        /// Path to api endpoin to buy an immunity
        /// </summary>
        public const string PATH_FOR_BUY_IMMUNITY = SERVERPATH + "/api/Game/PowerUp/Immunity";

        /// <summary>
        /// Path to api endpoint to store the last game's score
        /// </summary>
        public const string PATH_FOR_STORE_GAME_SCORE = SERVERPATH + "/api/Game/Scores/Store";

        /// <summary>
        /// Path to api endpoint to get all the game scores since the given time (limit 100)
        /// </summary>
        /// <param name="filterTime">Every score which happened later than this</param>
        public static string PATH_FOR_GET_GAME_SCORES(DateTime filterTime) => SERVERPATH + $"/api/Game/Scores/Get?since={filterTime.ToString("yyyy-MM-ddTHH:mm:ss")}";

        /// <summary>
        /// Path to api endpoint to get all of the task histtories
        /// </summary>
        public const string PATH_FOR_GET_TASK_HISTORIES = SERVERPATH + "/api/Tasks/History";
    }
}