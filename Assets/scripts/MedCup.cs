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
    bool isColliding;
    Ray ray;
    RaycastHit hit;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (isColliding) return;
        isColliding = true;
        print("kutsuttu");
        GameObject pillObj = other.gameObject;   
        Pill pill = pillObj.GetComponent<Pill>();
        Med med = new Med();
        med.name = pill.medName;
        med.dosage = pill.dosage;
        Add(med);
        pills.Add(other.gameObject);
        other.gameObject.tag = "disabledPill";
        Invoke("DisableRotation", 3f);
    }

    void Update()
    {
        isColliding = false;
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.tag == "morningCup")
                print("ASDDDD");
        }
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
