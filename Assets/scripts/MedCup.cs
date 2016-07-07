﻿using UnityEngine;
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

    Text medsInThisCupTxt;
    Med lastMed;
    int lastPill;

    void OnTriggerEnter2D(Collider2D other)
    {
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
            UpdateText();
        }
    }

    void Update()
    {
        isColliding = false;
    }

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

    public void Reset()
    {
        medsInThisCup.Clear();
        DestroyPills();
        UpdateText();
    }

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

    public void DestroyPills()
    {
        foreach (GameObject pill in pills)
            if (pill != null)
                Destroy(pill);
        pills.Clear();
    }

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
