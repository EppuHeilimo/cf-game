using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class MedCabinet : MonoBehaviour
{
    GameObject invObj;
    public Text input;
    Inventory playerInv;
    GameObject inventoryPanel;
    ItemDatabase database;
    string title;

    void Start()
    {
        invObj = GameObject.Find("Inventory");
        playerInv = invObj.GetComponent<Inventory>();
        database = invObj.GetComponent<ItemDatabase>();
        inventoryPanel = GameObject.Find("Med Cabinet Panel");
        inventoryPanel.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        inventoryPanel.SetActive(true);
        //TODO: check if player is close enough to the cabinet
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            inventoryPanel.SetActive(false);
        }
    }

    public void AddItem()
    {
        print("ASD");
        title = input.text.ToString();
        Item itemToAdd = database.FetchItemByTitle(title);
        if (itemToAdd == null)
        {
            print(title + " nimistä lääkettä ei ole olemassa.");
            return;
        }
        playerInv.AddItem(itemToAdd.ID);
    }
}