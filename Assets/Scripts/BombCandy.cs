using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BombType
{
    None,
    Column,
    Row,
    Roll,
    Near,
    Color
}
public class BombCandy : CandyPiece
{
    public BombType bombType;
}
