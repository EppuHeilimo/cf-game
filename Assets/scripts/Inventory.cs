using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Inventory : MonoBehaviour {
    GameObject inventoryPanel;
    GameObject slotPanel;
    ItemDatabase database;
    public GameObject inventorySlot;
    public GameObject inventoryItem;

    int slotAmount;
    public List<Item> items = new List<Item>();
    public List<GameObject> slots = new List<GameObject>();

    void Start()
    {
        database = GetComponent<ItemDatabase>();
        slotAmount = 4;
        inventoryPanel = GameObject.Find("Inventory Panel");
        slotPanel = inventoryPanel.transform.FindChild("Slot Panel").gameObject;
        // create 4 empty slots and items
        for (int i = 0; i < slotAmount; i++)
        {
            items.Add(new Item());
            slots.Add(Instantiate(inventorySlot));
            slots[i].GetComponent<Slot>().id = i;
            slots[i].transform.SetParent(slotPanel.transform);
        }

        AddItem(0);
        AddItem(1);

    }

    public void AddItem(int id)
    {
        Item itemToAdd = database.FetchItemByID(id);
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
                itemObj.transform.position = slots[i].transform.position;
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

    public void RemoveItem(int id)
    {
        Item itemToRemove = database.FetchItemByID(id);
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].ID == id)
            {
                items[i] = new Item();
                foreach (Transform child in slots[i].transform)
                {
                    Destroy(child.gameObject);
                }
                break;
            }       
        }
    }
}
