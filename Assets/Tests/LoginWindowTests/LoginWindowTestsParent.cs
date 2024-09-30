using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tests;
using UnityEngine;

namespace Tests
{
    namespace LoginWindow
    {
        public abstract class LoginWindowTestsParent<T> : UnityTestParent<T>
        {
            protected IEnumerator LoadScene(bool login = true, bool logout = false)
            {
                yield return base.LoadSceneBase(TestConfig.LOGIN_SCENE_NAME, TestConfig.USER_CONTROLLER_OBJECT_NAME, login, logout);

                yield return null;
            }
        }
    }
}