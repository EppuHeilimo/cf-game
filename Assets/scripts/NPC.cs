using UnityEngine;
using System.Collections;

public class NPC : MonoBehaviour
{
    public string nimi;
    public string hetu;
    bool reachedDest;
    Vector3 waitingRoomPos;
    NavMeshAgent agent;

    // Use this for initialization
    void Start()
    {
        reachedDest = false;
        waitingRoomPos = new Vector3(-20, 0, 130);
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!reachedDest)
            moveTo(waitingRoomPos);
        /*
        else
            agent.Stop();
        */
    }

    public void Init(string nimi, string hetu)
    {
        this.nimi = nimi;
        this.hetu = hetu;
        print("Uusi potilas spawnattu!");
        print("Nimi: " + this.nimi);
        print("ID: " + this.hetu);
    }

    public void moveTo(Vector3 dest)
    {
        agent.destination = dest;
        if (transform.position == dest)
        {
            reachedDest = true;
            print(this.nimi + " perillä!");
        }
    }
}
