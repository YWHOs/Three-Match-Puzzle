using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelCollect : Level
{
    public CollectGoal[] collectGoal;

    public void UpdateGoal(CandyPiece _piece)
    {
        if(_piece != null)
        {
            foreach(CollectGoal goal in collectGoal)
            {
                if(goal != null)
                {
                    goal.CollectPiece(_piece);
                }
            }
        }
        UpdateUI();
    }
    public void UpdateUI()
    {
        if(UIManager.Instance != null)
        {
            UIManager.Instance.UpdateCollection();
        }
    }
    bool IsGoal(CollectGoal[] _goals)
    {
        foreach(CollectGoal goal in _goals)
        {
            if(goal == null || _goals == null) { return false; }
            if(_goals.Length == 0) { return false; }
            if(goal.numberCollect != 0)
            {
                return false;
            }
        }
        return true;
    }
    public override bool IsWin()
    {
        if(ScoreManager.Instance != null)
        {
            return (ScoreManager.Instance.CurrentScore >= scoreGoal[0] && IsGoal(collectGoal));
        }
        return false;
    }

    public override bool IsGameOver()
    {
        if (IsGoal(collectGoal))
        {
            int max = scoreGoal[scoreGoal.Length - 1];
            if(ScoreManager.Instance.CurrentScore >= max)
            {
                return true;
            }
        }
        if(levelCounter == LevelCounter.Timer)
        {
            return (timeLeft <= 0);
        }
        else
        {
            return (moveLeft <= 0);
        }
    }
}
