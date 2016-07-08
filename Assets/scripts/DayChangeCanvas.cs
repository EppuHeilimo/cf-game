using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
public class DayChangeCanvas : MonoBehaviour {
    public GameObject patientprefab;
    int day = 1;
    GameObject[] mynpcs;
    int score = 50;
    bool initialized = false;
    Transform mainpanel;
    Transform patientspanel;
    List<GameObject> patientpanels = new List<GameObject>();
    // Use this for initialization
    void Start () {
        day = GameObject.FindGameObjectWithTag("Clock").GetComponent<ClockTime>().day;
        mynpcs = GameObject.FindGameObjectWithTag("NPCManager").GetComponent<NPCManagerV2>().responsibilityNpcs.ToArray();
        score = GameObject.FindGameObjectWithTag("ScoringSystem").GetComponent<ScoringSystem>().score;
	}
	
	// Update is called once per frame
	void Update () {
	    if(!initialized)
        {
            initialized = true;
            patientspanel = transform.FindChild("PatientPanel").FindChild("Patients");
            foreach(GameObject npc in mynpcs)
            {
                GameObject patient = Instantiate(patientprefab, Vector3.zero, Quaternion.identity) as GameObject;
                patient.transform.FindChild("Image").GetComponent<Image>().sprite = npc.GetComponent<NPCV2>().myHead2d;
                patient.transform.FindChild("Name").GetComponent<Text>().text = npc.GetComponent<NPCV2>().myName;
                patient.transform.SetParent(patientspanel);
                patientpanels.Add(patient);
            }
        }
	}

    public void continueButton()
    {
        GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>().continueToNextDay();
    }
}
