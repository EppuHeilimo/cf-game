using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

/* medicine cups in the slingshot minigame */
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

    Text medsInThisCupTxt;
    Med lastMed;
    int lastPill;

    void OnTriggerEnter2D(Collider2D other)
    {
        // pill enters the trigger, add the pill to this cup
        if (other.gameObject.tag == "Pill")
        {
            if (isColliding) return;
            isColliding = true;
            GameObject pillObj = other.gameObject;   
            Pill pill = pillObj.GetComponent<Pill>();
            pill.doNotDestroy = true;
            Med med = new Med();
            med.name = pill.medName;
            med.dosage = pill.dosage;
            Add(med);
            pills.Add(other.gameObject);
            lastPill = pills.Count - 1;
            other.gameObject.tag = "disabledPill";
            GetComponent<AudioSource>().Play();
            UpdateText();
        }
    }

    void Update()
    {
        isColliding = false;
    }

    // adds medicine to this cup
    // if duplicate -> add up dosage
    public void Add(Med m)
    {
        lastMed = m;
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

    // remove all meds in this cup
    public void Reset()
    {
        medsInThisCup.Clear();
        DestroyPills();
        UpdateText();
    }

    // remove last pill added to this cup
    public void DeleteLast()
    {
        if (lastMed == null)
        {
            Reset();
        }
        else
        {
            for (int i = 0; i < medsInThisCup.Count; i++)
            {
                if (medsInThisCup[i].name == lastMed.name)
                {
                    medsInThisCup[i].dosage -= lastMed.dosage;
                    if (medsInThisCup[i].dosage <= 0)
                        medsInThisCup.RemoveAt(i);
                    lastMed = null;
                    break;
                }
            }
            Destroy(pills[lastPill]);
            pills.RemoveAt(lastPill);
            UpdateText();
        }
    }

    // destroys all pill gameobjects in this cup
    public void DestroyPills()
    {
        foreach (GameObject pill in pills)
            if (pill != null)
                Destroy(pill);
        pills.Clear();
    }

    // updates the text of medicines in this cup when new one added/removed
    void UpdateText()
    {
        medsInThisCupTxt = transform.GetChild(0).GetChild(1).GetComponent<Text>();
        medsInThisCupTxt.text = "";
        for (int i = 0; i < medsInThisCup.Count; i++)
        {
            medsInThisCupTxt.text += medsInThisCup[i].name + " " + medsInThisCup[i].dosage + " mg" + "\n";
        }
    }
}
