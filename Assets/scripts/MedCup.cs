using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MedCup : MonoBehaviour {

    public class Med
    {
        public string name;
        public int dosage;

        public override string ToString()
        {
            return name + " " + dosage;
        }
    };

    public List<Med> medsInThisCup = new List<Med>();
    public List<GameObject> pills = new List<GameObject>();
    public string cupName;

    void OnTriggerEnter2D(Collider2D other)
    {
        GameObject pillObj = other.gameObject;
        Pill pill = pillObj.GetComponent<Pill>();
        Med med = new Med();
        med.name = pill.medName;
        med.dosage = pill.dosage;
        medsInThisCup.Add(med);
        //Destroy(other.gameObject);
        pills.Add(other.gameObject);
        other.gameObject.tag = "Untagged";
    }

    public void Reset()
    {
        medsInThisCup.Clear();
        foreach (GameObject pill in pills)
            Destroy(pill);
        pills.Clear();
    }

}
