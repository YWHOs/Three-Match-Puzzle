using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    [SerializeField] GameObject layout;
    [SerializeField] int baseWidth = 125;

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

    public void Setup(CollectGoal[] _goal, GameObject _layout)
    {
        if(_layout != null && _goal != null && _goal.Length != 0)
        {
            RectTransform rect = _layout.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(_goal.Length * baseWidth, rect.sizeDelta.y);
            CollectionGoalPanel[] panels = _layout.GetComponentsInChildren<CollectionGoalPanel>();

            for (int i = 0; i < panels.Length; i++)
            {
                if (i < _goal.Length && _goal[i] != null)
                {
                    panels[i].gameObject.SetActive(true);
                    panels[i].collectGoal = _goal[i];
                    panels[i].Setup();
                }
                else
                {
                    panels[i].gameObject.SetActive(false);
                }
            }
        }
    }
    public void Setup(CollectGoal[] _goal)
    {
        Setup(_goal, layout);
    }
    void UpdateCollection(GameObject _layout)
    {
        if(_layout != null)
        {
            CollectionGoalPanel[] panels = _layout.GetComponentsInChildren<CollectionGoalPanel>();
            if(panels != null && panels.Length != 0)
            {
                foreach (CollectionGoalPanel panel in panels)
                {
                    if (panel != null && panel.isActiveAndEnabled)
                    {
                        panel.UpdatePanel();
                    }
                }
            }
        }

    }
    public void UpdateCollection()
    {
        UpdateCollection(layout);
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
        if(layout != null && layout.activeSelf == true)
        {
            layout.SetActive(_state);
        }
    }
}
