using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BoardDeadlock : MonoBehaviour
{
    List<CandyPiece> GetRowColumn(CandyPiece[,] _pieces, int _x, int _y, int _list = 3, bool _isrow = true)
    {
        int width = _pieces.GetLength(0);
        int height = _pieces.GetLength(1);

        List<CandyPiece> piece = new List<CandyPiece>();

        for (int i = 0; i < _list; i++)
        {
            if (_isrow)
            {
                if(_x + i < width && _y < height)
                {
                    piece.Add(_pieces[_x + i, _y]);
                }
            }
            else
            {
                if(_x < width && _y + i < height)
                {
                    piece.Add(_pieces[_x, _y + i]);
                }
            }
        }
        return piece;
    }

    List<CandyPiece> GetMinimum(List<CandyPiece> _piece, int _min = 2)
    {
        List<CandyPiece> match = new List<CandyPiece>();

        var group = _piece.GroupBy(n => n.matchValue);

        foreach(var gr in group)
        {
            if(gr.Count() >= _min && gr.Key != MatchValue.None)
            {
                match = gr.ToList();
            }
        }
        return match;
    }

    List<CandyPiece> GetNeighbor(CandyPiece[,] _pieces, int _x, int _y)
    {
        int width = _pieces.GetLength(0);
        int height = _pieces.GetLength(1);

        List<CandyPiece> neighbor = new List<CandyPiece>();

        Vector2[] search = new Vector2[4]
        {
            new Vector2(-1f, 0f),
            new Vector2(1f, 0f),
            new Vector2(0f, 1f),
            new Vector2(0f, -1f),
        };

        foreach(Vector2 dir in search)
        {
            if(_x + dir.x >= 0 && _x + dir.x < width && _y + dir.y >= 0 && _y + dir.y < height)
            {
                if(_pieces[_x + (int)dir.x, _y + (int)dir.y] != null)
                {
                    if(!neighbor.Contains(_pieces[_x + (int)dir.x, _y + (int)dir.y]))
                    {
                        neighbor.Add(_pieces[_x + (int)dir.x, _y + (int)dir.y]);
                    }
                }
            }
        }
        return neighbor;
    }
}
