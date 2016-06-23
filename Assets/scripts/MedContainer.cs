using UnityEngine;
using System.Collections;

public class MedContainer : MonoBehaviour {
    public string medName;
    public int defaultDos;
    GameObject minigameObj;
    Minigame1 minigame;

    void Start()
    {
        minigameObj = GameObject.Find("Minigame1");
        minigame = minigameObj.GetComponent<Minigame1>();
    }

    public void clicked()
    {
        if (!minigame.kasiDesi)
        {
            print("ota nyt se vitun käsidesi eka plz...");
        }
        else
        {
            minigame.startDosingGame(medName, defaultDos);
        }
    }
}
