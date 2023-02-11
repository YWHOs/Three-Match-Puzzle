using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TileType
{
    Normal,
    Obstacle,
    Breakable
}
public class Tile : MonoBehaviour
{
    public int xIndex;
    public int yIndex;

    Board board;

    public TileType tileType = TileType.Normal;

    //Break
    SpriteRenderer spriteRenderer;
    public int breakValue;
    public Sprite[] breakSprite;
    [SerializeField] Color normalColor;
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Init(int _xIndex, int _yIndex, Board _board)
    {
        xIndex = _xIndex;
        yIndex = _yIndex;
        board = _board;

        //Break
        if(tileType == TileType.Breakable)
        {
            if (breakSprite[breakValue] != null)
            {
                spriteRenderer.sprite = breakSprite[breakValue];
            }
        }

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

    public void BreakTile()
    {
        if(tileType != TileType.Breakable) { return; }
        StartCoroutine(BreakTileCoroutine());
    }
    IEnumerator BreakTileCoroutine()
    {
        breakValue = Mathf.Clamp(--breakValue, 0, breakValue);
        yield return new WaitForSeconds(0.2f);

        if(breakSprite[breakValue] != null)
        {
            spriteRenderer.sprite = breakSprite[breakValue];
        }
        if(breakValue == 0)
        {
            tileType = TileType.Normal;
            spriteRenderer.color = normalColor;
        }
    }
}
