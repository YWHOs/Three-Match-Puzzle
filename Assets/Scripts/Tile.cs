using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public int xIndex;
    public int yIndex;

    Board board;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Init(int _xIndex, int _yIndex, Board _board)
    {
        xIndex = _xIndex;
        yIndex = _yIndex;
        board = _board;
    }

    void OnMouseDown()
    {
        if(board != null)
        {
            board.ClickTile(this);
        }
    }
    void OnMouseEnter()
    {
        if (board != null)
        {
            board.DragTile(this);
        }
    }
    void OnMouseUp()
    {
        if (board != null)
        {
            board.ReleaseTile();
        }
    }
}
