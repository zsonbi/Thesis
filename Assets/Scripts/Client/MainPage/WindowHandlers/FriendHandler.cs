using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FriendHandler : MonoBehaviour
{



    [SerializeField]
    private GameObject FriendsContainer;

    [SerializeField]
    private TMP_InputField userIdentificationInput;


    private bool showPending = true;

    public void ShowPendingChanged(Boolean newValue)
    {
        this.showPending = newValue;
    }

    public void Show()
    {

        this.gameObject.SetActive(true);
        LoadFromApi();
    }


    public void SendFriendRequest()
    {

    }

    public void Hide()
    {
        this.gameObject.SetActive(false);
    }

    private void LoadFromApi()
    {

    }

}
