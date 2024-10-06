using System.Collections;
using Tests;
using UnityEngine;
using MainPage;

namespace Tests
{
    namespace MainWindowTests
    {
        public abstract class MainWindowTestsParent<T> : UnityTestParent<T>
        {
            protected FriendWindowHandler friendHandler;
            protected ProfileHandler profileHandler;
            protected TaskOpenPanelController taskOpenPanelController;

            protected IEnumerator LoadScene(bool login = true, bool logout = false)
            {
                yield return base.LoadSceneBase(TestConfig.MAIN_SCENE_NAME, TestConfig.MAIN_SCREEN_CONTROLLER_OBJECT_NAME, login, logout);
                yield return null;
                this.friendHandler = GameObject.FindObjectOfType<FriendWindowHandler>(true);
                if (this.friendHandler == null)
                {
                    Debug.LogError("Friend Window Handler not found!");
                }
                this.profileHandler = GameObject.FindObjectOfType<ProfileHandler>(true);
                if (this.profileHandler == null)
                {
                    Debug.LogError("Profile Handler not found!");
                }

                this.taskOpenPanelController = GameObject.FindObjectOfType<TaskOpenPanelController>(true);
                if (this.taskOpenPanelController == null)
                {
                    Debug.LogError("Task Open Panel Controller not found!");
                }
                yield return null;
            }
        }
    }
}