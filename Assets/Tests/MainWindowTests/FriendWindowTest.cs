using NUnit.Framework;
using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using MainPage;
using Utility;

namespace Tests
{
    namespace MainWindowTests
    {
        public class FriendWindowTests : MainWindowTestsParent<MainWindowController>
        {
            private IEnumerator SendFriendRequest(string friendIdentifiaction)
            {
                int initFriendCount = GameObject.FindObjectsByType<FriendHandler>(FindObjectsInactive.Include, FindObjectsSortMode.InstanceID).Length;
                TMP_InputField userIdentification = GameObject.Find("FriendIdentifierInput").GetComponent<TMP_InputField>();
                userIdentification.text = friendIdentifiaction;
                friendHandler.SendFriendRequest();
                yield return new WaitForSeconds(TestConfig.ANSWER_TOLERANCE);

                yield return WaitForCondition(() => initFriendCount < GameObject.FindObjectsByType<FriendHandler>(FindObjectsInactive.Include, FindObjectsSortMode.InstanceID).Length);
                yield return WaitForCondition(() => GameObject.FindObjectsByType<FriendHandler>(FindObjectsInactive.Include, FindObjectsSortMode.InstanceID).FirstOrDefault() is not null);
                FriendHandler friend = GameObject.FindObjectsByType<FriendHandler>(FindObjectsInactive.Include, FindObjectsSortMode.InstanceID).First();

                Assert.AreEqual(1, GameObject.FindObjectsByType<FriendHandler>(FindObjectsInactive.Include, FindObjectsSortMode.InstanceID).Length);
                Assert.True(friend.Friend.Pending);
                Assert.AreEqual(User.UserData.Instance.Id, friend.Friend.Sender.ID);
                Assert.AreEqual(User.UserData.Instance.TotalScore, friend.Friend.Sender.TotalScore);
                yield return null;
            }

            [UnityTest]
            public IEnumerator FriendPanelSendAndDeleteTest()
            {
                yield return LoadScene(true, true);

                friendHandler.Show();
                yield return new WaitForSeconds(TestConfig.ANSWER_TOLERANCE);

                yield return SendFriendRequest(TestConfig.Username2);
                yield return WaitForCondition(() => GameObject.FindObjectsByType<FriendHandler>(FindObjectsInactive.Include, FindObjectsSortMode.InstanceID).Length > 0);

                FriendHandler friend = GameObject.FindObjectsByType<FriendHandler>(FindObjectsInactive.Include, FindObjectsSortMode.InstanceID).First();

                //Unfriend him
                friend.DeleteFriend();
                yield return WaitForCondition(() => GameObject.FindObjectsByType<FriendHandler>(FindObjectsInactive.Include, FindObjectsSortMode.InstanceID).Length == 0);

                Assert.AreEqual(0, GameObject.FindObjectsByType<FriendHandler>(FindObjectsInactive.Include, FindObjectsSortMode.InstanceID).Length);
            }

            [UnityTest]
            public IEnumerator FriendPanelPersistanceTest()
            {
                yield return LoadScene(true, true);

                friendHandler.Show();
                yield return new WaitForSeconds(TestConfig.ANSWER_TOLERANCE);

                yield return SendFriendRequest(TestConfig.Username2);
                yield return WaitForCondition(() => GameObject.FindObjectsByType<FriendHandler>(FindObjectsInactive.Include, FindObjectsSortMode.InstanceID).Length > 0);

                FriendHandler friend = GameObject.FindObjectsByType<FriendHandler>(FindObjectsInactive.Include, FindObjectsSortMode.InstanceID).First();

                //Hide the window
                friendHandler.Hide();

                //Show the window
                friendHandler.Show();
                yield return WaitForCondition(() => GameObject.FindObjectsByType<FriendHandler>(FindObjectsInactive.Include, FindObjectsSortMode.InstanceID).Length > 0);
                Assert.AreEqual(1, GameObject.FindObjectsByType<FriendHandler>(FindObjectsInactive.Include, FindObjectsSortMode.InstanceID).Length);

                yield return new WaitForSeconds(TestConfig.ANSWER_TOLERANCE);
                //Reload the scene
                yield return LoadScene(true, true);

                friendHandler.Show();
                yield return WaitForCondition(() => GameObject.FindObjectsByType<FriendHandler>(FindObjectsInactive.Include, FindObjectsSortMode.InstanceID).Length > 0);

                Assert.AreEqual(1, GameObject.FindObjectsByType<FriendHandler>(FindObjectsInactive.Include, FindObjectsSortMode.InstanceID).Length);
                friend = GameObject.FindObjectsByType<FriendHandler>(FindObjectsInactive.Include, FindObjectsSortMode.InstanceID).First();
                friend.DeleteFriend();
                yield return WaitForCondition(() => GameObject.FindObjectsByType<FriendHandler>(FindObjectsInactive.Include, FindObjectsSortMode.InstanceID).Length == 0);

                Assert.AreEqual(0, GameObject.FindObjectsByType<FriendHandler>(FindObjectsInactive.Include, FindObjectsSortMode.InstanceID).Length);
            }

            [UnityTest]
            public IEnumerator FriendPanelSendAcceptAndDeleteTest()
            {
                yield return LoadScene(true, true);

                friendHandler.Show();
                yield return new WaitForSeconds(TestConfig.ANSWER_TOLERANCE);

                yield return SendFriendRequest(TestConfig.Username2);
                yield return WaitForCondition(() => GameObject.FindObjectsByType<FriendHandler>(FindObjectsInactive.Include, FindObjectsSortMode.InstanceID).Length > 0);
                FriendHandler friend = GameObject.FindObjectsByType<FriendHandler>(FindObjectsInactive.Include, FindObjectsSortMode.InstanceID).First();

                //Login to the other user
                yield return Login(TestConfig.Username2, TestConfig.Password2);

                yield return LoadScene(true, false);
                friendHandler.Show();
                yield return WaitForCondition(() => GameObject.FindObjectsByType<FriendHandler>(FindObjectsInactive.Include, FindObjectsSortMode.InstanceID).Length > 0);
                friend = GameObject.FindObjectsByType<FriendHandler>(FindObjectsInactive.Include, FindObjectsSortMode.InstanceID).First();
                Assert.True(friend.Friend.Pending);
                Assert.AreEqual(User.UserData.Instance.Id, friend.Friend.Receiver.ID);
                Assert.AreEqual(User.UserData.Instance.TotalScore, friend.Friend.Receiver.TotalScore);
                //Accept it
                friend.AcceptFriendRequest();
                yield return WaitForCondition(() => !friend.Friend.Pending);

                Assert.False(friend.Friend.Pending);

                //Unfriend him
                friend.DeleteFriend();
                yield return WaitForCondition(() => GameObject.FindObjectsByType<FriendHandler>(FindObjectsInactive.Include, FindObjectsSortMode.InstanceID).Length == 0);

                Assert.AreEqual(0, GameObject.FindObjectsByType<FriendHandler>(FindObjectsInactive.Include, FindObjectsSortMode.InstanceID).Length);
            }
        }
    }
}