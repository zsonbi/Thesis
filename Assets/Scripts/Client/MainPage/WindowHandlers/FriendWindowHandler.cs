using Config;
using System.Collections.Generic;
using Thesis_backend.Data_Structures;
using TMPro;
using UnityEngine;

namespace MainPage
{
    /// <summary>
    /// Handles the friend window display
    /// </summary>
    public class FriendWindowHandler : ThreadSafeMonoBehaviour
    {
        /// <summary>
        /// Parent for the friend prefabs
        /// </summary>
        [SerializeField]
        private GameObject friendsParent;

        /// <summary>
        /// What is the user identification for the user
        /// </summary>
        [SerializeField]
        private TMP_InputField userIdentificationInput;

        /// <summary>
        /// The friend prefab
        /// </summary>
        [SerializeField]
        private GameObject friendPrefab;

        /// <summary>
        /// Modal window reference
        /// </summary>
        [SerializeField]
        private ModalWindow modalWindow;

        /// <summary>
        /// Should we display the pending friends
        /// </summary>
        private bool showPending = true;

        /// <summary>
        /// Changed event for pending checkbox
        /// Loads/Unloads the pending friend requests
        /// </summary>
        /// <param name="newValue">State of the checkbox</param>
        public void ShowPendingChanged(bool newValue)
        {
            this.showPending = newValue;
            LoadFriends();
        }

        /// <summary>
        /// Shows the profile window
        /// </summary>
        public void Show()
        {
            this.gameObject.SetActive(true);
            LoadFriends();
        }

        /// <summary>
        /// Sends a friend request to the user which was specified in the useridentification input
        /// </summary>
        public void SendFriendRequest()
        {
            if (userIdentificationInput.text == "")
            {
                return;
            }
            //Send request to server
            CoroutineRunner.RunCoroutine(Server.SendPostRequest<Friend>(ServerConfig.PATH_FOR_FRIEND_REQUEST_SEND, userIdentificationInput.text, onComplete: SentFriendRequest, onFailedAction: ShowRequestFail));
        }

        /// <summary>
        /// Show api request failure
        /// </summary>
        /// <param name="content">Server response</param>
        private void ShowRequestFail(string content)
        {
            modalWindow.Show("Request fail", content);
        }

        /// <summary>
        /// After the friend request was sent load the friend prefab if it can show pending
        /// </summary>
        /// <param name="friend">The friend to display</param>
        private void SentFriendRequest(Friend friend)
        {
            if (showPending)
            {
                FriendHandler friendDisplay = Instantiate(friendPrefab, this.friendsParent.transform).GetComponent<FriendHandler>();
                friendDisplay.InitValues(friend);
            }
        }

        /// <summary>
        /// Hides the friend window
        /// </summary>
        public void Hide()
        {
            this.gameObject.SetActive(false);
        }

        /// <summary>
        /// Loads the friends from the server
        /// </summary>
        private void LoadFriends()
        {
            CoroutineRunner.RunCoroutine(Server.SendGetRequest<List<Friend>>(ServerConfig.PATH_FOR_FRIEND_GETALL, onComplete: DisplayFriends));
        }

        /// <summary>
        /// Display the friends which was recieved from the server
        /// </summary>
        /// <param name="friends">The friend list recieved from the server</param>
        private void DisplayFriends(List<Friend> friends)
        {
            try
            {
                //Delete the previous ones
                for (int i = 0; i < this.friendsParent.transform.childCount; i++)
                {
                    Destroy(this.friendsParent.transform.GetChild(i).gameObject);
                }
                this.friendsParent.transform.DetachChildren();
                //Create the prefabs
                foreach (var item in friends)
                {
                    if (showPending)
                    {
                        FriendHandler friendDisplay = Instantiate(friendPrefab, this.friendsParent.transform).GetComponent<FriendHandler>();
                        friendDisplay.InitValues(item);
                    }
                    else
                    {
                        if (!item.Pending)
                        {
                            FriendHandler friendDisplay = Instantiate(friendPrefab, this.friendsParent.transform).GetComponent<FriendHandler>();
                            friendDisplay.InitValues(item);
                        }
                    }
                }
            }
            catch (MissingReferenceException)
            {
                Debug.LogWarning("Friend window tried to reference destroyed object");
                return;
            }
        }
    }
}