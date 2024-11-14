using System;
using TMPro;
using UnityEngine;

namespace Utility
{
    /// <summary>
    /// Modal window for alerting the user
    /// </summary>
    public class ModalWindow : ThreadSafeMonoBehaviour
    {
        /// <summary>
        /// Reference to the modal title's text object
        /// </summary>
        [SerializeField]
        private TMP_Text ModalTitleText;

        /// <summary>
        /// Reference to the modal content's text object
        /// </summary>
        [SerializeField]
        private TMP_Text ModalContentText;

        /// <summary>
        /// What to do when the user presses the ok button
        /// </summary>
        private Action onOkAction;

        /// <summary>
        /// Show the modal window
        /// </summary>
        public void Show()
        {
            if (this.Destroyed)
            {
                return;
            }
            //Error detection if it been destroyed somehow... can only happen in testing...
            try
            {
                this.gameObject.SetActive(true);
            }
            catch (MissingReferenceException)
            {
                return;
            }
        }

        /// <summary>
        /// Show the modal window with custom content
        /// </summary>
        /// <param name="title">The modal window's title</param>
        /// <param name="content">What should the content be</param>
        /// <param name="onOkAction">What to do when the ok buttton is pressed</param>
        public void Show(string title, string content, Action onOkAction = null)
        {
            if (this.Destroyed)
            {
                return;
            }
            //Error detection if it been destroyed somehow... can only happen in testing...
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

        /// <summary>
        /// Hides the modal window
        /// </summary>
        public void Hide()
        {
            if (this.Destroyed)
            {
                return;
            }
            this.gameObject.SetActive(false);
        }

        /// <summary>
        /// Action for ok buttton
        /// </summary>
        public void ConfirmButtonClick()
        {
            if (onOkAction is not null)
            {
                onOkAction();
            }
            Hide();
        }
    }
}