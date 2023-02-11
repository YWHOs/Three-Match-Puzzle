using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectible : CandyPiece
{
    public bool clearByBomb;
    public bool clearAtBottom;

    void Start()
    {
        matchValue = MatchValue.None;
    }
}
