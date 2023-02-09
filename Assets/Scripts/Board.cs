using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    [SerializeField] int width;
    [SerializeField] int height;
    [SerializeField] int borderSize;

    [SerializeField] GameObject tilePrefab;

    Tile[,] allTiles;
    // Start is called before the first frame update
    void Start()
    {
        allTiles = new Tile[width, height];
        SetTiles();
        //SetCamera();
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
                GameObject tile = Instantiate(tilePrefab, new Vector3(i, j, 0), Quaternion.identity);
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
}
