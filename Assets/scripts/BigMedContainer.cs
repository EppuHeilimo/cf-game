using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BigMedContainer : MonoBehaviour {

    string medName;
    float defaultDosage;
    GameObject invObj;
    Inventory playerInv;
    ItemDatabase database;

    void Start()
    {
        invObj = GameObject.Find("Inventory");
        playerInv = invObj.GetComponent<Inventory>();
        database = invObj.GetComponent<ItemDatabase>();
    }

    public void OpenMed(string medName, float defaultDosage)
    {
        this.medName = medName;
        this.defaultDosage = defaultDosage;
        gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Items/" + medName + "_cont");
    }

    public void AddItem()
    {
        Item itemToAdd = database.FetchItemByTitle(medName);
        if (itemToAdd != null)
        {
            playerInv.AddItem(itemToAdd.ID);
        }     
    }
}
