using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelTime : Level
{
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
}
