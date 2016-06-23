using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ItemContainer
{
    public int ID;
    public List<Item> medicine = new List<Item>();
    public Sprite mySprite;

    public ItemContainer()
    {
        ID = -1;
        mySprite = Resources.Load<Sprite>("Sprites/Items/null");
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
        int[] test = { 0, 1, 2, 3 };
        AddItems(test);
        AddItem(0);
        AddItem(0);
        RemoveItem(0);
        AddItem(1);

    }

    public void AddItem(int id)
    {
        Item itemToAdd = database.FetchItemByID(id);
        ItemContainer container = new ItemContainer();
        bool foundEmptySlot = false;
        for (int i = 0; i < items.Count; i++)
        {
            // ID -1 = empty slot
            if (items[i].ID == -1)
            {
                items[i].ID = 1;
                foundEmptySlot = true;
                items[i].medicine.Add(itemToAdd);
                GameObject itemObj = Instantiate(inventoryItem);
                itemObj.GetComponent<ItemData>().item = items[i];
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

    public void AddItems(int[] ids)
    {
        Item[] itemsToAdd = new Item[ids.Length];
        int i = 0;
        foreach (int id in ids)
        {
            itemsToAdd[i] = database.FetchItemByID(id);
            i++;
        }    
        ItemContainer container = new ItemContainer();
        bool foundEmptySlot = false;
        for (i = 0; i < items.Count; i++)
        {
            // ID -1 = empty slot
            if (items[i].ID == -1)
            {
                items[i].ID = 1;
                foundEmptySlot = true;
                for(int j = 0; j < itemsToAdd.Length; j++)
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
                itemObj.GetComponent<Image>().sprite = items[i].mySprite;
                break;
            }
        }
        if (!foundEmptySlot)
        {
            // no empty slots, show "inventory full" -message
            Debug.Log("Inventaario on täynnä!");
        }
    }

    public void AddItems(List<string> titles, List<int> dosages)
    {
        Item[] itemsToAdd = new Item[titles.Count];
        int i = 0;
        foreach (string title in titles)
        {
            itemsToAdd[i] = database.FetchItemByTitle(title);
            itemsToAdd[i].currentDosage = dosages[i];
            i++;
        }
        ItemContainer container = new ItemContainer();
        bool foundEmptySlot = false;
        for (i = 0; i < items.Count; i++)
        {
            // ID -1 = empty slot
            if (items[i].ID == -1)
            {
                items[i].ID = 1;
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
                itemObj.GetComponent<Image>().sprite = items[i].mySprite;
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
}
