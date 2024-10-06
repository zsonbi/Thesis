using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ModalWindow : ThreadSafeMonoBehaviour
{
    [SerializeField]
    private TMP_Text ModalTitleText;

    [SerializeField]
    private TMP_Text ModalContentText;

    private Action onOkAction;

    public void Show()
    {
        if (this.Destroyed)
        {
            return;
        }
        try
        {
            this.gameObject.SetActive(true);
        }
        catch (MissingReferenceException)
        {
            return;
        }
    }

    public void Show(string title, string content, Action onOkAction = null)
    {
        if (this.Destroyed)
        {
            return;
        }
        try
        {
            this.ModalTitleText.text = title;
            this.ModalContentText.text = content;
            this.onOkAction = onOkAction;
       

            Show();
        }
        catch (MissingReferenceException)
        {
            return;
        }
    }

    public void Hide()
    {
        if (this.Destroyed)
        {
            return;
        }
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