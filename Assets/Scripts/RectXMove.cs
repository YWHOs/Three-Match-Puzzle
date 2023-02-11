using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RectXMove : MonoBehaviour
{
    [SerializeField] Vector2 start;
    [SerializeField] Vector2 onScreen;
    [SerializeField] Vector2 end;

    [SerializeField] float time = 1f;

    RectTransform rectTransform;
    bool isMoving;
    // Start is called before the first frame update
    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    void Move(Vector2 _start, Vector2 _end, float _time)
    {
        if (!isMoving)
        {
            StartCoroutine(MoveCoroutine(_start, _end, _time));
        }
    }

    IEnumerator MoveCoroutine(Vector2 _start, Vector2 _end, float _time)
    {
        if(rectTransform != null)
        {
            rectTransform.anchoredPosition = _start;
        }
        bool isReach = false;
        float elapsedTime = 0f;
        isMoving = true;

        while (!isReach)
        {
            if(Vector2.Distance(rectTransform.anchoredPosition, _end) < 0.01f)
            {
                isReach = true;
                break;
            }
            elapsedTime += Time.deltaTime;

            float t = Mathf.Clamp(elapsedTime / _time, 0f, 1f);
            t = Mathf.Sin(t * Mathf.PI * 0.5f);

            if(rectTransform != null)
            {
                rectTransform.anchoredPosition = Vector2.Lerp(_start, _end, t);
            }
            yield return null;
        }
        isMoving = false;
    }

    public void MoveOn()
    {
        Move(start, onScreen, time);
    }
    public void MoveOff()
    {
        Move(onScreen, end, time);
    }
}
