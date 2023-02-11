using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class Board : MonoBehaviour
{
    [Header("Board Size")]
    [SerializeField] int width;
    [SerializeField] int height;
    [SerializeField] int borderSize;

    [SerializeField] GameObject tilePrefab;
    [SerializeField] GameObject tileObstaclePrefab;
    [SerializeField] GameObject[] candyPrefabs;

    [Header("Bomb")]
    [SerializeField] GameObject rowBombPrefab;
    [SerializeField] GameObject columnBombPrefab;
    [SerializeField] GameObject nearBombPrefab;
    GameObject clickBomb;
    GameObject targetBomb;

    [SerializeField] float swapTime = 0.3f;

    Tile[,] allTiles;
    CandyPiece[,] candyPiece;

    Tile clickTile;
    Tile targetTile;
    bool isInput = true;

    ParticleManager particleManager;
    [SerializeField] int yOffset = 10;
    [SerializeField] float moveTime = 0.5f;

    public StartTile[] startTiles;
    [SerializeField] StartTile[] startCandyPiece;
    [System.Serializable]
    public class StartTile
    {
        public GameObject tilePrefab;
        public int x;
        public int y;
    }
    // Start is called before the first frame update
    void Start()
    {
        // 타일 크기 조정
        allTiles = new Tile[width, height];
        candyPiece = new CandyPiece[width, height];
        SetTiles();
        SetupCandyPiece();
        //SetCamera();
        FillBoard(yOffset, moveTime);
        particleManager = FindObjectOfType<ParticleManager>();
    }

    void SetTiles()
    {
        //obstacle
        foreach (StartTile tile in startTiles)
        {
            if (tile != null)
            {
                CreateTile(tile.tilePrefab, tile.x, tile.y);
            }
        }

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if(allTiles[i, j] == null)
                    CreateTile(tilePrefab, i, j);
            }
        }
    }

    private void CreateTile(GameObject _prefab, int _x, int _y)
    {
        // Create Tiles
        if(_prefab != null && IsBound(_x, _y))
        {
            GameObject tile = Instantiate(_prefab, new Vector2(_x, _y), Quaternion.identity);
            tile.name = $"Tile({_x},{_y})";
            allTiles[_x, _y] = tile.GetComponent<Tile>();
            tile.transform.parent = transform;

            // 타일 x,y값 가져오기
            allTiles[_x, _y].Init(_x, _y, this);
        }

    }
    void SetupCandyPiece()
    {
        // 시작할때 원하는 캔디 넣는 기능
        foreach(StartTile _tile in startCandyPiece)
        {
            if(_tile != null)
            {
                GameObject piece = Instantiate(_tile.tilePrefab, new Vector2(_tile.x, _tile.y), Quaternion.identity);
                MakeCandyPiece(piece, _tile.x, _tile.y, yOffset, moveTime);
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

    void FillBoard(int _yOffset = 0, float _time = 0.1f)
    {
        int max = 100;
        int index = 0;
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if(candyPiece[i, j] == null && allTiles[i, j].tileType != TileType.Obstacle)
                {
                    CandyPiece piece = FillRandom(i, j, _yOffset, _time);
                    index = 0;
                    // 3Match 맞는게 안 나올때까지 제거하고 생성
                    while (IsMatchWhenFill(i, j))
                    {
                        ClearPiece(i, j);
                        piece = FillRandom(i, j, _yOffset, _time);

                        // while loop break
                        index++;
                        if (index >= max)
                        {
                            break;
                        }
                    }
                }

            }
        }
    }
    bool IsMatchWhenFill(int _x, int _y, int _matchLength = 3)
    {
        List<CandyPiece> leftMatch = FindMatch(_x, _y, new Vector2(-1, 0), _matchLength);
        List<CandyPiece> downMatch = FindMatch(_x, _y, new Vector2(0, -1), _matchLength);

        if(leftMatch == null)
        {
            leftMatch = new List<CandyPiece>();
        }
        if(downMatch == null)
        {
            downMatch = new List<CandyPiece>();
        }
        return (leftMatch.Count > 0 || downMatch.Count > 0);
    }
    CandyPiece FillRandom(int _x, int _y, int _yOffset = 0, float _time = 0.1f)
    {
        if(IsBound(_x, _y))
        {
            GameObject candy = Instantiate(GetRandomCandy(), Vector2.zero, Quaternion.identity);
            MakeCandyPiece(candy, _x, _y, _yOffset, _time);
            return candy.GetComponent<CandyPiece>();
        }
        return null;
    }
    void MakeCandyPiece(GameObject _prefab, int _x, int _y, int _yOffset = 0, float _time = 0.1f)
    {
        if (_prefab != null && IsBound(_x,_y))
        {
            _prefab.GetComponent<CandyPiece>().Init(this);
            PlaceCandy(_prefab.GetComponent<CandyPiece>(), _x, _y);

            // 위에서 캔디 생성
            if (_yOffset != 0)
            {
                _prefab.transform.position = new Vector2(_x, _y + _yOffset);
                _prefab.GetComponent<CandyPiece>().Move(_x, _y, _time);
            }

            _prefab.transform.parent = transform;
        }
    }
    GameObject MakeBomb(GameObject _prefab, int _x, int _y)
    {
        if(_prefab != null && IsBound(_x, _y))
        {
            GameObject bomb = Instantiate(_prefab, new Vector2(_x, _y), Quaternion.identity);
            bomb.GetComponent<BombCandy>().Init(this);
            bomb.GetComponent<BombCandy>().SetCandy(_x, _y);
            bomb.transform.parent = transform;
            return bomb;
        }
        return null;
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
        if (isInput)
        {
            CandyPiece click = candyPiece[_clickTile.xIndex, _clickTile.yIndex];
            CandyPiece target = candyPiece[_targetTile.xIndex, _targetTile.yIndex];

            if (click != null && target != null)
            {
                click.Move(_targetTile.xIndex, _targetTile.yIndex, swapTime);
                target.Move(_clickTile.xIndex, _clickTile.yIndex, swapTime);

                yield return new WaitForSeconds(swapTime);
                List<CandyPiece> clickList = FindMatch(_clickTile.xIndex, _clickTile.yIndex);
                List<CandyPiece> targetList = FindMatch(_targetTile.xIndex, _targetTile.yIndex);

                // 색깔 맞는게 없으면 다시 원 상태로 복구
                if (clickList.Count == 0 && targetList.Count == 0)
                {
                    click.Move(_clickTile.xIndex, _clickTile.yIndex, swapTime);
                    target.Move(_targetTile.xIndex, _targetTile.yIndex, swapTime);
                }
                else
                {
                    yield return new WaitForSeconds(swapTime);
                    //Bomb
                    Vector2 swap = new Vector2(_targetTile.xIndex - _clickTile.xIndex, _targetTile.yIndex - _clickTile.yIndex);
                    clickBomb = DropBomb(_clickTile.xIndex, _clickTile.yIndex, swap, clickList);
                    targetBomb = DropBomb(_targetTile.xIndex, _targetTile.yIndex, swap, targetList);

                    ClearAndRefill(clickList.Union(targetList).ToList());
                    //ClearPiece(clickList);
                    //ClearPiece(targetList);

                    //BreakColumn(clickList);
                    //BreakColumn(targetList);
                }
            }
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
    List<CandyPiece> FindMatch(List<CandyPiece> _piece, int _matchLength = 3)
    {
        List<CandyPiece> match = new List<CandyPiece>();
        foreach(CandyPiece piece in _piece)
        {
            match = match.Union(FindMatch(piece.xIndex, piece.yIndex, _matchLength)).ToList();
        }
        return match;
    }
    List<CandyPiece> FindAllMatch()
    {
        List<CandyPiece> combineMatch = new List<CandyPiece>();
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                List<CandyPiece> match = FindMatch(i, j);
                combineMatch = combineMatch.Union(match).ToList();
            }
        }
        return combineMatch;
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
    void HighlightPiece(List<CandyPiece> _piece)
    {
        foreach(CandyPiece piece in _piece)
        {
            if(piece != null)
            {
                HighlightTileOn(piece.xIndex, piece.yIndex, piece.GetComponent<SpriteRenderer>().color);
            }
        }
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
        if(allTiles[_x, _y].tileType != TileType.Breakable)
        {
            SpriteRenderer sprite = allTiles[_x, _y].GetComponent<SpriteRenderer>();
            sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, 0);
        }

    }
    void HighlightTileOn(int _x, int _y, Color _color)
    {
        if (allTiles[_x, _y].tileType != TileType.Breakable)
        {
            SpriteRenderer sprite = allTiles[_x, _y].GetComponent<SpriteRenderer>();
            sprite.color = _color;
        }
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
        //HighlightTileOff(_x, _y);
    }
    void ClearPiece(List<CandyPiece> _piece)
    {
        foreach(CandyPiece piece in _piece)
        {
            if(piece != null)
            {
                ClearPiece(piece.xIndex, piece.yIndex);
                // 파티클 이펙트
                if(particleManager != null)
                {
                    particleManager.ClearPiece(piece.xIndex, piece.yIndex);
                }
            }

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
    List<CandyPiece> BreakColumn(int _column, float _time = 0.1f)
    {
        List<CandyPiece> move = new List<CandyPiece>();
        for (int i = 0; i < height - 1; i++)
        {
            // 위에 빈 공간이 존재하는가
            if(candyPiece[_column,i] == null && allTiles[_column, i].tileType != TileType.Obstacle)
            {
                // 정상까지 확인
                for (int j = i + 1; j < height; j++)
                {
                    if(candyPiece[_column, j] != null)
                    {
                        // 빈 공간으로 이동
                        candyPiece[_column, j].Move(_column, i, _time * (j - 1));
                        // 바뀐 Candy 인덱스를 다시 참조
                        candyPiece[_column, i] = candyPiece[_column, j];
                        candyPiece[_column, i].SetCandy(_column, i);

                        if (!move.Contains(candyPiece[_column, i]))
                        {
                            move.Add(candyPiece[_column, i]);
                        }
                        candyPiece[_column, j] = null;
                        break;
                    }
                }
            }
        }
        return move;
    }
    List<CandyPiece> BreakColumn(List<CandyPiece> piece)
    {
        List<CandyPiece> move = new List<CandyPiece>();
        List<int> breakColumn = GetColumn(piece);
        foreach (int column in breakColumn)
        {
            move = move.Union(BreakColumn(column)).ToList();
        }
        return move;
    }
    // 영향을 받은 열만 실행 시키기
    List<int> GetColumn(List<CandyPiece> _piece)
    {
        List<int> column = new List<int>();
        foreach(CandyPiece piece in _piece)
        {
            if (!column.Contains(piece.xIndex))
            {
                column.Add(piece.xIndex);
            }
        }
        return column;
    }
    void ClearAndRefill(List<CandyPiece> _piece)
    {
        StartCoroutine(ClearAndRefillCoroutine(_piece));
    }
    IEnumerator ClearAndRefillCoroutine(List<CandyPiece> _piece)
    {
        isInput = false;
        List<CandyPiece> match = _piece;
         
        // 일치하는 항목을 찾지 못할 때까지 반복
        do
        {
            yield return StartCoroutine(ClearAndBreakCoroutine(match));
            yield return null;

            //리필
            yield return StartCoroutine(RefillCoroutine());
            match = FindAllMatch();

            yield return new WaitForSeconds(0.2f);
        }
        while (match.Count != 0);

        isInput = true;

    }
    IEnumerator RefillCoroutine()
    {
        FillBoard(yOffset, moveTime);
        yield return null;
    }
    IEnumerator ClearAndBreakCoroutine(List<CandyPiece> _piece)
    {
        List<CandyPiece> move = new List<CandyPiece>();
        List<CandyPiece> match = new List<CandyPiece>();
        //HighlightPiece(_piece);
        yield return new WaitForSeconds(0.2f);
        bool isFinish = false;
        // 매칭을 하고 아래로 내려갔을 때 매칭 되는게 또 있는지 확인
        while (!isFinish)
        {
            // Bomb
            List<CandyPiece> bombPiece = GetBombPiece(_piece);
            _piece = _piece.Union(bombPiece).ToList();

            ClearPiece(_piece);
            BreakTile(_piece);
            // 보드에 Bomb 추가
            if(clickBomb != null)
            {
                NoRefillWhenBomb(clickBomb);
                clickBomb = null;
            }
            if(targetBomb != null)
            {
                NoRefillWhenBomb(targetBomb);
                targetBomb = null;
            }
            yield return new WaitForSeconds(0.2f);
            move = BreakColumn(_piece);
            // 캔디가 아직 움직이는가?
            while (!IsBreak(move))
            {
                yield return null;
            }
            yield return new WaitForSeconds(0.2f);
            match = FindMatch(move);
            if(match.Count == 0)
            {
                isFinish = true;
                break;
            }
            else
            {
                yield return StartCoroutine(ClearAndBreakCoroutine(match));
            }
        }
        yield return null;
    }
    bool IsBreak(List<CandyPiece> _piece)
    {
        foreach(CandyPiece piece in _piece)
        {
            if(piece != null)
            {
                if(piece.transform.position.y - piece.yIndex > 0.001f)
                {
                    // 캔디가 아직도 움직이고 있으면 false
                    return false;
                }
            }
        }
        return true;
    }
    void BreakTile(int _x, int _y)
    {
        Tile tile = allTiles[_x, _y];
        if(tile != null && tile.tileType == TileType.Breakable)
        {
            // 파티클 이펙트
            if (particleManager != null)
            {
                particleManager.BreakTile(tile.breakValue, _x, _y);
            }
            tile.BreakTile();
        }
    }
    void BreakTile(List<CandyPiece> _piece)
    {
        foreach(CandyPiece piece in _piece)
        {
            if(piece != null)
            {
                BreakTile(piece.xIndex, piece.yIndex);
            }
        }
    }

    // Bomb Type Candy //
    List<CandyPiece> GetRowPiece(int _row)
    {
        List<CandyPiece> piece = new List<CandyPiece>();
        for (int i = 0; i < width; i++)
        {
            if(candyPiece[i, _row] != null)
            {
                piece.Add(candyPiece[i, _row]);
            }
        }
        return piece;
    }
    List<CandyPiece> GetColumnPiece(int _column)
    {
        List<CandyPiece> piece = new List<CandyPiece>();
        for (int i = 0; i < height; i++)
        {
            if (candyPiece[_column, i] != null)
            {
                piece.Add(candyPiece[_column, i]);
            }
        }
        return piece;
    }
    List<CandyPiece> GetNearPiece(int _x, int _y, int _offset = 1)
    {
        // 인접 캔디 가져오기 (Bomb Type)
        List<CandyPiece> piece = new List<CandyPiece>();
        for (int i = _x - _offset; i <= _x + _offset; i++)
        {
            for (int j = _y - _offset; j <= _y + _offset; j++)
            {
                if(IsBound(i, j))
                {
                    piece.Add(candyPiece[i, j]);
                }
            }
        }
        return piece;
    }
    List<CandyPiece> GetBombPiece(List<CandyPiece> _piece)
    {
        List<CandyPiece> allPiece = new List<CandyPiece>();
        foreach(CandyPiece piece in _piece)
        {
            if(piece != null)
            {
                List<CandyPiece> clearPiece = new List<CandyPiece>();
                BombCandy bombCandy = piece.GetComponent<BombCandy>();
                if(bombCandy != null)
                {
                    switch (bombCandy.bombType)
                    {
                        case BombType.Column:
                            clearPiece = GetColumnPiece(bombCandy.xIndex);
                            break;
                        case BombType.Row:
                            clearPiece = GetRowPiece(bombCandy.yIndex);
                            break;
                        case BombType.Near:
                            clearPiece = GetNearPiece(bombCandy.xIndex, bombCandy.yIndex);
                            break;
                        case BombType.Color:
                            break;
                    }
                    allPiece = allPiece.Union(clearPiece).ToList();
                }
            }
        }
        return allPiece;
    }
    bool IsCorner(List<CandyPiece> _piece)
    {
        // true 면 인접 폭탄
        bool vertical = false;
        bool horizontal = false;
        int x = -1;
        int y = -1;
        foreach(CandyPiece piece in _piece)
        {
            if(piece != null)
            {
                if(x == -1 || y == -1)
                {
                    x = piece.xIndex;
                    y = piece.yIndex;
                    continue;
                }
            }
            if(piece.xIndex != x && piece.yIndex == y)
            {
                horizontal = true;
            }
            if(piece.xIndex == x && piece.yIndex != y)
            {
                vertical = true;
            }
        }
        return (horizontal && vertical);
    }
    GameObject DropBomb (int _x, int _y, Vector2 _swap, List<CandyPiece> _piece)
    {
        GameObject bomb = null;
        // 4개의 게임조각 이상이어야 bomb 생성
        if(_piece.Count >= 4)
        {
            if (IsCorner(_piece))
            {
                if(nearBombPrefab != null)
                {
                    bomb = MakeBomb(nearBombPrefab, _x, _y);
                }
            }
            else
            {
                if(_swap.x != 0)
                {
                    bomb = MakeBomb(rowBombPrefab, _x, _y);
                }
                else
                {
                    bomb = MakeBomb(columnBombPrefab, _x, _y);
                }
            }
        }
        return bomb;
    }
    void NoRefillWhenBomb(GameObject _bomb)
    {
        int x = (int)_bomb.transform.position.x;
        int y = (int)_bomb.transform.position.y;

        if(IsBound(x, y))
        {
            candyPiece[x, y] = _bomb.GetComponent<CandyPiece>();
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
