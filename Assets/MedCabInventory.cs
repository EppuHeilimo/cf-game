using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class MedCabInventory : MonoBehaviour {
    GameObject inventoryPanel;
    GameObject slotPanel;
    ItemDatabase database;
    public GameObject inventorySlot;
    public GameObject inventoryItem;

    public List<Item> items = new List<Item>();
    public List<GameObject> slots = new List<GameObject>();

    void Start()
    {
        database = GameObject.Find("Inventory").GetComponent<ItemDatabase>();
        inventoryPanel = GameObject.Find("MedCabPanel");
        slotPanel = inventoryPanel.transform.FindChild("MedCabSlotPanel").gameObject;
        for (int i = 0; i < database.database.Count; i++)
        {
            items.Add(new Item());
            slots.Add(Instantiate(inventorySlot));
            slots[i].GetComponent<Slot>().id = i;
            slots[i].transform.SetParent(slotPanel.transform);
        }
        FillMedCab();
    }

    public void FillMedCab()
    {
        for (int j = 0; j < database.database.Count; j++)
        {
            Item itemToAdd = database.FetchItemByID(j);
            bool foundEmptySlot = false;
            for (int i = 0; i < items.Count; i++)
            {
                // ID -1 = empty slot
                if (items[i].ID == -1)
                {
                    foundEmptySlot = true;
                    items[i] = itemToAdd;
                    GameObject itemObj = Instantiate(inventoryItem);
                    itemObj.GetComponent<ItemData>().item = itemToAdd;
                    itemObj.GetComponent<ItemData>().slot = i;
                    // make the object child of the corresponding slot
                    itemObj.transform.SetParent(slots[i].transform);
                    //itemObj.transform.position = slots[i].transform.position;
                    itemObj.transform.localScale = Vector3.one;
                    itemObj.GetComponent<RectTransform>().sizeDelta = Vector2.zero;
                    itemObj.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                    itemObj.GetComponent<Image>().sprite = itemToAdd.Sprite;
                    break;
                }
            }
            if (!foundEmptySlot)
            {
                // no empty slots, show "inventory full" -message
                Debug.Log("Inventaario on täynnä!");
            }
        }       
    }
}
