using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ItemContainer
{
    public int ID;
    public List<Item> medicine = new List<Item>();
    /* Sprite showed in inventory slot */
    public Sprite mySprite;
    
    public ItemContainer()
    {
        ID = -1;
    }
}

public class Inventory : MonoBehaviour {
    GameObject inventoryPanel;
    GameObject slotPanel;
    ItemDatabase database;
    public GameObject inventorySlot;
    public GameObject inventoryItem;
    int slotAmount;
    public List<ItemContainer> items = new List<ItemContainer>();
    public List<GameObject> slots = new List<GameObject>();
    public Sprite[] cupSprites = new Sprite[4];

    void Start()
    {
        database = GetComponent<ItemDatabase>();
        slotAmount = 4;
        inventoryPanel = GameObject.Find("Inventory Panel");
        slotPanel = inventoryPanel.transform.FindChild("Slot Panel").gameObject;
        // create 4 empty slots and items
        for (int i = 0; i < slotAmount; i++)
        {
            items.Add(new ItemContainer());
            slots.Add(Instantiate(inventorySlot));
            slots[i].GetComponent<Slot>().id = i;
            slots[i].transform.SetParent(slotPanel.transform);
        }

    }

    public void AddItems(List<MedCup.Med> itemsInCup, int cupNum)
    {
        Item[] itemsToAdd = new Item[itemsInCup.Count];
        int i = 0;
        foreach (MedCup.Med m in itemsInCup)
        {
            Item newItem = new Item();
            newItem = (Item)database.FetchItemByTitle(m.name).Clone();
            newItem.currentDosage = m.dosage;
            itemsToAdd[i] = newItem;
            i++;
        }
        bool foundEmptySlot = false;
        for (i = 0; i < items.Count; i++)
        {
            // ID -1 = empty slot
            if (items[i].ID == -1)
            {
                items[i].ID = i;
                foundEmptySlot = true;
                for (int j = 0; j < itemsToAdd.Length; j++)
                {
                    items[i].medicine.Add(itemsToAdd[j]);
                }
                GameObject itemObj = Instantiate(inventoryItem);
                itemObj.GetComponent<ItemData>().item = items[i];
                itemObj.GetComponent<ItemData>().slot = i;
                // make the object child of the corresponding slot
                itemObj.transform.SetParent(slots[i].transform);
                //itemObj.transform.position = slots[i].transform.position;
                itemObj.transform.localScale = Vector3.one;
                itemObj.GetComponent<RectTransform>().sizeDelta = Vector2.zero;
                itemObj.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                itemObj.GetComponent<Image>().sprite = cupSprites[cupNum];
                break;
            }
        }
        if (!foundEmptySlot)
        {
            GameObject.FindGameObjectWithTag("Player").GetComponent<FloatTextNPC>().addFloatText("Inventory is full!", false);
        }
    }

    public void RemoveItem(int id)
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].ID == id)
            {
                items[i] = new ItemContainer();
                foreach (Transform child in slots[i].transform)
                {
                    Destroy(child.gameObject);
                }
                break;
            }       
        }
    }

    public void ResetInventory()
    {
        for (int i = 0; i < items.Count; i++)
        {
            items[i] = new ItemContainer();
            foreach (Transform child in slots[i].transform)
            {
                Destroy(child.gameObject);
            }
        }
    }
}
