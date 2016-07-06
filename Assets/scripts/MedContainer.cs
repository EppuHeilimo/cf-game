using UnityEngine;
using System.Collections;

public class MedContainer : MonoBehaviour {
    public string medName;
    public int defaultDos;
    public int canSplit;
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
            minigame.kasDesObj.GetComponent<KasiDesi>().StartBlinking();
        }
        else
        {
            minigame.startDosingGame(medName, defaultDos, canSplit);
        }
    }
}
