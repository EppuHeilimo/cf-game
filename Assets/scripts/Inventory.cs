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
        slotAmount = 3;
        inventoryPanel = GameObject.Find("Inventory Panel");
        slotPanel = inventoryPanel.transform.FindChild("Slot Panel").gameObject;
        // create 3 empty slots and items
        for (int i = 0; i < slotAmount; i++)
        {
            items.Add(new Item());
            slots.Add(Instantiate(inventorySlot));
            slots[i].GetComponent<Slot>().id = i;
            slots[i].transform.SetParent(slotPanel.transform);
        }

        AddItem(0);
        AddItem(1);
        RemoveItem(1);
        AddItem(1);
    }

    public void AddItem(int id)
    {
        Item itemToAdd = database.FetchItemByID(id);
        bool foundEmptySlot = false;
        for (int i = 0; i < items.Count; i++)
        {
<<<<<<< HEAD
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].ID == id)
                {
                    ItemData data = slots[i].transform.GetChild(0).GetComponent<ItemData>();
                    data.amount++;
                    data.transform.GetChild(0).GetComponent<Text>().text = data.amount.ToString();
                    
                    break;
                }
            }
        }
        else
        { 
            for (int i = 0; i < items.Count; i++)
            {
                // ID -1 = empty slot
                if (items[i].ID == -1)
                {
                    items[i] = itemToAdd;
                    GameObject itemObj = Instantiate(inventoryItem);
                    itemObj.GetComponent<ItemData>().item = itemToAdd;
                    itemObj.GetComponent<ItemData>().amount = 1;
                    itemObj.GetComponent<ItemData>().slot = i;
                    // make the object child of the corresponding slot
                    itemObj.transform.SetParent(slots[i].transform);
                    itemObj.transform.position = Vector2.zero;
                    itemObj.GetComponent<Image>().sprite = itemToAdd.Sprite;
                    itemObj.name = itemToAdd.Title;
                    break;
                }
=======
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
                itemObj.transform.position = Vector2.zero;
                itemObj.GetComponent<Image>().sprite = itemToAdd.Sprite;
                break;
>>>>>>> 39bf8b359089bad04555c85874e0e00e9acaad67
            }
        }
        if (!foundEmptySlot)
        {
            // no empty slots, show "inventory full" -message
            Debug.Log("täynnä!");
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
