using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CandyPiece : MonoBehaviour
{
    [SerializeField] int xIndex;
    [SerializeField] int yIndex;

    bool isMoving;
    Board board;
    public Interpolation interpolation = Interpolation.EaseOut;
    public enum Interpolation
    {
        Linear,
        EaseOut,
        EaseIn
    };
    public MatchValue matchValue;
    public enum MatchValue
    {
        Blue,
        Green,
        Orange,
        Purple,
        Red
    };
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            Move((int)transform.position.x + 1, (int)transform.position.y, 0.5f);
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Move((int)transform.position.x - 1, (int)transform.position.y, 0.5f);
        }
    }
    public void Init(Board _board)
    {
        board = _board;
    }
    public void SetCandy(int _xIndex, int _yIndex)
    {
        xIndex = _xIndex;
        yIndex = _yIndex;
    }

    public void Move(int _x, int _y, float _time)
    {
        if(!isMoving)
            StartCoroutine(MoveCoroutine(new Vector2(_x, _y), _time));
    }
    
    IEnumerator MoveCoroutine(Vector2 _destination, float _time)
    {
        Vector2 startPosition = transform.position;
        bool isReach = false;
        float elapsedTime = 0f;
        isMoving = true;
        while (!isReach)
        {
            if(Vector2.Distance(transform.position, _destination) < 0.01f)
            {
                isReach = true;
                //transform.position = _destination;
                //SetCandy((int)_destination.x, (int)_destination.y);
                if(board != null)
                {
                    board.PlaceCandy(this, (int)_destination.x, (int)_destination.y);
                }
                break;
            }
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / _time;

            // 선형보간 (부드러운 효과)
            switch (interpolation)
            {
                case Interpolation.Linear:
                    break;
                case Interpolation.EaseOut:
                    t = Mathf.Sin(t * Mathf.PI * 0.5f);
                    break;
                case Interpolation.EaseIn:
                    t = 1 - Mathf.Cos(t * Mathf.PI * 0.5f);
                    break;
            }

            transform.position = Vector2.Lerp(startPosition, _destination, t);
            yield return null;
        }
        isMoving = false;
    }
}
