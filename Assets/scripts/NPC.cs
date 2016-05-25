using UnityEngine;
using System.Collections;

public class NPC : MonoBehaviour
{
    public string myName;
    public string myId;
    bool atReception;
    bool atQue;
    NavMeshAgent agent;
    ReceptionManager receptionManager;
    Vector3 receptionEntryPos;

    // Use this for initialization
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();   
        receptionManager = GameObject.Find("ReceptionManager").GetComponent<ReceptionManager>();
        receptionEntryPos = new Vector3(-155, 0, -23);
        moveTo(receptionEntryPos);
    }

    // Update is called once per frame
    void Update()
    {
        if (Mathf.Approximately(transform.position.x, receptionEntryPos.x) && Mathf.Approximately(transform.position.z, receptionEntryPos.z) && !atReception)
        {
            atReception = true;
        }

        if (atReception)
        {
            atQue = true;
            atReception = false;
            float quePosX = receptionManager.addToQue();
            Vector3 quePosVec = new Vector3(quePosX, 0, 130);
            moveTo(quePosVec); 
        }

        if (atQue)
        {
            //print(this.myName + " jonossa");
        }
    }

    public void Init(string myName, string myId)
    {
        this.myName = myName;
        this.myId = myId;
        //print("Uusi potilas spawnattu!");
        //print("myName: " + this.myName);
        //print("myId: " + this.myId);
    }

    public void moveTo(Vector3 dest)
    {
        agent.SetDestination(dest);
    }
}
