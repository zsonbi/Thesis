using Game.World;
using System.Collections;
using Tests;
using UnityEngine;

namespace Tests
{
    namespace GameTests
    {
        public abstract class GameTestsParent<T> : UnityTestParent<T>
        {
            protected GameUI GameUI;
            protected CarSpawner CarSpawner;
            protected ShopWindow ShopWindow;

            protected IEnumerator LoadScene(bool login = true, bool logout = false)
            {
                yield return base.LoadSceneBase(TestConfig.GAME_SCENE_NAME, TestConfig.GAME_CONTROLLER_OBJECT_NAME, login, logout);
                yield return null;
                this.GameUI = GameObject.FindObjectOfType<GameUI>(true);
                if (this.GameUI == null)
                {
                    Debug.LogError("Game ui was not found!");
                }
                this.CarSpawner = GameObject.FindObjectOfType<CarSpawner>(true);
                if (this.CarSpawner == null)
                {
                    Debug.LogError("Car spawner was not found!");
                }
                this.ShopWindow = GameObject.FindObjectOfType<ShopWindow>(true);
                if (this.ShopWindow == null)
                {
                    Debug.LogError("Shop window was not found!");
                }

                yield return null;
            }
        }
    }
}