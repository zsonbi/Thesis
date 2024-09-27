using System;

namespace Tests
{
    internal static class TestConfig
    {
        public const float ANSWER_TOLERANCE = 3f;
        public const float SCENE_TOLERANCE = 1f;

        public static string UserName = "test";

        public static string Email = "test@gmail.com";

        public const string Password = "TestCase123456";

        public const string LOGIN_SCENE_NAME = "LoginScene";

        public const string USER_CONTROLLER_OBJECT_NAME = "UserController";

        public const string MAIN_SCENE_NAME = "MainScene";

        public const string MAIN_SCREEN_CONTROLLER_OBJECT_NAME = "MainWindow";
        public const string FRIEND_PANEL_CONTROLLER_OBJECT_NAME = "FriendsPanel";
        public const string PROFILE_PANEL_CONTROLLER_OBJECT_NAME = "ProfilePanel";
        public const string TASK_PANEL_CONTROLLER_OBJECT_NAME = "TaskOpenPanel";
    }
}