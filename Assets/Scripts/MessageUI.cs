using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MessageUI : MonoBehaviour
{
    [SerializeField] Image icon;
    [SerializeField] Text messageText;
    [SerializeField] Text buttonText;

    [SerializeField] Sprite winSprite;
    [SerializeField] Sprite loseSprite;
    [SerializeField] Sprite goalSprite;

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

    public void ShowScore(int _score)
    {
        string message = "SCORE GAOL\n" + _score.ToString();
        ShowMessage(goalSprite, message, "START");
    }
    public void ShowWin()
    {
        ShowMessage(winSprite, "WIN!!", "OK");
    }
    public void ShowLose()
    {
        ShowMessage(loseSprite, "You Lose..", "OK");
    }
}
