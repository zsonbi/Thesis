using Config;
using System;

namespace Tests
{
    internal static class TestConfig
    {
        public const float ANSWER_TOLERANCE = 3f;
        public const float SCENE_TOLERANCE = 1f;

        public const string TASK_SCORE_CHEAT_PATH = ServerConfig.SERVERPATH + "/api/Tasks/Cheat?amount=100000'";
        public const string TASK_SCORE_CHEAT_PASSWORD = "O07gYNin2*8f3BxdC$gI";

        public static string UserName = "test";
        public static string Email = "test@gmail.com";
        public const string Password = "TestCase123456";

        public static string Username2 = "test";
        public static string Email2 = "test@gmail.com";
        public const string Password2 = "TestCase123456";

        public const string LOGIN_SCENE_NAME = "LoginScene";

        public const string USER_CONTROLLER_OBJECT_NAME = "UserController";

        public const string MAIN_SCENE_NAME = "MainScene";

        public const string MAIN_SCREEN_CONTROLLER_OBJECT_NAME = "MainWindow";
        public const string FRIEND_PANEL_CONTROLLER_OBJECT_NAME = "FriendsPanel";
        public const string PROFILE_PANEL_CONTROLLER_OBJECT_NAME = "ProfilePanel";
        public const string TASK_PANEL_CONTROLLER_OBJECT_NAME = "TaskOpenPanel";

        public const string TASK_PANEL_GOOD_TASK_BUTTON_NAME = "GoodTaskButton";
        public const string TASK_PANEL_BAD_TASK_BUTTON_NAME = "BadHabitButton";
        public const string TASK_PANEL_TASK_NAME_INPUT_NAME = "TaskNameInput";
        public const string TASK_PANEL_TASK_NAME_DESCRIPTION_NAME = "DescriptionInput";
        public const string TASK_PANEL_TASK_PERIOD_DROPDOWN_NAME = "Intervals";
        public const string TASK_PANEL_SAVE_TASK_BUTTON_NAME = "SaveButton";
        public const string TASK_PANEL_CANCEL_TASK_BUTTON_NAME = "CancelButton";
        public const string TASK_PANEL_CLOSE_BUTTON_NAME = "CloseButton";

        public const string TASK_PARENT_NAME = "TaskParent";

        public const string GAME_SCENE_NAME = "GameScene";
        public const string GAME_CONTROLLER_OBJECT_NAME = "Game";
    }
}