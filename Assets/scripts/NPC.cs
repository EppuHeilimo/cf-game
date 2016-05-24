using UnityEngine;
using System.Collections;

public class NPC : MonoBehaviour
{
    public string nimi;
    public string hetu;
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
            //print(this.nimi + " jonossa");
        }
    }

    public void Init(string nimi, string hetu)
    {
        this.nimi = nimi;
        this.hetu = hetu;
        //print("Uusi potilas spawnattu!");
        //print("Nimi: " + this.nimi);
        //print("ID: " + this.hetu);
    }

    public void moveTo(Vector3 dest)
    {
        agent.SetDestination(dest);
    }
}
