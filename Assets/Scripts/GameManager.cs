using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Level))]
public class GameManager : Singleton<GameManager>
{
    // public int moveLeft = 30;
    // [SerializeField] int scoreGoal = 5000;

    [SerializeField] Text moveText;
    FadeManager fadeManager;
    Board board;
    MessageUI messageUI;
    Level level;
    LevelCollect levelCollect;
    LevelTime levelTime;
    public LevelTime LevelTime { get { return levelTime; } }
    [SerializeField] ScoreSlide scoreSlide;
    [SerializeField] Sprite winSprite;
    [SerializeField] Sprite loseSprite;
    [SerializeField] Sprite goalSprite;

    [SerializeField] GameObject menu;

    bool isStart;
    public bool isGameOver;
    bool isWin;
    bool isReload;

    public override void Awake()
    {
        base.Awake();
        level = GetComponent<Level>();
        levelTime = GetComponent<LevelTime>();
        levelCollect = GetComponent<LevelCollect>();
        board = FindObjectOfType<Board>().GetComponent<Board>();
    }
    void Start()
    {
        fadeManager = FindObjectOfType<FadeManager>();
        messageUI = FindObjectOfType<MessageUI>();

        scoreSlide.SetupStar(level);

        level.moveLeft++;
        Move();
        StartCoroutine(GameLoop());
    }

    public void Move()
    {
        if(levelTime == null)
        {
            level.moveLeft--;
            if (moveText != null)
            {
                moveText.text = level.moveLeft.ToString();
            }
        }
        else
        {
            if(moveText != null)
            {
                // ¹«ÇÑ´ë TEXT
                moveText.text = "\u221E";
                moveText.fontSize = 70;
            }
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

        yield return StartCoroutine("WaitBoardCoroutine", 0.5f);
        yield return StartCoroutine("EndCoroutine");
    }
    IEnumerator StartCoroutine()
    {
        if(messageUI != null)
        {
            messageUI.GetComponent<RectXMove>().MoveOn();
            messageUI.ShowMessage(goalSprite, "SCORE GOAL\n" + level.scoreGoal[0].ToString(), "START");
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
        if(levelTime != null)
        {
            levelTime.CountTime();
        }
        while (!isGameOver)
        {
            isWin = level.IsWin();
            isGameOver = level.IsGameOver();
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
            if(AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayWin();
            }
        }
        else
        {
            if (messageUI != null)
            {
                messageUI.GetComponent<RectXMove>().MoveOn();
                messageUI.ShowMessage(loseSprite, "YOU LOSE..", "OK");
            }
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayLose();
            }
        }
        while (!isReload)
        {
            yield return null;
        }
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    IEnumerator WaitBoardCoroutine(float _delay)
    {
        levelTime.timer.StopFlash();
        levelTime.timer.isPaused = true;
        if(board != null)
        {
            yield return new WaitForSeconds(0.5f);
            while (board.isRefilling)
            {
                yield return null;
            }
        }
        yield return new WaitForSeconds(_delay);
    }
    public void Score(CandyPiece _piece)
    {
        if(_piece != null)
        {
            if (ScoreManager.Instance != null)
            {
                ScoreManager.Instance.AddScore(_piece.score);
                level.UpdateScoreStar(ScoreManager.Instance.CurrentScore);
                if(scoreSlide != null)
                {
                    scoreSlide.UpdateScore(ScoreManager.Instance.CurrentScore, level.scoreStar);
                }
            }
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayAudio(_piece.sound, 0.2f);
            }
        }

    }
    public void AddTime(int _time)
    {
        if(levelTime != null)
        {
            levelTime.AddTime(_time);
        }

    }
    public void UpdateCollectGoal(CandyPiece _piece)
    {
        levelCollect.UpdateGoal(_piece);
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
