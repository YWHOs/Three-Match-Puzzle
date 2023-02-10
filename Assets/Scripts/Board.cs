using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class Board : MonoBehaviour
{
    [SerializeField] int width;
    [SerializeField] int height;
    [SerializeField] int borderSize;

    [SerializeField] GameObject tilePrefab;
    [SerializeField] GameObject[] candyPrefabs;

    [SerializeField] float swapTime = 0.3f;

    Tile[,] allTiles;
    CandyPiece[,] candyPiece;

    Tile clickTile;
    Tile targetTile;
    // Start is called before the first frame update
    void Start()
    {
        // 타일 크기 조정
        allTiles = new Tile[width, height];
        candyPiece = new CandyPiece[width, height];
        SetTiles();
        //SetCamera();
        FillRandom();
        //HighlightMatch();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SetTiles()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                // Create Tiles
                GameObject tile = Instantiate(tilePrefab, new Vector2(i, j), Quaternion.identity);
                tile.name = $"Tile({i},{j})";
                allTiles[i, j] = tile.GetComponent<Tile>();
                tile.transform.parent = transform;

                // 타일 x,y값 가져오기
                allTiles[i, j].Init(i, j, this);
            }
        }
    }

    void SetCamera()
    {
        Camera.main.transform.position = new Vector3((width-1)/ 2f, (height-1)/ 2f, -10f);

        float ratio = Screen.width / Screen.height;
        float vertical = height / 2f + borderSize;
        float horizontal = (width / 2f + borderSize) / ratio;

        Camera.main.orthographicSize = (vertical > horizontal) ? vertical : horizontal;
    }

    GameObject GetRandomCandy()
    {
        int random = Random.Range(0, candyPrefabs.Length);

        if(candyPrefabs[random] == null)
        {

        }
        return candyPrefabs[random];
    }

    public void PlaceCandy(CandyPiece _candyPiece, int _x, int _y)
    {
        if(_candyPiece == null) { return; }

        _candyPiece.transform.position = new Vector2(_x, _y);
        _candyPiece.transform.rotation = Quaternion.identity;
        if(IsBound(_x, _y))
            candyPiece[_x, _y] = _candyPiece;
        _candyPiece.SetCandy(_x, _y);
    }
    bool IsBound(int _x, int _y)
    {
        return (_x >= 0 && _x < width && _y >= 0 && _y < height);
    }

    void FillRandom()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                GameObject candy = Instantiate(GetRandomCandy(), Vector2.zero, Quaternion.identity);

                if(candy != null)
                {
                    candy.GetComponent<CandyPiece>().Init(this);
                    PlaceCandy(candy.GetComponent<CandyPiece>(), i, j);
                    candy.transform.parent = transform;
                }
            }
        }
    }

    public void ClickTile(Tile _tile)
    {
        if(clickTile == null)
        {
            clickTile = _tile;
        }
    }
    public void DragTile(Tile _tile)
    {
        if(clickTile != null && IsCloseTile(_tile, clickTile))
        {
            targetTile = _tile;
        }
    }
    public void ReleaseTile()
    {
        if(clickTile != null && targetTile != null)
        {
            SwitchTile(clickTile, targetTile);
        }

        clickTile = null;
        targetTile = null;
    }
    void SwitchTile(Tile _clickTile, Tile _targetTile)
    {
        StartCoroutine(SwitchTilesCoroutine(_clickTile, _targetTile));

    }
    IEnumerator SwitchTilesCoroutine(Tile _clickTile, Tile _targetTile)
    {
        CandyPiece click = candyPiece[_clickTile.xIndex, _clickTile.yIndex];
        CandyPiece target = candyPiece[_targetTile.xIndex, _targetTile.yIndex];

        click.Move(_targetTile.xIndex, _targetTile.yIndex, swapTime);
        target.Move(_clickTile.xIndex, _clickTile.yIndex, swapTime);

        yield return new WaitForSeconds(swapTime);
        List<CandyPiece> clickList = FindMatch(_clickTile.xIndex, _clickTile.yIndex);
        List<CandyPiece> targetList = FindMatch(_targetTile.xIndex, _targetTile.yIndex);

        // 색깔 맞는게 없으면 다시 원 상태로 복구
        if(clickList.Count == 0 && targetList.Count == 0)
        {
            click.Move(_clickTile.xIndex, _clickTile.yIndex, swapTime);
            target.Move(_targetTile.xIndex, _targetTile.yIndex, swapTime);
        }
        else
        {
            yield return new WaitForSeconds(swapTime);
            ClearPiece(clickList);
            ClearPiece(targetList);
            //HighlightMatchAt(_clickTile.xIndex, _clickTile.yIndex);
            //HighlightMatchAt(_targetTile.xIndex, _targetTile.yIndex);
        }

    }
    bool IsCloseTile(Tile _start, Tile _end)
    {
        if (Mathf.Abs(_start.xIndex - _end.xIndex) == 1 && _start.yIndex == _end.yIndex)
        {
            return true;
        }
        if (Mathf.Abs(_start.yIndex - _end.yIndex) == 1 && _start.xIndex == _end.xIndex)
        {
            return true;
        }
        return false;
    }

    List<CandyPiece> FindMatch(int _x, int _y, Vector2 _search, int _matchLength = 3)
    {
        List<CandyPiece> match = new List<CandyPiece>();
        CandyPiece start = null;

        if(IsBound(_x, _y))
        {
            start = candyPiece[_x, _y];
        }
        if(start != null)
        {
            match.Add(start);
        }
        else { return null; }

        int nextX;
        int nextY;
        int maxValue = (width > height) ? width : height;
        for (int i = 1; i < maxValue - 1; i++)
        {
            nextX = _x + (int)Mathf.Clamp(_search.x, -1, 1) * i;
            nextY = _y + (int)Mathf.Clamp(_search.y, -1, 1) * i;

            if(!IsBound(nextX, nextY))
            {
                break;
            }
            CandyPiece next = candyPiece[nextX, nextY];

            if(next == null) { break; }
            else
            {
                if (next.matchValue == start.matchValue && !match.Contains(next))
                {
                    match.Add(next);
                }
                else { break; }
            }

        }
        if(match.Count >= _matchLength)
        {
            return match;
        }
        return null;
    }
    List<CandyPiece> FindVertical(int _x, int _y, int _matchLength = 3)
    {
        // 시작 지점부터 검색해서 2개가 맞는지 검색
        List<CandyPiece> upMatch = FindMatch(_x, _y, new Vector2(0, 1), 2);
        List<CandyPiece> downMatch = FindMatch(_x, _y, new Vector2(0, -1), 2);

        if (upMatch == null)
        {
            upMatch = new List<CandyPiece>();
        }
        if (downMatch == null)
        {
            downMatch = new List<CandyPiece>();
        }
        // 합집합은 요소를 2배로 늘리는 것을 방지
        var combineMatch = upMatch.Union(downMatch).ToList();
        return (combineMatch.Count >= _matchLength) ? combineMatch : null;
        //foreach (CandyPiece piece in downMatch)
        //{
        //    if (!upMatch.Contains(piece))
        //    {
        //        upMatch.Add(piece);
        //    }
        //}
        //return (upMatch.Count >= _matchLength) ? upMatch : null;
    }
    List<CandyPiece> FindHorizontal(int _x, int _y, int _matchLength = 3)
    {
        List<CandyPiece> rightMatch = FindMatch(_x, _y, new Vector2(1, 0), 2);
        List<CandyPiece> leftMatch = FindMatch(_x, _y, new Vector2(-1, 0), 2);

        if (rightMatch == null)
        {
            rightMatch = new List<CandyPiece>();
        }
        if (leftMatch == null)
        {
            leftMatch = new List<CandyPiece>();
        }
        // 합집합은 요소를 2배로 늘리는 것을 방지
        var combineMatch = rightMatch.Union(leftMatch).ToList();
        return (combineMatch.Count >= _matchLength) ? combineMatch : null;
    }
    void HighlightMatch()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                HighlightMatchAt(i, j);
            }
        }
    }

    private void HighlightMatchAt(int _x, int _y)
    {
        HighlightTileOff(_x, _y);
        var combineMatch = FindMatch(_x, _y);
        if (combineMatch.Count > 0)
        {
            foreach (CandyPiece piece in combineMatch)
            {
                HighlightTileOn(piece.xIndex, piece.yIndex, piece.GetComponent<SpriteRenderer>().color);
            }
        }
    }

    void HighlightTileOff(int _x, int _y)
    {
        SpriteRenderer sprite = allTiles[_x, _y].GetComponent<SpriteRenderer>();
        sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, 0);
    }
    void HighlightTileOn(int _x, int _y, Color _color)
    {
        SpriteRenderer sprite = allTiles[_x, _y].GetComponent<SpriteRenderer>();
        sprite.color = _color;
    }
    List<CandyPiece> FindMatch(int _x, int _y, int _matchLength = 3)
    {
        List<CandyPiece> vertical = FindVertical(_x, _y, _matchLength);
        List<CandyPiece> horizontal = FindHorizontal(_x, _y, _matchLength);

        if (vertical == null)
        {
            vertical = new List<CandyPiece>();
        }
        if (horizontal == null)
        {
            horizontal = new List<CandyPiece>();
        }

        var combineMatch = vertical.Union(horizontal).ToList();
        return combineMatch;
    }
    void ClearPiece(int _x, int _y)
    {
        CandyPiece piece = candyPiece[_x, _y];
        if(piece != null)
        {
            candyPiece[_x, _y] = null;
            Destroy(piece.gameObject);
        }
        HighlightTileOff(_x, _y);
    }
    void ClearPiece(List<CandyPiece> _piece)
    {
        foreach(CandyPiece piece in _piece)
        {
            ClearPiece(piece.xIndex, piece.yIndex);
        }
    }
    void ClearBoard()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                ClearPiece(i, j);
            }
        }
    }
    void ElementDelete()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {

            }
        }
    }
}
