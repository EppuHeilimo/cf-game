using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BigMedContainer : MonoBehaviour {

    string medName;
    int defaultDosage;

    public void OpenMed(string medName, int defaultDosage)
    {
        this.medName = medName;
        this.defaultDosage = defaultDosage;
        gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Items/" + medName + "_cont");
    }

    public void print()
    {
        print(medName + defaultDosage);
    }
}
