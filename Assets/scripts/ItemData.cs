using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;

public class ItemData : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler {
    public Item item;
    public int slot;

    Inventory inv;
    Tooltip tooltip;
    Vector2 offset;
    UIManager uiManager;

    void Start()
    {
        inv = GameObject.Find("Inventory").GetComponent<Inventory>();
        uiManager = GameObject.Find("UIManager").GetComponent<UIManager>();
        tooltip = inv.GetComponent<Tooltip>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (item != null)
        {
            this.transform.SetParent(this.transform.parent.parent);
            GetComponent<CanvasGroup>().blocksRaycasts = false;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (item != null)
        {
            this.transform.position = eventData.position - offset;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        this.transform.SetParent(inv.slots[slot].transform);
        this.transform.position = inv.slots[slot].transform.position;
        GetComponent<CanvasGroup>().blocksRaycasts = true;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (item != null)
        {
            // give medicine to NPC with right click
            if (eventData.pointerId == -2)
            {
                // remove medicine from inventory if giving it succeeds
                if (uiManager.giveMed(item.Title))
                {
                    inv.RemoveItem(item.ID);
                    tooltip.Deactivate();
                }
            }
            else
            { 
                offset = eventData.position - new Vector2(this.transform.position.x, this.transform.position.y);
                this.transform.position = eventData.position - offset;
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        tooltip.Activate(item);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tooltip.Deactivate();
    }
}
