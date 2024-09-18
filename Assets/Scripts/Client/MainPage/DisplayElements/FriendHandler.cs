using Config;
using Thesis_backend.Data_Structures;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using User;

public class FriendHandler : MonoBehaviour
{
    [SerializeField]
    private TMP_Text FriendNameLabel;

    [SerializeField]
    private TMP_Text ScoreText;

    [SerializeField]
    private TMP_Text ScoreLabelText;

    [SerializeField]
    private Button AcceptButton;

    [SerializeField]
    private TMP_Text Pendingtext;

    public Friend Friend { get; private set; }

    public void Update()
    {
    }

    public void InitValues(Friend friend)
    {
        this.Friend = friend;
        this.AcceptButton.gameObject.SetActive(Friend.Pending);
        if (UserData.Instance.Id != this.Friend.Sender.ID)
        {
            FriendNameLabel.text = this.Friend.Sender.Username;
            ScoreText.text = this.Friend.Sender.TotalScore.ToString();
        }
        else
        {
            FriendNameLabel.text = this.Friend.Receiver.Username;
            ScoreText.text = this.Friend.Receiver.TotalScore.ToString();

            if (friend.Pending)
            {
                AcceptButton.gameObject.SetActive(false);
                Pendingtext.gameObject.SetActive(true);
            }
        }
        this.ScoreText.gameObject.SetActive(!friend.Pending);
        this.ScoreLabelText.gameObject.SetActive(!friend.Pending);
    }

    public void AcceptFriendRequest()
    {
        StartCoroutine(Server.SendPatchRequest<Thesis_backend.Data_Structures.Friend>(ServerConfig.PATH_FOR_FRIEND_ACCEPT(this.Friend.ID), onComplete: Accepted));
    }

    public void DeleteFriend()
    {
        StartCoroutine(Server.SendDeleteRequest<Thesis_backend.Data_Structures.Friend>(ServerConfig.PATH_FOR_FRIEND_DELETE(this.Friend.ID), onComplete: Accepted));
    }

    private void Accepted(Thesis_backend.Data_Structures.Friend result)
    {
        this.Friend.Pending = false;
        this.AcceptButton.gameObject.SetActive(false);
        this.ScoreText.gameObject.SetActive(true);
        this.ScoreLabelText.gameObject.SetActive(true);
    }

    private void Deleted(Thesis_backend.Data_Structures.PlayerTask result)
    {
        Destroy(this.gameObject);
    }
}