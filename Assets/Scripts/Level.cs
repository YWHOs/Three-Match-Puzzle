using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LevelCounter
{
    Timer,
    Move
}
public abstract class Level : Singleton<Level>
{
    public int scoreStar;
    public int[] scoreGoal = new int[3] { 1000, 2000, 3000 };
    public int moveLeft = 30;
    public int timeLeft = 60;
    int maxTime;

    public LevelCounter levelCounter = LevelCounter.Move;
    public virtual void Start()
    {
        Init();

        if(levelCounter == LevelCounter.Timer)
        {
            maxTime = timeLeft;
            if (UIManager.Instance != null && UIManager.Instance.timer != null)
                UIManager.Instance.timer.InitTime(timeLeft);
        }


    }
    void Init()
    {
        scoreStar = 0;
        for (int i = 1; i < scoreGoal.Length; i++)
        {
            if(scoreGoal[i] < scoreGoal[i - 1])
            {

            }
        }
    }
    int UpdateScore(int _score)
    {
        for (int i = 0; i < scoreGoal.Length; i++)
        {
            if(_score < scoreGoal[i])
            {
                return i;
            }
        }
        return scoreGoal.Length;
    }

    public void UpdateScoreStar(int _score)
    {
        scoreStar = UpdateScore(_score);
    }

    public abstract bool IsWin();
    public abstract bool IsGameOver();


    // Level Time
    public void CountTime()
    {
        StartCoroutine(CountTimeCoroutine());
    }
    IEnumerator CountTimeCoroutine()
    {
        while (timeLeft > 0)
        {
            yield return new WaitForSeconds(1f);
            timeLeft--;

            if (UIManager.Instance != null && UIManager.Instance.timer != null)
                UIManager.Instance.timer.UpdateTime(timeLeft);
        }
    }
    public void AddTime(int _time)
    {
        timeLeft += _time;
        timeLeft = Mathf.Clamp(timeLeft, 0, maxTime);

        if (UIManager.Instance != null && UIManager.Instance.timer != null)
            UIManager.Instance.timer.UpdateTime(timeLeft);
    }

}
