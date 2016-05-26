using UnityEngine;
using System.Collections;

public class NPC : MonoBehaviour
{
    /* states */
    public enum NPCState
    {
        STATE_ARRIVED,
        STATE_QUE,
        STATE_IDLE,
        STATE_DEAD
    }

    /* basic stuff */
    public string myName;
    public string myId;
    public int myHp = 50;
    public int myHappiness = 50;
    public NPCState myState;

    /* medicine stuff */
    bool gotMed;
    float deathTimer; // time without medicine
    float medTimer; // time with medicine
    const int LOSE_HP_TIME = 2; // lose one hitpoint every X seconds 
    const float MED_DURATION = 10;

    /* position stuff */
    Vector3 dest; // current destination position
    NavMeshAgent agent;
    QueManager queManager;
    Vector3 receptionPos = new Vector3(-155, 0, -23); // position of reception
    const float QUE_POS_Y = 130; // y-position of queue

    /* timing stuff */
    float timer; // time NPC has been in the current state
    const float RECEPTION_WAITING_TIME = 2f;
    const float QUE_WAITING_TIME = 10f;
    const int WALK_RADIUS = 500;

    // Use this for initialization
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        queManager = GameObject.Find("QueManager").GetComponent<QueManager>();
        myState = NPCState.STATE_ARRIVED;
        dest = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        switch (myState)
        {
            case NPCState.STATE_ARRIVED:
                // move to reception when NPC first arrives
                if (dest == Vector3.zero)
                {
                    dest = receptionPos;
                    moveTo(dest);
                }
                
                // NPC has arrived at reception
                if (Mathf.Approximately(transform.position.x, dest.x) && Mathf.Approximately(transform.position.z, dest.z))
                {
                    // chill for a while at reception and then move to doctor's queue
                    timer += Time.deltaTime;
                    if (timer > RECEPTION_WAITING_TIME)
                    { 
                        myState = NPCState.STATE_QUE;
                        timer = 0;
                        dest = Vector3.zero;
                    }
                }
                break;

            case NPCState.STATE_QUE:
                if (dest == Vector3.zero)
                {
                    // get next free queue position from queManager
                    float quePosX = queManager.addToQue();
                    dest = new Vector3(quePosX, 0, QUE_POS_Y);
                    // move to the queue position received
                    moveTo(dest);
                }

                // NPC has arrived at the queue position
                if (Mathf.Approximately(transform.position.x, dest.x) && Mathf.Approximately(transform.position.z, dest.z))
                {
                    // wait for a while at queue and then go idle
                    timer += Time.deltaTime;
                    if (timer > QUE_WAITING_TIME)
                    {
                        myState = NPCState.STATE_IDLE;
                        giveMed();
                        timer = 0;
                        dest = Vector3.zero;
                    }
                }
                break;

            case NPCState.STATE_IDLE:
                checkMed();
                if (dest == Vector3.zero)
                {
                    // move to idle at random position
                    Vector3 randomDirection = Random.insideUnitSphere * WALK_RADIUS;
                    randomDirection += transform.position;
                    NavMeshHit hit;
                    NavMesh.SamplePosition(randomDirection, out hit, WALK_RADIUS, 1);
                    Vector3 finalPosition = hit.position;
                    dest = new Vector3(finalPosition.x, 0, finalPosition.z);
                    moveTo(dest);
                    
                }
                break;
            case NPCState.STATE_DEAD:
                print(myName + " lähti teho-osastolle...");
                Destroy(gameObject);
                break;
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

    void checkMed()
    {
        if (gotMed)
        {
            medTimer += Time.deltaTime;
            if (medTimer >= MED_DURATION)
            {
                medTimer = 0;
                gotMed = false;
                print("medicine duration over!");
            }
        }
        else
        {
            deathTimer += Time.deltaTime;
            if (deathTimer >= LOSE_HP_TIME)
            {
                print("lost hp, give me new medicine!");
                myHp--;
                deathTimer = 0;
            }
            if (myHp <= 0)
            {
                myState = NPCState.STATE_DEAD;
            }
        }
    }

    public void giveMed()
    {
        // TODO: check if given medicine is correct
        print("medicine kicked in!");
        gotMed = true;
    }
}
