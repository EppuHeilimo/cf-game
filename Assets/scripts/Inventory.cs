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
        slotAmount = 16;
        inventoryPanel = GameObject.Find("Inventory Panel");
        slotPanel = inventoryPanel.transform.FindChild("Slot Panel").gameObject;
        // create 20 empty slots and items
        for (int i = 0; i < slotAmount; i++)
        {
            items.Add(new Item());
            slots.Add(Instantiate(inventorySlot));
            slots[i].GetComponent<Slot>().id = i;
            slots[i].transform.SetParent(slotPanel.transform);
        }

        AddItem(0);
        AddItem(1);
        AddItem(1);
        AddItem(1);
        AddItem(1);
    }

    public void AddItem(int id)
    {
        Item itemToAdd = database.FetchItemByID(id);
        // stack item if it's already in inventory
        if (CheckIfItemIsInInventory(itemToAdd))
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].ID == id)
                {
                    ItemData data = slots[i].transform.GetChild(0).GetComponent<ItemData>();
                    data.amount++;
                    data.transform.GetChild(0).GetComponent<Text>().text = data.amount.ToString();
                    print("asd");
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
                    print("ddd");
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
            }
        }
    }

    bool CheckIfItemIsInInventory(Item item)
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].ID == item.ID)
                return true;
        }
        return false;
    }

    public void RemoveItem(int id)
    {
        Item itemToRemove = database.FetchItemByID(id);
        if (CheckIfItemIsInInventory(itemToRemove))
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].ID == id)
                {
                    ItemData data = slots[i].transform.GetChild(0).GetComponent<ItemData>();
                    data.amount--;
                    if (data.amount == 0)
                    {
                        items[i] = new Item();
                        Transform t = slots[i].transform.GetChild(0);
                        Destroy(t.gameObject);
                        print("yks");
                        break;
                    }
                    else
                    {
                        if (data.amount == 1)
                        {
                            data.transform.GetComponentInChildren<Text>().text = "";
                            print("kaks");
                            break;
                        }
                        else
                        {
                            data.transform.GetComponentInChildren<Text>().text = data.amount.ToString();
                            print("kol");
                            break;
                        }
                    }
                }
            }
        }
    }
}
