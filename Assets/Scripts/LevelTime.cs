using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelTime : Level
{
    public Timer timer;
    int max;

    private void Start()
    {
        timer.InitTime(timeLeft);
        max = timeLeft;
    }
    public void CountTime()
    {
        StartCoroutine(CountTimeCoroutine());
    }
    IEnumerator CountTimeCoroutine()
    {
        while(timeLeft > 0)
        {
            yield return new WaitForSeconds(1f);
            timeLeft--;

            timer.UpdateTime(timeLeft);
        }
    }
    public override bool IsWin()
    {
        if (ScoreManager.Instance != null)
        {
            return (ScoreManager.Instance.CurrentScore >= scoreGoal[0]);
        }
        return false;
    }
    public override bool IsGameOver()
    {
        int max = scoreGoal[scoreGoal.Length - 1];
        if (ScoreManager.Instance.CurrentScore >= max)
        {
            return true;
        }
        return (timeLeft <= 0);
    }

    public void AddTime(int _time)
    {
        timeLeft += _time;
        timeLeft = Mathf.Clamp(timeLeft, 0, max);

        timer.UpdateTime(timeLeft);
    }
}
