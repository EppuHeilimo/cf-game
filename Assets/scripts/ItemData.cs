using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;

public class ItemData : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler {

    public ItemContainer item;
    public int slot;

    bool tapped = false;

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

    // player clicks item in inventory
    public void OnPointerDown(PointerEventData eventData)
    {
        // Android
        if (Application.platform == RuntimePlatform.Android)
        {
            if (item != null)
            {
                // first tap, show item's tooltip
                if(!tapped)
                {
                    tapped = true;
                    tooltip.Activate(item);
                }
                // 2nd tap
                else
                {
                    tapped = false;
                    // give medicine to NPC with second tap
                    for (int i = 0; i < Input.touchCount; ++i)
                    {
                        Touch touch = Input.GetTouch(i);
                        if (touch.phase == TouchPhase.Began)
                        {
                            // iterate through the tapped item's medicines + dosages
                            // and save them to arrays
                            string[] titles = new string[item.medicine.Count];
                            float[] dosages = new float[item.medicine.Count];
                            int j = 0;
                            foreach (Item it in item.medicine)
                            {
                                titles[j] = it.Title;
                                dosages[j] = it.currentDosage;
                                j++;
                            }

                            // pass the saved arrays to UIManager
                            // try giving item to NPC first, then try trashing it
                            // both return true if they succeed -> remove item from inventory
                            if (uiManager.giveMed(titles, dosages))
                            {
                                inv.RemoveItem(item.ID);
                                tooltip.Deactivate();
                                break;
                            }
                            else if(uiManager.trashItem())
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
        // same for PC, but without the mobile tapping stuff
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

                if (uiManager.giveMed(titles, dosages))
                {
                    inv.RemoveItem(item.ID);
                    tooltip.Deactivate();
                }
                else if (uiManager.trashItem())
                {
                    inv.RemoveItem(item.ID);
                    tooltip.Deactivate();
                }
                else
                {
                    // show tooltip above mouse cursor's position
                    offset = eventData.position - new Vector2(this.transform.position.x, this.transform.position.y);
                    this.transform.position = eventData.position - offset;
                }
            }
        }
    }

    // show/hide tooltip on mouse cursor hover/unhover
    public void OnPointerEnter(PointerEventData eventData)
    {
        tooltip.Activate(item);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tooltip.Deactivate();
    }
}
