using Config;
using System;
using System.Collections;
using System.Collections.Generic;
using Thesis_backend.Data_Structures;
using TMPro;
using UnityEngine;

public class FriendWindowHandler : ThreadSafeMonoBehaviour
{
    [SerializeField]
    private GameObject FriendsContainer;

    [SerializeField]
    private TMP_InputField userIdentificationInput;

    [SerializeField]
    private GameObject friendPrefab;

    [SerializeField]
    private ModalWindow ModalWindow;

    private bool showPending = true;

    public void ShowPendingChanged(bool newValue)
    {
        this.showPending = newValue;
        LoadFriends();
    }

    public void Show()
    {
        this.gameObject.SetActive(true);
        LoadFriends();
    }

    public void SendFriendRequest()
    {
        if (userIdentificationInput.text == "")
        {
            return;
        }

        CoroutineRunner.RunCoroutine(Server.SendPostRequest<Friend>(ServerConfig.PATH_FOR_FRIEND_REQUEST_SEND, userIdentificationInput.text, onComplete: SentFriendRequest, onFailedAction: ShowRequestFail));
    }

    private void ShowRequestFail(string content)
    {
        ModalWindow.Show("Request fail", content);
    }

    private void SentFriendRequest(Friend friend)
    {
        if (showPending)
        {
            FriendHandler friendDisplay = Instantiate(friendPrefab, this.FriendsContainer.transform).GetComponent<FriendHandler>();
            friendDisplay.InitValues(friend);
        }
    }

    public void Hide()
    {
        this.gameObject.SetActive(false);
    }

    private void LoadFriends()
    {
        CoroutineRunner.RunCoroutine(Server.SendGetRequest<List<Friend>>(ServerConfig.PATH_FOR_FRIEND_GETALL, onComplete: DisplayFriends));
    }

    private void DisplayFriends(List<Friend> friends)
    {
        //Delete the previous ones
        for (int i = 0; i < this.FriendsContainer.transform.childCount; i++)
        {
            Destroy(this.FriendsContainer.transform.GetChild(i).gameObject);
        }
        this.FriendsContainer.transform.DetachChildren();

        foreach (var item in friends)
        {
            if (showPending)
            {
                FriendHandler friendDisplay = Instantiate(friendPrefab, this.FriendsContainer.transform).GetComponent<FriendHandler>();
                friendDisplay.InitValues(item);
            }
            else
            {
                if (!item.Pending)
                {
                    FriendHandler friendDisplay = Instantiate(friendPrefab, this.FriendsContainer.transform).GetComponent<FriendHandler>();
                    friendDisplay.InitValues(item);
                }
            }
        }
    }
}