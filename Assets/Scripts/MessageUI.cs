using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MessageUI : MonoBehaviour
{
    [SerializeField] Image icon;
    [SerializeField] Text messageText;
    [SerializeField] Text buttonText;

    public void ShowMessage(Sprite _sprite = null, string _message = "", string _button = "START")
    {
        if(icon != null)
        {
            icon.sprite = _sprite;
        }
        if(messageText != null)
        {
            messageText.text = _message;
        }
        if(buttonText != null)
        {
            buttonText.text = _button;
        }
    }
}
