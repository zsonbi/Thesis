namespace Config
{
    public static class ServerConfig
    {
        public const TaskType DEFAULT_TASKTYPE = TaskType.GoodTask;
        public const TaskIntervals DEFAULT_TASKINTERVALS = TaskIntervals.Daily;

        public const string SERVERPATH = "http://localhost:5133";

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
        /// Saves a task (modifies it if other id than -1 was given)
        /// Needs to be already logged in "taskName", "description", "periodRate", "taskType"
        /// Optional "id"
        /// </summary>
        public const string PATHFORTASKSAVE = SERVERPATH + "/saveTask.php";

        public const string PATHFORTASKCREATE = SERVERPATH + "/api/Tasks/Create";

        public static string PATHFORTASKUPDATE(long id) => SERVERPATH + $"/api/Tasks/{id}/Update";

        /// <summary>
        /// Gets all the tasks
        /// Needs to be already logged in
        /// </summary>
        public const string PATHFORTASKSQUERY = SERVERPATH + "/api/Tasks/GetAll";

        /// <summary>
        /// Complete the given task
        /// Needs to be already logged in and the task's "id"
        /// </summary>
        public static string PATHFORTASKCOMPLETE(long id) => SERVERPATH + $"/api/Tasks/{id}/Complete";

        /// <summary>
        /// Deletes the given task
        /// Needs to be already logged in and the task's "id"
        /// </summary>
        public static string PATHFORTASKDELETE(long id) => SERVERPATH + $"/api/Tasks/{id}";
    }
}