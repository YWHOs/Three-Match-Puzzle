using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MatchValue
{
    Blue,
    Green,
    Orange,
    Purple,
    Red,
    None // <- Collectible
}
public class CandyPiece : MonoBehaviour
{
    public int xIndex;
    public int yIndex;
    public int score = 100;
    public AudioClip sound;

    public bool isMoving;
    Board board;

    public MatchValue matchValue;

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

            transform.position = Vector2.Lerp(startPosition, _destination, t);
            yield return null;
        }
        isMoving = false;
    }

    public void ChangeColor(CandyPiece _piece)
    {
        SpriteRenderer spriteChange = GetComponent<SpriteRenderer>();
        Color color = Color.clear;
        if(_piece != null)
        {
            SpriteRenderer spriteMatch = _piece.GetComponent<SpriteRenderer>();

            if(spriteMatch != null && spriteChange != null)
            {
                spriteChange.color = spriteMatch.color;
            }

            matchValue = _piece.matchValue;
        }
    }
}
