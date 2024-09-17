using Config;
using System;
using System.Collections;
using System.Collections.Generic;
using Thesis_backend.Data_Structures;
using TMPro;
using UnityEngine;

public class FriendHandler : MonoBehaviour
{
    [SerializeField]
    private GameObject FriendsContainer;

    [SerializeField]
    private TMP_InputField userIdentificationInput;

    [SerializeField]
    private GameObject friendPrefab;

    private bool showPending = true;

    public void ShowPendingChanged(Boolean newValue)
    {
        this.showPending = newValue;
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

        StartCoroutine(Server.SendPostRequest<Friend>(ServerConfig.PATH_FOR_FRIEND_REQUEST_SEND, userIdentificationInput.text, onComplete: SentFriendRequest));
    }

    private void SentFriendRequest(Friend friend)
    {
    }

    public void Hide()
    {
        this.gameObject.SetActive(false);
    }

    private void LoadFriends()
    {
        StartCoroutine(Server.SendGetRequest<List<Friend>>(ServerConfig.PATH_FOR_FRIEND_GETALL, onComplete: DisplayFriends));
    }

    private void DisplayFriends(List<Friend> friends)
    {
        foreach (var item in friends)
        {
            FriendDisplayHandler friendDisplay = Instantiate(friendPrefab, this.FriendsContainer.transform).GetComponent<FriendDisplayHandler>();
            friendDisplay.InitValues(item);
        }
    }
}