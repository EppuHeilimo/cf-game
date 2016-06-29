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
        pills.Add(other.gameObject);
        other.gameObject.tag = "Untagged";
        Invoke("DisableRotation", 3f);
    }

    public void Clear()
    {
        medsInThisCup.Clear();
    }

    public void Reset()
    {
        medsInThisCup.Clear();
        DestroyPills();
    }

    public void DestroyPills()
    {
        foreach (GameObject pill in pills)
            if (pill != null)
                Destroy(pill);
        pills.Clear();
    }

    void DisableRotation()
    {
        if (pills.Count > 0)
        { 
            foreach (GameObject pill in pills)
                if (pill != null)
                    pill.GetComponent<Rigidbody2D>().freezeRotation = true;
        }
    }
}
