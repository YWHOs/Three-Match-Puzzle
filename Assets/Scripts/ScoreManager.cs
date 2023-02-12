using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    int currentScore;
    public int CurrentScore { get { return currentScore; } }
    int counterValue;
    int increase = 5;

    [SerializeField] Text scoreText;
    //[SerializeField] float time = 1f;

    public static ScoreManager instance;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }
    // Start is called before the first frame update
    void Start()
    {
        ScoreText(currentScore);
    }

    public void ScoreText(int _score)
    {
        if(scoreText != null)
        {
            scoreText.text = _score.ToString();
        }
    }
    public void AddScore(int _score)
    {
        currentScore += _score;
        StartCoroutine(ScoreCoroutine());
    }
    IEnumerator ScoreCoroutine()
    {
        int index = 0;
        while(counterValue < currentScore && index < 10000)
        {
            counterValue += increase;
            ScoreText(counterValue);
            index++;
            yield return null;
        }

        counterValue = currentScore;
        ScoreText(currentScore);
    }

}
