using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelScore : Level
{
    public override bool IsWin()
    {
        if(ScoreManager.Instance != null)
        {
            return (ScoreManager.Instance.CurrentScore >= scoreGoal[0]);
        }
        return false;
    }
    public override bool IsGameOver()
    {
        int max = scoreGoal[scoreGoal.Length - 1];
        if(ScoreManager.Instance.CurrentScore >= max)
        {
            return true;
        }
        return (moveLeft == 0);
    }
}
