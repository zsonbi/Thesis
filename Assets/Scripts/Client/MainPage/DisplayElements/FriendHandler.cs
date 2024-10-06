using Config;
using Thesis_backend.Data_Structures;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using User;

namespace MainPage
{
    public class FriendHandler : ThreadSafeMonoBehaviour
    {
        [SerializeField]
        private TMP_Text friendNameLabel;

        [SerializeField]
        private TMP_Text scoreText;

        [SerializeField]
        private TMP_Text scoreLabelText;

        [SerializeField]
        private Button acceptButton;

        [SerializeField]
        private TMP_Text pendingtext;

        public Friend Friend { get; private set; }

        public void Update()
        {
        }

        public void InitValues(Friend friend)
        {
            this.Friend = friend;
            this.acceptButton.gameObject.SetActive(Friend.Pending);
            if (UserData.Instance.Id != this.Friend.Sender.ID)
            {
                friendNameLabel.text = this.Friend.Sender.Username;
                scoreText.text = this.Friend.Sender.TotalScore.ToString();
                if (Friend.Pending)
                {
                    acceptButton.gameObject.SetActive(true);
                }
            }
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

        public void AcceptFriendRequest()
        {
            CoroutineRunner.RunCoroutine(Server.SendPatchRequest<Thesis_backend.Data_Structures.Friend>(ServerConfig.PATH_FOR_FRIEND_ACCEPT(this.Friend.ID), onComplete: Accepted));
        }

        public void DeleteFriend()
        {
            CoroutineRunner.RunCoroutine(Server.SendDeleteRequest<string>(ServerConfig.PATH_FOR_FRIEND_DELETE(this.Friend.ID), onComplete: Deleted));
        }

        private void Accepted(Thesis_backend.Data_Structures.Friend result)
        {
            this.Friend.Pending = false;
            this.acceptButton.gameObject.SetActive(false);
            this.scoreText.gameObject.SetActive(true);
            this.scoreLabelText.gameObject.SetActive(true);
        }

        private void Deleted(string result)
        {
            if (result == "Deleted")
            {
                Destroy(this.gameObject);
            }
        }
    }
}