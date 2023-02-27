using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    [SerializeField] GameObject layout;
    [SerializeField] int baseWidth = 125;
    CollectionGoalPanel[] collectionGoalPanels;

    public Text moveText;
    public FadeManager fadeManager;
    public MessageUI messageUI;
    public ScoreSlide scoreSlide;

    [SerializeField] GameObject move;
    public Timer timer;

    public override void Awake()
    {
        base.Awake();

        if(messageUI != null)
        {
            messageUI.gameObject.SetActive(true);
        }
        if(fadeManager != null)
        {
            fadeManager.gameObject.SetActive(true);
        }
    }

    public void Setup(CollectGoal[] _goal)
    {
        if(layout != null && _goal != null && _goal.Length != 0)
        {
            RectTransform rect = layout.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(_goal.Length * baseWidth, rect.sizeDelta.y);
            collectionGoalPanels = layout.gameObject.GetComponentsInChildren<CollectionGoalPanel>();
        }
        for (int i = 0; i < collectionGoalPanels.Length; i++)
        {
            if(i< _goal.Length && _goal[i] != null)
            {
                collectionGoalPanels[i].gameObject.SetActive(true);
                collectionGoalPanels[i].collectGoal = _goal[i];
                collectionGoalPanels[i].Setup();
            }
            else
            {
                collectionGoalPanels[i].gameObject.SetActive(false);
            }
        }
    }
    public void UpdateCollection()
    {
        foreach(CollectionGoalPanel panel in collectionGoalPanels)
        {
            if(panel != null && panel.gameObject.activeInHierarchy)
            {
                panel.UpdatePanel();
            }
        }
    }

    public void EnableTimer(bool _state)
    {
        if(timer != null)
        {
            timer.gameObject.SetActive(_state);
        }
    }
    public void EnableMove(bool _state)
    {
        if(move != null)
        {
            move.SetActive(_state);
        }
    }
    public void EnableCollection(bool _state)
    {
        if(layout != null)
        {
            layout.SetActive(_state);
        }
    }
}
