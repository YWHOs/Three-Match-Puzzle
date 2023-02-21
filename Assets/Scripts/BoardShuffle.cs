using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardShuffle : MonoBehaviour
{
    public List<CandyPiece> RemoveNormalCandy(CandyPiece[,] _pieces)
    {
        List<CandyPiece> normal = new List<CandyPiece>();
        int width = _pieces.GetLength(0);
        int height = _pieces.GetLength(1);

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if(_pieces[i, j] != null)
                {
                    BombCandy bomb = _pieces[i, j].GetComponent<BombCandy>();
                    Collectible collect = _pieces[i, j].GetComponent<Collectible>();

                    if(bomb == null && collect == null)
                    {
                        normal.Add(_pieces[i, j]);
                        _pieces[i, j] = null;
                    }
                }
            }
        }
        return normal;
    }

    public void ShuffleList(List<CandyPiece> _piece)
    {
        int max = _piece.Count;

        for (int i = 0; i < max; i++)
        {
            int r = Random.Range(i, max);
            if(r == i) { continue; }

            CandyPiece temp = _piece[r];
            _piece[r] = _piece[i];
            _piece[i] = temp;
        }
    }

    public void MovePiece(CandyPiece[,] _pieces, float _time = 0.5f)
    {
        int width = _pieces.GetLength(0);
        int height = _pieces.GetLength(1);

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if(_pieces[i, j] != null)
                {
                    _pieces[i, j].Move(i, j, _time);
                }
            }
        }
    }
}
