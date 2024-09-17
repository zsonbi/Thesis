using Config;
using Thesis_backend.Data_Structures;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FriendDisplayHandler : MonoBehaviour
{
    [SerializeField]
    private TMP_Text FriendNameLabel;

    [SerializeField]
    private TMP_Text ScoreText;

    [SerializeField]
    private Button AcceptButton;

    public Friend Friend { get; private set; }

    public void Update()
    {
    }

    public void InitValues(Friend friend)
    {
        this.Friend = friend;
        this.AcceptButton.gameObject.SetActive(Friend.Pending);
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
    }

    private void Deleted(Thesis_backend.Data_Structures.PlayerTask result)
    {
        Destroy(this.gameObject);
    }
}