using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
public class Item : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    Image image;
    RectTransform rect;
    Vector3 startPos;

    Board board;
    [SerializeField] Tile tile;

    public static GameObject activeObject;
    [SerializeField] Text itemText;
    [SerializeField] string instruct;

    bool isEnabled;
    bool isDrag = true;
    bool isLock = false;

    public List<CanvasGroup> canvasGroups;
    public UnityEvent unityEvent;
    [SerializeField] int time = 15;

    private void Awake()
    {
        image = GetComponent<Image>();
        rect = GetComponent<RectTransform>();
        board = FindObjectOfType<Board>();
    }

    void Start()
    {
        EnableItem(false);
    }
    public void EnableItem(bool _state)
    {
        isEnabled = _state;
        if (_state)
        {
            DisableItem();
            activeObject = gameObject;
        }
        else if (gameObject == activeObject)
        {
            activeObject = null;
        }
        image.color = (_state) ? Color.white : Color.gray;

        if(itemText != null)
        {
            itemText.gameObject.SetActive(activeObject != null);

            if(gameObject == activeObject)
            {
                itemText.text = instruct;
            }
        }
    }

    public void ToogleItem()
    {
        EnableItem(!isEnabled);
    }
    void DisableItem()
    {
        Item[] items = FindObjectsOfType<Item>();

        foreach(Item item in items)
        {
            if(item != this)
            {
                item.EnableItem(false);
            }
        }
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        if(isEnabled && isDrag && !isLock)
        {
            startPos = gameObject.transform.position;
            EnableCanvas(false);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isEnabled && isDrag && !isLock && Camera.main != null)
        {
            Vector3 onScreen;
            RectTransformUtility.ScreenPointToWorldPointInRectangle(rect, eventData.position, Camera.main, out onScreen);

            gameObject.transform.position = onScreen;

            RaycastHit2D hit = Physics2D.Raycast(onScreen, Vector3.forward, Mathf.Infinity);
            if (hit.collider != null)
            {
                tile = hit.collider.GetComponent<Tile>();
            }
            else
            {
                tile = null;
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if(isEnabled && isDrag && !isLock)
        {
            gameObject.transform.position = startPos;
            EnableCanvas(true);
            if(board != null && board.isRefilling)
            {
                return;
            }
            if(tile != null)
            {
                if(unityEvent != null)
                {
                    unityEvent.Invoke();
                }
                EnableItem(false);
                tile = null;
                activeObject = null;
            }
        }
    }

    void EnableCanvas(bool _state)
    {
        if(canvasGroups != null && canvasGroups.Count > 0)
        {
            foreach(CanvasGroup c in canvasGroups)
            {
                if(c != null)
                {
                    c.blocksRaycasts = _state;
                }
            }
        }
    }

    public void RemoveCandy()
    {
        if(board != null && tile != null)
        {
            board.ClearAndRefill(tile.xIndex, tile.yIndex);
        }
    }

    public void AddTime()
    {
        if(GameManager.Instance != null)
        {
            GameManager.Instance.AddTime(time);
        }
    }

    public void MakeColorBomb()
    {
        if(board != null && tile != null)
        {
            board.MakeColorBomb(tile.xIndex, tile.yIndex);
        }
    }
}
