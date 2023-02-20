using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class Board : MonoBehaviour
{
    [Header("Board Size")]
    [SerializeField] int width;
    [SerializeField] int height;
    //[SerializeField] int borderSize;

    [SerializeField] GameObject tilePrefab;
    [SerializeField] GameObject tileObstaclePrefab;
    [SerializeField] GameObject[] candyPrefabs;

    [Header("Bomb")]
    [SerializeField] GameObject[] rowBombPrefab;
    [SerializeField] GameObject[] columnBombPrefab;
    [SerializeField] GameObject[] nearBombPrefab;
    [SerializeField] GameObject rollBombPrefab;
    [SerializeField] GameObject colorBombPrefab;
    GameObject clickBomb;
    GameObject targetBomb;

    [Header("Collectible")]
    [SerializeField] int maxCollectible = 3;
    [SerializeField] int countCollectible;
    [Range(0, 1)]
    [SerializeField] float chanceCollectible = 0.1f;
    [SerializeField] GameObject[] collectiblePrefabs;

    Tile[,] allTiles;
    CandyPiece[,] candyPiece;

    Tile clickTile;
    Tile targetTile;
    bool isClearAndRefilling = true;
    bool isSwitching;
    bool isSquare;
    ParticleManager particleManager;
    [SerializeField] float swapTime = 0.3f;
    [Tooltip("캔디가 위에서 떨어지는 높이")]
    [SerializeField] int yOffset = 2;
    [SerializeField] float moveTime = 0.5f;

    [Header("처음 시작 타일 설정")]
    public StartTile[] startTiles;
    [Header("처음 시작 캔디 설정")]
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
        particleManager = FindObjectOfType<ParticleManager>();
    }

    public void SetupBoard()
    {
        SetTiles();
        SetupCandyPiece();
        // Collectible
        List<CandyPiece> startCollectible = FindAllCollectible();
        countCollectible = startCollectible.Count;
        FillBoard(yOffset, moveTime);
    }

    void SetTiles()
    {
        //obstacle 장애물 타일
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

    // 시작할때 원하는 캔디 넣는 기능
    void SetupCandyPiece()
    {
        foreach(StartTile _tile in startCandyPiece)
        {
            if(_tile != null)
            {
                GameObject piece = Instantiate(_tile.tilePrefab, new Vector2(_tile.x, _tile.y), Quaternion.identity);
                MakeCandyPiece(piece, _tile.x, _tile.y, yOffset, moveTime);
            }
        }
    }

    GameObject GetRandomCandy()
    {
        int random = Random.Range(0, candyPrefabs.Length);
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
    // 보드의 경계선안에 있는가?
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
                    CandyPiece piece = FillRandomCandy(i, j, _yOffset, _time);
                    index = 0;
                    // 3Match 맞는게 안 나올때까지 제거하고 생성
                    while (IsMatchWhenFill(i, j))
                    {
                        ClearPiece(i, j);
                        piece = FillRandomCandy(i, j, _yOffset, _time);

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

        if (leftMatch == null)
        {
            leftMatch = new List<CandyPiece>();
        }
        if(downMatch == null)
        {
            downMatch = new List<CandyPiece>();
        }
        return (leftMatch.Count > 0 || downMatch.Count > 0);
    }
    CandyPiece FillRandomCandy(int _x, int _y, int _yOffset = 0, float _time = 0.1f)
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

            // 위에서 캔디가 생성
            if (_yOffset != 0)
            {
                _prefab.transform.position = new Vector2(_x, _y + _yOffset);
                _prefab.GetComponent<CandyPiece>().Move(_x, _y, _time);
            }

            _prefab.transform.parent = transform;
        }
    }
    // 위치-타일 Input //
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
        if (isClearAndRefilling && !isSwitching)
        {
            isSwitching = true;
            // 캔디를 클릭타일 및 타겟타일로 설정
            CandyPiece click = candyPiece[_clickTile.xIndex, _clickTile.yIndex];
            CandyPiece target = candyPiece[_targetTile.xIndex, _targetTile.yIndex];

            if (click != null && target != null)
            {
                // 클릭타일 타겟타일 서로 Swap
                click.Move(_targetTile.xIndex, _targetTile.yIndex, swapTime);
                target.Move(_clickTile.xIndex, _clickTile.yIndex, swapTime);

                yield return new WaitForSeconds(swapTime);
                // Swap 후 매칭되는게 있는가
                List<CandyPiece> clickList = FindMatch(_clickTile.xIndex, _clickTile.yIndex);
                List<CandyPiece> targetList = FindMatch(_targetTile.xIndex, _targetTile.yIndex);

                // Color Bomb
                List<CandyPiece> colorMatch = new List<CandyPiece>();
                // 클릭한게 Color Bomb면 알맞는 섹상 매칭
                if(IsColorBomb(click) && !IsColorBomb(target))
                {
                    click.matchValue = target.matchValue;
                    colorMatch = FindAllMatchValue(click.matchValue);
                }
                else if(!IsColorBomb(click) && IsColorBomb(target))
                {
                    target.matchValue = click.matchValue;
                    colorMatch = FindAllMatchValue(target.matchValue);
                }
                else if (IsColorBomb(click) && IsColorBomb(target))
                {
                    foreach(CandyPiece piece in candyPiece)
                    {
                        if (!colorMatch.Contains(piece))
                        {
                            colorMatch.Add(piece);
                        }
                    }
                }

                // ROLL Bomb
                List<CandyPiece> rollMatch = new List<CandyPiece>();
                if (Is2X2Bomb(click))
                {
                    rollMatch = Bomb2X2Drag(click.xIndex, click.yIndex, target.xIndex, target.yIndex);
                }

                // 캔디 색상 맞는게 없으면 다시 원 상태로 복구 Swap
                if (clickList.Count == 0 && targetList.Count == 0 && colorMatch.Count == 0 && rollMatch.Count == 0)
                {
                    click.Move(_clickTile.xIndex, _clickTile.yIndex, swapTime);
                    target.Move(_targetTile.xIndex, _targetTile.yIndex, swapTime);
                }
                else
                {
                    yield return new WaitForSeconds(swapTime);
                    // 타일을 맞췄을 때만 Bomb 반환
                    Vector2 swap = new Vector2(_targetTile.xIndex - _clickTile.xIndex, _targetTile.yIndex - _clickTile.yIndex);
                    clickBomb = DropBomb(_clickTile.xIndex, _clickTile.yIndex, swap, clickList);
                    targetBomb = DropBomb(_targetTile.xIndex, _targetTile.yIndex, swap, targetList);
                    // Bomb Color 반환
                    if (clickBomb != null && target != null)
                    {
                        CandyPiece bombPiece = clickBomb.GetComponent<CandyPiece>();
                        if (!IsColorBomb(bombPiece) && !Is2X2Bomb(bombPiece))
                            bombPiece.ChangeColor(target);
                    }
                    if (targetBomb != null && click != null)
                    {
                        CandyPiece bombPiece = targetBomb.GetComponent<CandyPiece>();
                        if (!IsColorBomb(bombPiece) && !Is2X2Bomb(bombPiece))
                            bombPiece.ChangeColor(click);
                    }
                    ClearAndRefill(clickList.Union(targetList).ToList().Union(colorMatch).ToList().Union(rollMatch).ToList());
                }
                // 타일 이동하면 Move 감소
                if (GameManager.instance != null)
                {
                    GameManager.instance.moveLeft--;
                    GameManager.instance.Move();
                }
            }
            yield return new WaitForSeconds(swapTime);
            isSwitching = false;
        }

    }
    // 인접한 타일인지 확인
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

    List<CandyPiece> FindMatch(int _x, int _y, Vector2 _search, int _matchLength = 3, int _squareLength = 3)
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

            if (!IsBound(nextX, nextY)) { break; }
            // swap한 후, 인접한 캔디들
            CandyPiece next = candyPiece[nextX, nextY];

            if (next == null) { break; }
            else
            {
                if (next.matchValue == start.matchValue && !match.Contains(next))
                {
                    match.Add(next);
                }
                else { break; }
            }

        }
        if (match.Count >= _matchLength)
        {
            return match;
        }
        return null;
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
        List<CandyPiece> piece = new List<CandyPiece>();
        piece.Add(candyPiece[_x, _y]);
        var combineMatch2 = Find2X2Match(piece);
        var combineMatch = vertical.Union(horizontal).ToList().Union(combineMatch2).ToList();
        // 맞는게 존재하지 않으면 2X2 검색
        if(combineMatch.Count == 0)
        {

        }
        return combineMatch;
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
    
    // 2X2행 찾기
    List<CandyPiece> Find2X2Match(List<CandyPiece> _piece, int _matchLength = 4)
    {
        List<CandyPiece> match = new List<CandyPiece>();
        Vector2 start = new Vector2();

        for (int i = 0; i < _piece.Count; i++)
        {

            CandyPiece startPiece = _piece[i];

            // 사각형 4방면 검색
            for (int j = 0; j < _matchLength; j++)
            {
                match = new List<CandyPiece>();

                if (j == 0)
                    start = new Vector2(startPiece.xIndex, startPiece.yIndex);
                else if (j == 1)
                    start = new Vector2(startPiece.xIndex, startPiece.yIndex - 1);
                else if (j == 2)
                    start = new Vector2(startPiece.xIndex - 1, startPiece.yIndex - 1);
                else if (j == 3)
                    start = new Vector2(startPiece.xIndex - 1, startPiece.yIndex);
                for (int x = 0; x <= 1; x++)
                {
                    for (int y = 0; y <= 1; y++)
                    {
                        if (!IsBound((int)start.x + x, (int)start.y + y))
                        {
                            break;
                        }

                        CandyPiece nextPiece = candyPiece[(int)start.x + x, (int)start.y + y];

                        if (nextPiece == null)
                        {
                            break;
                        }
                        else
                        {
                            if (nextPiece.matchValue == startPiece.matchValue && !match.Contains(nextPiece))
                            {
                                match.Add(nextPiece);
                                if (match.Count == _matchLength)
                                {
                                    isSquare = true;
                                    return match;
                                }
                            }
                            else
                            {
                                isSquare = false;
                                break;
                            }
                        }
                    }
                }
            }
        }
        return new List<CandyPiece>();
    }

    List<CandyPiece> FindVertical(int _x, int _y, int _matchLength = 3)
    {
        // 시작 지점부터 검색해서 위 아래로 2개가 맞는지 검색
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
    }
    List<CandyPiece> FindHorizontal(int _x, int _y, int _matchLength = 3)
    {
        // 시작 지점부터 검색해서 양 옆으로 2개가 맞는지 검색
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
    // 캔디 제거
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
    // CandyPiece 리스트에서 제거
    void ClearPiece(List<CandyPiece> _piece)
    {
        foreach(CandyPiece piece in _piece)
        {
            if(piece != null)
            {
                ClearPiece(piece.xIndex, piece.yIndex);
                // Score
                piece.Score();
                // 파티클 이펙트
                if(particleManager != null)
                {
                    particleManager.ClearPiece(piece.xIndex, piece.yIndex);
                }
            }

        }
    }
    // 전체 제거
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

    List<CandyPiece> BreakColumn(List<int> _column)
    {
        List<CandyPiece> move = new List<CandyPiece>();
        foreach (int column in _column)
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
            if(piece != null)
            {
                if (!column.Contains(piece.xIndex))
                {
                    column.Add(piece.xIndex);
                }
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
        isClearAndRefilling = false;
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

        isClearAndRefilling = true;

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
        HighlightPiece(_piece);
        yield return new WaitForSeconds(0.2f);
        bool isFinish = false;
        // 매칭을 하고 아래로 내려갔을 때 매칭 되는게 또 있는지 확인
        while (!isFinish)
        {
            // Bomb
            List<CandyPiece> bombPiece = GetBombPiece(_piece);
            _piece = _piece.Union(bombPiece).ToList();

            // Bomb가 보드에 여러개 있을 때 모든 Bomb 작동
            bombPiece = GetBombPiece(_piece);
            _piece = _piece.Union(bombPiece).ToList();

            List<int> columnToBreak = GetColumn(_piece);

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
            move = BreakColumn(columnToBreak);
            // 부셔지고 나서 캔디가 아직 움직이는가?
            while (!IsBreak(move))
            {
                yield return null;
            }
            yield return new WaitForSeconds(0.2f);
            // 부셔지고 나서 매치 맞는거 찾기
            match = FindMatch(move);

            // 맞는게 없으면 종료 아니면 재귀
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
    // 부수는 타일 타입 (얼음)
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

    // 위치-폭탄 Bomb Type Candy //
    GameObject MakeBomb(GameObject _prefab, int _x, int _y)
    {
        if (_prefab != null && IsBound(_x, _y))
        {
            GameObject bomb = Instantiate(_prefab, new Vector2(_x, _y), Quaternion.identity);
            bomb.GetComponent<BombCandy>().Init(this);
            bomb.GetComponent<BombCandy>().SetCandy(_x, _y);
            bomb.transform.parent = transform;
            return bomb;
        }
        return null;
    }
    // 행 목록 가져오기
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
    // 열 목록 가져오기
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
    // 인접 목록 가져오기 (Near Bomb Type)
    List<CandyPiece> GetNearPiece(int _x, int _y, int _offset = 1)
    {
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
    // 2X2 행
    bool Is2X2Bomb(CandyPiece _piece)
    {
        BombCandy bomb = _piece.GetComponent<BombCandy>();

        if (bomb != null)
        {
            return (bomb.bombType == BombType.Roll);
        }
        return false;
    }
    // 2X2행 폭탄 드래그한 방향쪽 캔디 가져오기
    List<CandyPiece> Bomb2X2Drag(int _x, int _y, int _targetX, int _targetY)
    {
        List<CandyPiece> piece = new List<CandyPiece>();
        CandyPiece start = candyPiece[_x, _y];
        if(_y > _targetY)
        {
            for (int i = _y; i < height; i++)
            {
                piece.Add(candyPiece[_x, i]);
            }
        }
        else if (_y < _targetY)
        {
            for (int i = _y; i >= 0; i--)
            {
                piece.Add(candyPiece[_x, i]);
            }
        }
        else if (_x < _targetX)
        {
            for (int i = _x; i >= 0; i--)
            {
                piece.Add(candyPiece[i, _y]);
            }
        }
        else if (_x > _targetX)
        {
            for (int i = _x; i < width; i++)
            {
                piece.Add(candyPiece[i, _y]);
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
                // 폭탄에 의해 제거될 캔디 리스트
                List<CandyPiece> clearPiece = new List<CandyPiece>();
                // Bomb를 갖고있는 캔디 확인
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
                    }
                    allPiece = allPiece.Union(clearPiece).ToList();
                }
            }
        }
        return allPiece;
    }
    // 캔디 리스트 목록이 L 모양인가
    bool IsLMatch(List<CandyPiece> _piece)
    {
        // true 면 인접 폭탄
        bool vertical = false;
        bool horizontal = false;
        // -1로 설정한 이유는 0,0 이 시작지점인 경우도 있기 때문
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
        // 둘다 True면 L모양 완성
        return (horizontal && vertical);
    }
    GameObject DropBomb (int _x, int _y, Vector2 _swap, List<CandyPiece> _piece)
    {
        GameObject bomb = null;
        MatchValue match = MatchValue.None;

        if(_piece != null)
        {
            match = FindMatchValue(_piece);
        }
        // 4개의 게임조각 이상이어야 bomb 생성
        if (_piece.Count >= 5 && !isSquare && match != MatchValue.None)
        {
            if (!IsLMatch(_piece))
            {
                bomb = MakeBomb(colorBombPrefab, _x, _y);
            }
            else
            {
                GameObject nearBomb = FindCandyPieceMatchValue(nearBombPrefab, match);
                bomb = MakeBomb(nearBomb, _x, _y);
            }
        }
        else if (_piece.Count == 4 && !isSquare && match != MatchValue.None)
        {
            if (_swap.x != 0)
            {
                GameObject rowBomb = FindCandyPieceMatchValue(rowBombPrefab, match);
                bomb = MakeBomb(rowBomb, _x, _y);
            }
            else
            {
                GameObject columnBomb = FindCandyPieceMatchValue(columnBombPrefab, match);
                bomb = MakeBomb(columnBomb, _x, _y);
            }
        }

        if (_piece.Count >= 4 && isSquare)
        {
            bomb = MakeBomb(rollBombPrefab, _x, _y);
        }
        return bomb;
    }
    // 캔디가 들어갈 자리에 Bomb를 넣어서 그 자리에 못 들어오게 하기
    void NoRefillWhenBomb(GameObject _bomb)
    {
        int x = (int)_bomb.transform.position.x;
        int y = (int)_bomb.transform.position.y;

        if(IsBound(x, y))
        {
            candyPiece[x, y] = _bomb.GetComponent<CandyPiece>();
        }
    }
    // For Color Bomb
    List<CandyPiece> FindAllMatchValue(MatchValue _value)
    {
        List<CandyPiece> piece = new List<CandyPiece>();
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if(candyPiece[i, j] != null)
                {
                    if(candyPiece[i, j].matchValue == _value)
                    {
                        piece.Add(candyPiece[i, j]);
                    }
                }
            }
        }
        return piece;
    }
    bool IsColorBomb(CandyPiece _piece)
    {
        BombCandy bomb = _piece.GetComponent<BombCandy>();

        if(bomb != null)
        {
            return (bomb.bombType == BombType.Color);
        }
        return false;
    }

    //Collectible
    List<CandyPiece> FindCollectible(int _row, bool _clearAtBottom = false)
    {
        List<CandyPiece> collectible = new List<CandyPiece>();
        for (int i = 0; i < width; i++)
        {
            if (candyPiece[i, _row] != null)
            {
                Collectible collectibleComponent = candyPiece[i, _row].GetComponent<Collectible>();
                if (collectibleComponent != null)
                {
                    if (!_clearAtBottom || (_clearAtBottom && collectibleComponent.clearAtBottom))
                    {
                        collectible.Add(candyPiece[i, _row]);
                    }
                }
            }

        }
        return collectible;
    }
    List<CandyPiece> FindAllCollectible()
    {
        List<CandyPiece> collectible = new List<CandyPiece>();
        for (int i = 0; i < height; i++)
        {
            List<CandyPiece> collectibleRow = FindCollectible(i);
            collectible = collectible.Union(collectibleRow).ToList();
        }
        return collectible;
    }
    bool IsAddCollectible()
    {
        return (Random.Range(0f, 1f) <= chanceCollectible && collectiblePrefabs.Length > 0 && countCollectible < maxCollectible);
    }
    GameObject GetRandomObject(GameObject[] _object)
    {
        int random = Random.Range(0, _object.Length);
        if (_object[random] == null)
        {

        }
        return _object[random];
    }
    GameObject GetRandomCollectible()
    {
        return GetRandomObject(collectiblePrefabs);
    }
    CandyPiece FillRandomCollectible(int _x, int _y, int _yOffset = 0, float _time = 0.1f)
    {
        // In FillBoard Method
        if (IsBound(_x, _y))
        {
            GameObject random = Instantiate(GetRandomCollectible(), Vector2.zero, Quaternion.identity);
            MakeCandyPiece(random, _x, _y, _yOffset, _time);
            return random.GetComponent<CandyPiece>();
        }
        return null;
    }
    List<CandyPiece> RemoveCollectible(List<CandyPiece> _piece)
    {
        // In GetBombPiece Method
        List<CandyPiece> collectible = FindAllCollectible();
        List<CandyPiece> remove = new List<CandyPiece>();
        foreach (CandyPiece piece in collectible)
        {
            Collectible component = piece.GetComponent<Collectible>();
            if (component != null)
            {
                if (!component.clearByBomb)
                {
                    remove.Add(piece);
                }
            }
        }
        return _piece.Except(remove).ToList();
    }

    MatchValue FindMatchValue(List<CandyPiece> _piece)
    {
        foreach(CandyPiece piece in _piece)
        {
            if(piece != null)
            {
                return piece.matchValue;
            }
        }
        return MatchValue.None;
    }
    GameObject FindCandyPieceMatchValue(GameObject[] _prefabs, MatchValue _match)
    {
        if(_match == MatchValue.None) { return null; }
        foreach(GameObject go in _prefabs)
        {
            CandyPiece piece = go.GetComponent<CandyPiece>();
            if(piece != null)
            {
                if(piece.matchValue == _match)
                {
                    return go;
                }
            }
        }
        return null;
    }
}
