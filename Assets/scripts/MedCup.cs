using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MedCup : MonoBehaviour {

    public class Med
    {
        public string name;
        public int dosage;
    };

    public List<Med> medsInThisCup = new List<Med>();
    public string cupName;

    void OnTriggerEnter2D(Collider2D other)
    {
        GameObject pillObj = other.gameObject;
        Pill pill = pillObj.GetComponent<Pill>();
        Med med = new Med();
        med.name = pill.medName;
        med.dosage = pill.dosage;
        medsInThisCup.Add(med);
        Destroy(other.gameObject);
    }

    public void Reset()
    {
        medsInThisCup.Clear();
    }

}
