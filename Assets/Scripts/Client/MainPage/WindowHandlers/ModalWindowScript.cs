using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ModalWindow : MonoBehaviour
{
    [SerializeField]
    private TMP_Text ModalTitleText;

    [SerializeField]
    private TMP_Text ModalContentText;

    private Action onOkAction;

    public void Show()
    {
        this.gameObject.SetActive(true);
    }

    public void Show(string title, string content, Action onOkAction = null)
    {
        this.ModalTitleText.text = title;
        this.ModalContentText.text = content;
        this.onOkAction = onOkAction;
        Show();
    }

    public void Hide()
    {
        this.gameObject.SetActive(false);
    }

    public void ConfirmButtonClick()
    {
        if (onOkAction is not null)
        {
            onOkAction();
        }
        Hide();
    }
}