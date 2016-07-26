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

    // called when medicine clicked in the cabinet
    public void clicked()
    {
        // check if hand disinfectant used
        if (!minigame.kasiDesi)
        {
            minigame.kasDesObj.GetComponent<KasiDesi>().StartBlinking();
        }
        // start the slingshot game
        else
        {
            minigame.startDosingGame(medName, defaultDos, canSplit);
        }
    }
}
