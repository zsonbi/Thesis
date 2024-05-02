namespace Config
{
    public static class ServerConfig
    {
        public const TaskType DEFAULT_TASKTYPE = TaskType.GoodTask;
        public const TaskIntervals DEFAULT_TASKINTERVALS = TaskIntervals.Daily;

        public const string SERVERPATH = "http://89.134.179.209:5555";

        /// <summary>
        /// Needs "UserIdentification", "Password"
        /// </summary>
        public const string PATHFORLOGIN = SERVERPATH + "/login.php";

        /// <summary>
        /// Needs "Username", "Password", "Email"
        /// </summary>
        public const string PATHFORREGISTER = SERVERPATH + "/register.php";

        /// <summary>
        /// Just need to be sent
        /// </summary>
        public const string PATHFORCHECKLOGGEDIN = SERVERPATH + "/isLoggedIn.php";

        /// <summary>
        /// Just need to be sent
        /// Cleares the users session
        /// (Still need to reset the userData)
        /// </summary>
        public const string PATHFORLOGOUT = SERVERPATH + "/logout.php";

        /// <summary>
        /// Saves a task (modifies it if other id than -1 was given)
        /// Needs to be already logged in "taskName", "description", "periodRate", "taskType"
        /// Optional "id"
        /// </summary>
        public const string PATHFORTASKSAVE = SERVERPATH + "/saveTask.php";

        /// <summary>
        /// Gets all the tasks
        /// Needs to be already logged in
        /// </summary>
        public const string PATHFORTASKSQUERY = SERVERPATH + "/getTasks.php";

        /// <summary>
        /// Complete the given task
        /// Needs to be already logged in and the task's "id"
        /// </summary>
        public const string PATHFORTASKCOMPLETE = SERVERPATH + "/completeTask.php";

        /// <summary>
        /// Deletes the given task
        /// Needs to be already logged in and the task's "id"
        /// </summary>
        public const string PATHFORTASKDELETE = SERVERPATH + "/deleteTask.php";
    }
}