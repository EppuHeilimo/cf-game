using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

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
    bool isColliding;
    Ray ray;
    RaycastHit hit;
    Text medsInThisCupTxt;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (isColliding) return;
        isColliding = true;
        GameObject pillObj = other.gameObject;   
        Pill pill = pillObj.GetComponent<Pill>();
        Med med = new Med();
        med.name = pill.medName;
        med.dosage = pill.dosage;
        Add(med);
        pills.Add(other.gameObject);
        other.gameObject.tag = "disabledPill";
        Invoke("DisableRotation", 1f);
        UpdateText();
    }

    void Update()
    {
        isColliding = false;
    }

    public void Add(Med m)
    {
        if (medsInThisCup.Count == 0)
            medsInThisCup.Add(m);
        else
        {
            bool found = false;
            for (int i = 0; i < medsInThisCup.Count; i++)
            {
                if (medsInThisCup[i].name == m.name)
                {
                    medsInThisCup[i].dosage += m.dosage;
                    found = true;
                    break;
                }
            }
            if (!found)
                medsInThisCup.Add(m);
        }
    }

    public void Reset()
    {
        medsInThisCup.Clear();
        DestroyPills();
        UpdateText();
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

    void UpdateText()
    {
        medsInThisCupTxt = transform.GetChild(0).GetChild(1).GetComponent<Text>();
        medsInThisCupTxt.text = "";
        for (int i = 0; i < medsInThisCup.Count; i++)
        {
            medsInThisCupTxt.text += medsInThisCup[i].name + " " + medsInThisCup[i].dosage + "\n";
        }
    }
}
