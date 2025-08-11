using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class WebGLMobileKeyboardManager : MonoBehaviour
{
#if UNITY_WEBGL && !UNITY_EDITOR
    private TMP_InputField currentField;

    void Update()
    {
        // Detect if selection changed
        if (EventSystem.current.currentSelectedGameObject != null)
        {
            var newField = EventSystem.current.currentSelectedGameObject.GetComponent<TMP_InputField>();
            if (newField != null && newField != currentField)
            {
                currentField = newField;
                // Send full field data to HTML
                string placeholder = currentField.placeholder is TextMeshProUGUI ph ? ph.text : "";
                string contentType = currentField.contentType.ToString(); // For HTML input type decision

                string json = JsonUtility.ToJson(new FieldData
                {
                    text = currentField.text,
                    placeholder = placeholder,
                    contentType = contentType,
                    multiline = currentField.multiLine
                });

                Application.ExternalCall("ShowMobileKeyboardAuto", json);
            }
        }
        else if (currentField != null)
        {
            // Lost focus
            Application.ExternalCall("HideMobileKeyboard");
            currentField = null;
        }
    }

    // Called from HTML when text changes
    public void OnKeyboardInput(string newText)
    {
        if (currentField != null)
            currentField.text = newText;
    }

    [System.Serializable]
    private class FieldData
    {
        public string text;
        public string placeholder;
        public string contentType;
        public bool multiline;
    }
#endif
}