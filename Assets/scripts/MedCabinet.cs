using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class MedCabinet : MonoBehaviour
{
    GameObject miniGame;
    public bool playerInZone = false;

    void Start()
    {
        miniGame = GameObject.Find("Minigame1");
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
            playerInZone = true;
            
    }

    public void startMinigame()
    {
        miniGame.GetComponent<Minigame1>().startMinigame();
    }

    /*
    public void AddItem()
    {
        title = input.text.ToString();
        Item itemToAdd = database.FetchItemByTitle(title);
        if (itemToAdd == null)
        {
            print(title + " nimistä lääkettä ei ole olemassa.");
            return;
        }
        playerInv.AddItem(itemToAdd.ID);
    }
    */
}