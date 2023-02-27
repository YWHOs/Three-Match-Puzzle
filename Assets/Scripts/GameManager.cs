using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Level))]
public class GameManager : Singleton<GameManager>
{
    Board board;
    Level level;
    LevelCollect levelCollect;
    public Level Level { get { return level; } }


    [SerializeField] GameObject menu;

    bool isStart;
    public bool isGameOver;
    bool isWin;
    bool isReload;

    public override void Awake()
    {
        base.Awake();
        level = GetComponent<Level>();
        levelCollect = GetComponent<LevelCollect>();
        board = FindObjectOfType<Board>().GetComponent<Board>();
    }
    void Start()
    {
        if(UIManager.Instance != null)
        {
            if (UIManager.Instance.scoreSlide != null)
            {
                UIManager.Instance.scoreSlide.SetupStar(level);
            }
            if (levelCollect != null)
            {
                UIManager.Instance.EnableCollection(true);
                UIManager.Instance.Setup(levelCollect.collectGoal);
            }
            else
            {
                UIManager.Instance.EnableCollection(false);
            }

            bool isTimer = (level.levelCounter == LevelCounter.Timer);

            UIManager.Instance.EnableTimer(isTimer);
            UIManager.Instance.EnableMove(!isTimer);
        }

        level.moveLeft++;
        Move();
        StartCoroutine(GameLoop());
    }

    public void Move()
    {
        if(level.levelCounter == LevelCounter.Move)
        {
            level.moveLeft--;
            if (UIManager.Instance.moveText != null)
            {
                UIManager.Instance.moveText.text = level.moveLeft.ToString();
            }
        }
        //else
        //{
        //    if(UIManager.Instance.moveText != null)
        //    {
        //        // ¹«ÇÑ´ë TEXT
        //        UIManager.Instance.moveText.text = "\u221E";
        //        UIManager.Instance.moveText.fontSize = 70;
        //    }
        //}

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
        if(UIManager.Instance.messageUI != null)
        {
            UIManager.Instance.messageUI.GetComponent<RectXMove>().MoveOn();
            int max = level.scoreGoal.Length - 1;
            UIManager.Instance.messageUI.ShowScore(level.scoreGoal[max]);
        }
        while (!isStart)
        {
            yield return null;
        }
        if(UIManager.Instance.fadeManager != null)
        {
            UIManager.Instance.fadeManager.FadeOut();
        }
        yield return new WaitUntil(() => !UIManager.Instance.fadeManager.isFade);
        if(board != null)
        {
            board.SetupBoard();
        }
    }
    IEnumerator PlayCoroutine()
    {
        if(level.levelCounter == LevelCounter.Timer)
        {
            level.CountTime();
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

            if (UIManager.Instance.messageUI != null)
            {
                UIManager.Instance.messageUI.GetComponent<RectXMove>().MoveOn();
                UIManager.Instance.messageUI.ShowWin();
            }
            if(AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayWin();
            }
        }
        else
        {
            if (UIManager.Instance.messageUI != null)
            {
                UIManager.Instance.messageUI.GetComponent<RectXMove>().MoveOn();
                UIManager.Instance.messageUI.ShowLose();
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
        if(level.levelCounter == LevelCounter.Timer && UIManager.Instance != null && UIManager.Instance.timer != null)
        {
            UIManager.Instance.timer.StopFlash();
            UIManager.Instance.timer.isPaused = true;
        }

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
                if(UIManager.Instance.scoreSlide != null)
                {
                    UIManager.Instance.scoreSlide.UpdateScore(ScoreManager.Instance.CurrentScore, level.scoreStar);
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
        if(level.levelCounter == LevelCounter.Timer)
        {
            level.AddTime(_time);
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
