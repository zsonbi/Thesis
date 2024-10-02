using Config;
using Game;
using NUnit.Framework;
using System;
using Tests;
using Thesis_backend.Data_Structures;
using UnityEngine;

namespace Tests
{
    [SetUpFixture]
    public class GlobalSetup
    {
        // This method will run once before any tests in the namespace or assembly
        [OneTimeSetUp]
        public void BeforeAllTestsAsync()
        {
            RegisterTestUsers();
        }

        private void RegisterTestUsers()
        {
            TestConfig.UserName = "testt7GuSu" + DateTime.Now.Ticks.ToString();
            TestConfig.Email = TestConfig.UserName + "@gmail.com";
            Register(TestConfig.UserName, TestConfig.Email, TestConfig.Password);

            TestConfig.Username2 = "testt7GuSu" + (DateTime.Now.Ticks+1).ToString();
            TestConfig.Email2 = TestConfig.Username2 + "@gmail.com";
            Register(TestConfig.Username2, TestConfig.Email2, TestConfig.Password);
        }

        private void Register(string username, string email, string password)
        {
            UserRequest userRequest = new UserRequest()
            {
                Email = email,
                UserName = username,
                Password = password
            };

            CoroutineRunner.RunCoroutine(Server.SendPostRequest<Thesis_backend.Data_Structures.User>(ServerConfig.PATHFORREGISTER, userRequest));
        }

        // This method will run once after all tests in the namespace or assembly
        [OneTimeTearDown]
        public void AfterAllTests()
        {
            CoroutineRunner.RunCoroutine(Server.SendDeleteRequest<string>(ServerConfig.PATHFORLOGOUT));
            Debug.Log("This runs once after all tests in the namespace/assembly.");
            // Add global teardown code here
        }
    }
}