using Config;
using Thesis_backend.Data_Structures;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using User;

namespace MainPage
{
    /// <summary>
    /// Handles a single friend entry display
    /// </summary>
    public class FriendHandler : ThreadSafeMonoBehaviour
    {
        /// <summary>
        /// Label to display the friend's name
        /// </summary>
        [SerializeField]
        private TMP_Text friendNameLabel;

        /// <summary>
        /// Label to display the friend's total score
        /// </summary>
        [SerializeField]
        private TMP_Text scoreText;

        /// <summary>
        /// Label to write out "Score:" referenced so we can hide it or show
        /// </summary>
        [SerializeField]
        private TMP_Text scoreLabelText;

        /// <summary>
        /// Button to accept the friend request
        /// </summary>
        [SerializeField]
        private Button acceptButton;

        /// <summary>
        /// Text to show if the friend request is still pending
        /// </summary>
        [SerializeField]
        private TMP_Text pendingtext;

        /// <summary>
        /// Reference to the friend which is being displayed here
        /// </summary>
        public Friend Friend { get; private set; }

        /// <summary>
        /// Inititializes a new display with the given friend to display
        /// </summary>
        /// <param name="friend">The friend to display</param>
        public void InitValues(Friend friend)
        {
            this.Friend = friend;
            this.acceptButton.gameObject.SetActive(Friend.Pending);
            //Determine which user to show
            if (UserData.Instance.Id != this.Friend.Sender.ID)
            {
                friendNameLabel.text = this.Friend.Sender.Username;
                scoreText.text = this.Friend.Sender.TotalScore.ToString();
                if (Friend.Pending)
                {
                    acceptButton.gameObject.SetActive(true);
                }
            }
            //If the current user is the receiver show him the accept button
            else
            {
                friendNameLabel.text = this.Friend.Receiver.Username;
                scoreText.text = this.Friend.Receiver.TotalScore.ToString();

                if (friend.Pending)
                {
                    acceptButton.gameObject.SetActive(false);
                    pendingtext.gameObject.SetActive(true);
                }
            }
            this.scoreText.gameObject.SetActive(!friend.Pending);
            this.scoreLabelText.gameObject.SetActive(!friend.Pending);
        }

        /// <summary>
        /// Send a request to the server that the user accepted the friend request
        /// </summary>
        public void AcceptFriendRequest()
        {
            CoroutineRunner.RunCoroutine(Server.SendPatchRequest<Thesis_backend.Data_Structures.Friend>(ServerConfig.PATH_FOR_FRIEND_ACCEPT(this.Friend.ID), onComplete: Accepted));
        }

        /// <summary>
        /// Send a request to the server that the user deleted this friend
        /// </summary>
        public void DeleteFriend()
        {
            CoroutineRunner.RunCoroutine(Server.SendDeleteRequest<string>(ServerConfig.PATH_FOR_FRIEND_DELETE(this.Friend.ID), onComplete: Deleted));
        }

        /// <summary>
        /// After the server responded successfully to the friend accept
        /// </summary>
        /// <param name="result">Accepted friend</param>
        private void Accepted(Thesis_backend.Data_Structures.Friend result)
        {
            this.Friend.Pending = false;
            this.acceptButton.gameObject.SetActive(false);
            this.scoreText.gameObject.SetActive(true);
            this.scoreLabelText.gameObject.SetActive(true);
        }

        /// <summary>
        /// After the server responded successfully to the friend delete
        /// </summary>
        /// <param name="result">Delete result</param>
        private void Deleted(string result)
        {
            if (result == "Deleted")
            {
                Destroy(this.gameObject);
            }
        }
    }
}