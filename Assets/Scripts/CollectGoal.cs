using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectGoal : MonoBehaviour
{
    public CandyPiece prefab;

    [Range(1, 50)]
    public int numberCollect = 5;

    SpriteRenderer spriteRenderer;
    // Start is called before the first frame update
    void Start()
    {
        if(prefab != null)
        {
            spriteRenderer = prefab.GetComponent<SpriteRenderer>();
        }
    }

    public void CollectPiece(CandyPiece _piece)
    {
        if(_piece != null)
        {
            SpriteRenderer renderer = _piece.GetComponent<SpriteRenderer>();

            if(spriteRenderer.sprite == renderer.sprite && prefab.matchValue == _piece.matchValue)
            {
                numberCollect--;
                numberCollect = Mathf.Clamp(numberCollect, 0, numberCollect);
            }
        }

    }
}
