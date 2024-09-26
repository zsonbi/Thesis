using Assets.Tests;
using Config;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tests;
using Thesis_backend.Data_Structures;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

[SetUpFixture]
public class GlobalSetup
{
    // This method will run once before any tests in the namespace or assembly
    [OneTimeSetUp]
    public void BeforeAllTestsAsync()
    {
        RegisterTestUser();
   

    }

    private void RegisterTestUser()
    {
        TestConfig.UserName = "test" + DateTime.Now.Ticks.ToString();
        TestConfig.Email = TestConfig.UserName + "@gmail.com";
        Register(TestConfig.UserName, TestConfig.Email, TestConfig.Password);
    }

    private void Register(string userName, string email, string password)
    {
        UserRequest userRequest = new UserRequest()
        {
            Email = email,
            UserName = userName,
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