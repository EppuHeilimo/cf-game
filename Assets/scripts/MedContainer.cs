using UnityEngine;
using System.Collections;

public class MedContainer : MonoBehaviour {

    public string medName;
    public float defaultDos;
    GameObject minigameObj;
    Minigame1 minigame;
    GameObject bigMedContObj;
    BigMedContainer bigMedCont;

    void Start()
    {
        minigameObj = GameObject.Find("Minigame1");
        minigame = minigameObj.GetComponent<Minigame1>();
        bigMedContObj = GameObject.Find("BigMedContainer");
        bigMedCont = bigMedContObj.GetComponent<BigMedContainer>();
    }

    public void clicked()
    {
        if (!minigame.kasiDesi)
        {
            print("ota nyt se vitun käsidesi eka plz...");
        }
        else
        {
            bigMedCont.OpenMed(medName, defaultDos);
        }
    }
}
