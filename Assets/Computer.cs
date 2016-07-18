using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Computer : MonoBehaviour {

    public GameObject computerCanvas;
    public GameObject patientInfoPrefab;
    private GameObject computerCanvasClone;

    GameObject patientpanel;
    GameObject taskWindow;
    GameObject databaseWindow;

    List<GameObject> patients = new List<GameObject>();

    NPCManagerV2 npcmanager;
    List<NPCV2> npcList = new List<NPCV2>();
    int currentnpc;

	// Use this for initialization
	void Start () {
        npcmanager = GameObject.FindGameObjectWithTag("NPCManager").GetComponent<NPCManagerV2>();    
	}
	
	// Update is called once per frame
	void Update () {
	    
	}

    public void StartComputer()
    {
        GameObject.FindGameObjectWithTag("UIManager").GetComponent<UIManager>().pause(true);
        computerCanvasClone = Instantiate(computerCanvas);
        taskWindow = computerCanvasClone.transform.FindChild("Computer").FindChild("TaskWindow").gameObject;
        databaseWindow = computerCanvasClone.transform.FindChild("Computer").FindChild("DBWindow").gameObject;
        patientpanel = computerCanvasClone.transform.FindChild("Computer").FindChild("DBWindow").FindChild("Patients").FindChild("PatientPanel").gameObject;
        taskWindow.SetActive(false);
        databaseWindow.SetActive(false);
        npcList.Clear();
        foreach (GameObject go in npcmanager.responsibilityNpcs)
        {
            npcList.Add(go.GetComponent<NPCV2>());
        }
        /*
        foreach (NPCV2 npc in npcList)
        {
            GameObject patient = Instantiate(patientInfoPrefab, Vector3.zero, Quaternion.identity) as GameObject;
            patient.transform.FindChild("Image").GetComponent<Image>().sprite = npc.myHead2d;
            patient.transform.FindChild("Name").GetComponent<Text>().text = npc.myName;
            patient.transform.FindChild("ID").GetComponent<Text>().text = npc.myId;
            patient.transform.SetParent(patientspanel.transform);
            patients.Add(patient);
        } */
    }

    public NPCV2 NextPatient()
    {
        NPCV2 nextnpc;
        int next = currentnpc + 1;
        if(next > npcList.Count - 1)
        {
            currentnpc = 0;
            nextnpc = npcList[0];
            setPatientActive(nextnpc);
        }
        else
        {
            currentnpc = next;
            nextnpc = npcList[next];
            setPatientActive(nextnpc);
        }
        return nextnpc;
    }

    public NPCV2 PrevPatient()
    {
        NPCV2 prevnpc;
        int prev = currentnpc - 1;
        if (prev < 0)
        {
            currentnpc = npcList.Count - 1;
            prevnpc = npcList[npcList.Count - 1];
            setPatientActive(prevnpc);
        }
        else
        {
            currentnpc = prev;
            prevnpc = npcList[prev];
            setPatientActive(prevnpc);
        }
        return prevnpc;
    }



    private void setPatientActive(NPCV2 npc)
    {
        patientpanel.transform.FindChild("Image").GetComponent<Image>().sprite = npc.myHead2d;
        patientpanel.transform.FindChild("Name").GetComponent<Text>().text = npc.myName;
        patientpanel.transform.FindChild("Id").GetComponent<Text>().text = npc.myId;

    }

    public void openTaskWindow()
    {
        taskWindow.SetActive(true);
        moveWindowToFront(taskWindow.transform);
    }

    public NPCV2 openDBWindow()
    {
        NPCV2 ret = null;
        databaseWindow.SetActive(true);
        moveWindowToFront(databaseWindow.transform);
        if(npcList.Count > 0)
        {
            setPatientActive(npcList[0]);
            ret = npcList[0];

        }
        return ret;
            
    }

    public void closeDB()
    {
        databaseWindow.SetActive(false);
    }

    public void closeTasks()
    {
        taskWindow.SetActive(false);
    }

    void moveWindowToFront(Transform obj)
    {
        obj.SetAsLastSibling();
    }

    public void ShutdownComputer()
    {
        GameObject.FindGameObjectWithTag("UIManager").GetComponent<UIManager>().pause(false);
        Destroy(computerCanvasClone);
    }
}
