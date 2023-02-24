using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreSlide : MonoBehaviour
{
    [SerializeField] Slider slider;
    [SerializeField] ScoreStar[] scoreStar = new ScoreStar[3];
    Level level;
    int max;

    private void Awake()
    {
        slider = GetComponent<Slider>();
    }

    public void SetupStar(Level _level)
    {
        if(_level == null) { return; }
        level = _level;

        max = level.scoreGoal[level.scoreGoal.Length - 1];
        float sliderWidth = slider.GetComponent<RectTransform>().rect.width;

        if(max > 0)
        {
            for (int i = 0; i < level.scoreGoal.Length; i++)
            {
                if(scoreStar[i] != null)
                {
                    float x = (sliderWidth * level.scoreGoal[i] / max) - (sliderWidth * 0.5f);
                    RectTransform rect = scoreStar[i].GetComponent<RectTransform>();
                    rect.anchoredPosition = new Vector2(x, rect.anchoredPosition.y);
                }
            }
        }
    }

    public void UpdateScore(int _score, int _count)
    {
        if(level != null)
        {
            slider.value = (float)_score / (float)max;
        }
        for (int i = 0; i < _count; i++)
        {
            if(scoreStar[i] != null)
            {
                scoreStar[i].Active();
            }
        }
    }
}
