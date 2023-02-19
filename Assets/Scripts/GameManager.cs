using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public int moveLeft = 30;
        int moveLeftScore = 500;
    [SerializeField] int scoreGoal = 5000;

    [SerializeField] Text moveText;
    FadeManager fadeManager;
    Board board;
    MessageUI messageUI;
    [SerializeField] Sprite winSprite;
    [SerializeField] Sprite loseSprite;
    [SerializeField] Sprite goalSprite;

    [SerializeField] GameObject menu;

    bool isStart;
    public bool isGameOver;
    bool isWin;
    bool isReload;
    public static GameManager instance;
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }
    void Start()
    {
        fadeManager = FindObjectOfType<FadeManager>();
        messageUI = FindObjectOfType<MessageUI>();
        board = FindObjectOfType<Board>();
        Move();
        StartCoroutine(GameLoop());
    }

    public void Move()
    {
        if(moveText != null)
        {
            moveText.text = moveLeft.ToString();
        }
    }
    public void StartGameButton()
    {
        isStart = true;
    }
    IEnumerator GameLoop()
    {
        yield return StartCoroutine("StartCoroutine");
        yield return StartCoroutine("PlayCoroutine");
        yield return StartCoroutine("EndCoroutine");
    }
    IEnumerator StartCoroutine()
    {
        if(messageUI != null)
        {
            messageUI.GetComponent<RectXMove>().MoveOn();
            messageUI.ShowMessage(goalSprite, "SCORE GOAL\n" + scoreGoal.ToString(), "START");
        }
        while (!isStart)
        {
            yield return null;
        }
        if(fadeManager != null)
        {
            fadeManager.FadeOut();
        }
        yield return new WaitUntil(() => !fadeManager.isFade);
        if(board != null)
        {
            board.SetupBoard();
        }
    }
    IEnumerator PlayCoroutine()
    {
        while (!isGameOver)
        {
            if(ScoreManager.instance != null)
            {
                // ��ǥ���� �޼�
                if(ScoreManager.instance.CurrentScore >= scoreGoal)
                {
                    isGameOver = true;
                    isWin = true;
                }
            }
            if(moveLeft == 0)
            {
                isGameOver = true;
                isWin = false;
            }
            yield return null;
        }
    }
    IEnumerator EndCoroutine()
    {
        isReload = false;
        if (isWin)
        {

            if (messageUI != null)
            {
                messageUI.GetComponent<RectXMove>().MoveOn();
                messageUI.ShowMessage(winSprite, "WIN!!", "OK");
            }
            if(AudioManager.instance != null)
            {
                AudioManager.instance.PlayWin();
            }
        }
        else
        {
            if (messageUI != null)
            {
                messageUI.GetComponent<RectXMove>().MoveOn();
                messageUI.ShowMessage(loseSprite, "YOU LOSE..", "OK");
            }
            if (AudioManager.instance != null)
            {
                AudioManager.instance.PlayLose();
            }
        }
        while (!isReload)
        {
            yield return null;
        }
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void ReloadButton()
    {
        isReload = true;
    }

    public void MenuButton()
    {
        if (menu.activeSelf == false)
            menu.SetActive(true);
        else
            menu.SetActive(false);
    }
    public void MenuStartButton()
    {
        menu.SetActive(false);
    }
    public void MenuReloadButton()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void QuitButton()
    {
        Application.Quit();
    }
}