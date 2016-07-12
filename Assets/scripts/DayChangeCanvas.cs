using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
public class DayChangeCanvas : MonoBehaviour {
    public GameObject patientprefab;
    int day = 1;
    List<GameObject> mynpcs;
    NPCManagerV2 npcmanager;
    int score = 50;
    int speed = 5;
    int totalscore = 0;
    int oldtotalscore = 0;
    bool initialized = false;
    bool complete = false;
    Transform patientspanel;
    Transform performancepanel;
    Transform totalscoretextpanel;
    List<GameObject> patientpanels = new List<GameObject>();
    // Use this for initialization
    void Start () {
        day = GameObject.FindGameObjectWithTag("Clock").GetComponent<ClockTime>().day;
        mynpcs = GameObject.FindGameObjectWithTag("NPCManager").GetComponent<NPCManagerV2>().responsibilityNpcs;
        score = GameObject.FindGameObjectWithTag("ScoringSystem").GetComponent<ScoringSystem>().score;
        npcmanager = GameObject.FindGameObjectWithTag("NPCManager").GetComponent<NPCManagerV2>();
        performancepanel = transform.FindChild("Performance").FindChild("ScoreBar");
        totalscoretextpanel = transform.FindChild("Footer").FindChild("Score");
        totalscore = GameObject.FindGameObjectWithTag("ScoringSystem").GetComponent<ScoringSystem>().totalscore;
        oldtotalscore = GameObject.FindGameObjectWithTag("ScoringSystem").GetComponent<ScoringSystem>().oldtotalscore;
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
            foreach(NPCINFO npc in npcmanager.respNpcsWhoLeftOrDied)
            {
                GameObject patient = Instantiate(patientprefab, Vector3.zero, Quaternion.identity) as GameObject;
                patient.transform.FindChild("Image").GetComponent<Image>().sprite = npc.headimage;
                patient.transform.FindChild("Name").GetComponent<Text>().text = npc.name;
                if(npc.dead)
                {
                    patient.transform.FindChild("dead").gameObject.SetActive(true);
                }
                else
                    patient.transform.FindChild("left").gameObject.SetActive(true);
                patient.transform.SetParent(patientspanel);
                patientpanels.Add(patient);
            }
            performancepanel.FindChild("Positive").GetComponent<RectTransform>().sizeDelta = new Vector2(score * 2, 30);
            performancepanel.FindChild("Percentage").GetComponent<Text>().text = score + "%";
            totalscoretextpanel.GetComponent<Text>().text = oldtotalscore.ToString();
        }
        if(initialized && !complete)
        {
            if(oldtotalscore < totalscore)
            {
                int sub = totalscore - oldtotalscore;
                if (sub < 10)
                    speed = 1;
                else if (sub < 100)
                    speed = 2;
                oldtotalscore += 1 * speed;
                totalscoretextpanel.GetComponent<Text>().text = oldtotalscore.ToString();
            }
            else
            {
                totalscoretextpanel.GetComponent<Text>().text = totalscore.ToString();
                complete = true;
                speed = 5;
            }
            
        }
	}

    public void continueButton()
    {
        GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>().continueToNextDay();
    }
}
