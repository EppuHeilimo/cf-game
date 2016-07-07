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
}