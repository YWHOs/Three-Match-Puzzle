using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CollectionGoalPanel : MonoBehaviour
{
    public CollectGoal collectGoal;
    [SerializeField] Text leftText;
    [SerializeField] Image image;

    // Start is called before the first frame update
    void Start()
    {
        Setup();
    }

    public void Setup()
    {
        if(collectGoal != null && leftText != null)
        {
            SpriteRenderer renderer = collectGoal.prefab.GetComponent<SpriteRenderer>();

            image.sprite = renderer.sprite;
            image.color = renderer.color;

            leftText.text = collectGoal.numberCollect.ToString();
        }
    }

    public void UpdatePanel()
    {
        leftText.text = collectGoal.numberCollect.ToString();
    }
}
