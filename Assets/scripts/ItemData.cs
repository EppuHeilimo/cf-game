using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;

public class ItemData : MonoBehaviour,/* IBeginDragHandler,  IDragHandler, IEndDragHandler,*/ IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler {
    public ItemContainer item;
    public int slot;

    float taptimer = 0;
    bool tapped = false;
    const float DOUBLE_TAP_TIME = 1f;

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
    /*
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
    */
    public void OnPointerDown(PointerEventData eventData)
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            if (item != null)
            {
                if(!tapped)
                {
                    tapped = true;
                    tooltip.Activate(item);
                }
                else
                {
                    tapped = false;
                    // give medicine to NPC with second tap
                    for (int i = 0; i < Input.touchCount; ++i)
                    {
                        Touch touch = Input.GetTouch(i);
                        if (touch.phase == TouchPhase.Began)
                        {
                            // remove medicine from inventory if giving it succeeds
                            string[] titles = new string[item.medicine.Count];
                            float[] dosages = new float[item.medicine.Count];
                            int j = 0;
                            foreach (Item it in item.medicine)
                            {
                                titles[j] = it.Title;
                                dosages[j] = it.currentDosage;
                                j++;
                            }

                            if (uiManager.giveMed(titles, dosages))
                            {
                                inv.RemoveItem(item.ID);
                                tooltip.Deactivate();
                                break;
                            }
                        }
                    }
                }
            }
        }
        else
        {
            if (item != null)
            {
                string[] titles = new string[item.medicine.Count];
                float[] dosages = new float[item.medicine.Count];
                int j = 0;
                foreach (Item it in item.medicine)
                {
                    titles[j] = it.Title;
                    dosages[j] = it.currentDosage;
                    j++;
                }
                // remove medicine from inventory if giving it succeeds
                if (uiManager.giveMed(titles, dosages))
                {
                    inv.RemoveItem(item.ID);
                    tooltip.Deactivate();
                }
                else
                { 
                    offset = eventData.position - new Vector2(this.transform.position.x, this.transform.position.y);
                    this.transform.position = eventData.position - offset;
                }
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
