using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : Singleton<Level>
{
    public int scoreStar;
    public int[] scoreGoal = new int[3] { 1000, 2000, 3000 };
    public int moveLeft = 30;

    void Start()
    {
        Init();
    }
    public void Init()
    {
        scoreStar = 0;
        for (int i = 1; i < scoreGoal.Length; i++)
        {
            if(scoreGoal[i] < scoreGoal[i - 1])
            {

            }
        }
    }
    public int UpdateScore(int _score)
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

    public void UpdateScoreStart(int _score)
    {
        scoreStar = UpdateScore(_score);
    }
}
