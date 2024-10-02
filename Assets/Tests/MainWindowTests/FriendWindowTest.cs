using NUnit.Framework;
using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace Tests
{
    namespace MainWindowTests
    {
        public class FriendWindowTests : MainWindowTestsParent<MainWindowController>
        {
            private IEnumerator SendFriendRequest(string friendIdentifiaction)
            {
                TMP_InputField userIdentification = GameObject.Find("FriendIdentifierInput").GetComponent<TMP_InputField>();
                userIdentification.text = friendIdentifiaction;
                friendHandler.SendFriendRequest();

                yield return new WaitForSeconds(TestConfig.ANSWER_TOLERANCE);
            }

            [UnityTest]
            public IEnumerator FriendPanelSendAndDeleteTest()
            {
                yield return LoadScene(true, true);

                friendHandler.Show();

                yield return SendFriendRequest(TestConfig.Username2);

                FriendHandler friend = GameObject.FindObjectsByType<FriendHandler>(FindObjectsSortMode.InstanceID).First();

                Assert.AreEqual(1, GameObject.FindObjectsByType<FriendHandler>(FindObjectsSortMode.InstanceID).Length);
                Assert.True(friend.Friend.Pending);
                Assert.AreEqual(User.UserData.Instance.Id, friend.Friend.Sender.ID);
                Assert.AreEqual(User.UserData.Instance.TotalScore, friend.Friend.Sender.TotalScore);

                //Unfriend him
                friend.DeleteFriend();
                yield return new WaitForSeconds(TestConfig.ANSWER_TOLERANCE);

                Assert.AreEqual(0, GameObject.FindObjectsByType<FriendHandler>(FindObjectsSortMode.InstanceID).Length);
            }

            [UnityTest]
            public IEnumerator FriendPanelPersistanceTest()
            {
                yield return LoadScene(true, true);

                friendHandler.Show();

                yield return SendFriendRequest(TestConfig.Username2);

                FriendHandler friend = GameObject.FindObjectsByType<FriendHandler>(FindObjectsSortMode.InstanceID).First();

                Assert.AreEqual(1, GameObject.FindObjectsByType<FriendHandler>(FindObjectsSortMode.InstanceID).Length);
                Assert.True(friend.Friend.Pending);
                Assert.AreEqual(User.UserData.Instance.Id, friend.Friend.Sender.ID);
                Assert.AreEqual(User.UserData.Instance.TotalScore, friend.Friend.Sender.TotalScore);

                //Hide the window
                friendHandler.Hide();

                //Show the window
                friendHandler.Show();
                yield return new WaitForSeconds(TestConfig.ANSWER_TOLERANCE);
                Assert.AreEqual(1, GameObject.FindObjectsByType<FriendHandler>(FindObjectsSortMode.InstanceID).Length);

                //Reload the scene
                yield return LoadScene(true, true);

                friendHandler.Show();
                yield return new WaitForSeconds(TestConfig.ANSWER_TOLERANCE);

                Assert.AreEqual(1, GameObject.FindObjectsByType<FriendHandler>(FindObjectsSortMode.InstanceID).Length);
                friend = GameObject.FindObjectsByType<FriendHandler>(FindObjectsSortMode.InstanceID).First();
                friend.DeleteFriend();
                yield return new WaitForSeconds(TestConfig.ANSWER_TOLERANCE);
            }

            [UnityTest]
            public IEnumerator FriendPanelSendAcceptAndDeleteTest()
            {
                yield return LoadScene(true, true);

                friendHandler.Show();

                yield return SendFriendRequest(TestConfig.Username2);

                FriendHandler friend = GameObject.FindObjectsByType<FriendHandler>(FindObjectsSortMode.InstanceID).First();

                Assert.AreEqual(1, GameObject.FindObjectsByType<FriendHandler>(FindObjectsSortMode.InstanceID).Length);
                Assert.True(friend.Friend.Pending);
                Assert.AreEqual(User.UserData.Instance.Id, friend.Friend.Sender.ID);
                Assert.AreEqual(User.UserData.Instance.TotalScore, friend.Friend.Sender.TotalScore);

                yield return Login(TestConfig.Username2, TestConfig.Password2);
                yield return LoadScene(true, false);
                friendHandler.Show();
                yield return new WaitForSeconds(TestConfig.ANSWER_TOLERANCE);

                friend = GameObject.FindObjectsByType<FriendHandler>(FindObjectsSortMode.InstanceID).First();
                Assert.True(friend.Friend.Pending);
                Assert.AreEqual(User.UserData.Instance.Id, friend.Friend.Receiver.ID);
                Assert.AreEqual(User.UserData.Instance.TotalScore, friend.Friend.Receiver.TotalScore);
                //Accept it
                friend.AcceptFriendRequest();
                yield return new WaitForSeconds(TestConfig.ANSWER_TOLERANCE);

                Assert.False(friend.Friend.Pending);

                //Unfriend him
                friend.DeleteFriend();
                yield return new WaitForSeconds(TestConfig.ANSWER_TOLERANCE);

                Assert.AreEqual(0, GameObject.FindObjectsByType<FriendHandler>(FindObjectsSortMode.InstanceID).Length);
            }
        }
    }
}