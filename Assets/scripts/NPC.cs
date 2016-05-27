using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NPC : MonoBehaviour
{
    /* states */
    public enum NPCState
    {
        STATE_ARRIVED,
        STATE_QUE,
        STATE_IDLE,
        STATE_DEAD,
        STATE_PAUSE //Pause everything and continue last task
    }

    /* basic stuff */
    public string myName;
    public string myId;
    public int myHp = 50;
    public int myHappiness = 50;
    public NPCState prevState;
    public NPCState myState;
    public Dictionary<int, Queue<NPCState>> stateQueue;
    private Quaternion currentRotation;
    //how far from destination player can be to start the task
    private float minDistanceToDestination = 10.0f;
    private bool taskCompleted = true;
    GameObject dialogZone;

    /* medicine stuff */
    bool gotMed;
    float deathTimer; // time without medicine
    float medTimer; // time with medicine
    float hpTimer;
    const int LOSE_HP_TIME = 2; // lose one hitpoint every X seconds 
    const int GET_HP_TIME = 2; // get one hitpoint every X seconds 
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
    const float IDE_IN_THIS_PLACE_TIME = 2f;
    const int WALK_RADIUS = 500;
    


    // Use this for initialization
    void Start()
    {
        stateQueue = new Dictionary<int, Queue<NPCState>>();
        agent = GetComponent<NavMeshAgent>();
        queManager = GameObject.Find("QueManager").GetComponent<QueManager>();
        dest = Vector3.zero;
        stateQueue.Add(1, new Queue<NPCState>());
        stateQueue.Add(2, new Queue<NPCState>());
        stateQueue.Add(3, new Queue<NPCState>());
        addStateToQueue(2, NPCState.STATE_ARRIVED);
    }

    // Update is called once per frame
    void Update()
    {
        if(dialogZone.GetComponent<Dialog>().playerInZone)
        {
            myState = NPCState.STATE_PAUSE;
        }

        if(taskCompleted)
            setMyStateFromQueue();

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
                if (arrivedToDestination())
                {
                    // chill for a while at reception and then move to doctor's queue
                    timer += Time.deltaTime;
                    if (timer > RECEPTION_WAITING_TIME)
                    {
                        addStateToQueue(2, NPCState.STATE_QUE);
                        timer = 0;
                        dest = Vector3.zero;
                        taskCompleted = true;
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
                if (arrivedToDestination()) //Mathf.Approximately(transform.position.x, dest.x) && Mathf.Approximately(transform.position.z, dest.z)
                {
                    // wait for a while at queue and then go idle
                    timer += Time.deltaTime;
                    if (timer > QUE_WAITING_TIME)
                    {
                        giveMed();
                        timer = 0;
                        dest = Vector3.zero;
                        taskCompleted = true;
                    }
                }
                break;

            case NPCState.STATE_IDLE:
                checkMed();
                //check if there's something else to do
                setMyStateFromQueue();

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
                    timer += Time.deltaTime;
                    if (timer > IDE_IN_THIS_PLACE_TIME)
                    {
                        timer = 0;
                        randomDirection = Random.insideUnitSphere * WALK_RADIUS;
                        randomDirection += transform.position;
                        NavMesh.SamplePosition(randomDirection, out hit, WALK_RADIUS, 1);
                        finalPosition = hit.position;
                        dest = new Vector3(finalPosition.x, 0, finalPosition.z);
                        moveTo(dest);
                    }
                    
                }
                break;
            case NPCState.STATE_DEAD:
                print(myName + " lähti teho-osastolle...");
                Destroy(gameObject);
                break;
            case NPCState.STATE_PAUSE:
                agent.Stop();
                RotateTowards(GameObject.FindGameObjectWithTag("Player").transform);
                //transform.LookAt();
                if (!dialogZone.GetComponent<Dialog>().playerInZone)
                {
                    myState = prevState;
                    agent.Resume();
                }
                break;
                
        }
    }

    private void RotateTowards(Transform target)
    {
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 10.0f);
    }

    private bool arrivedToDestination()
    {
        float dist = Vector3.Distance(dest, transform.position);
        if (dist < minDistanceToDestination)
            return true;
        else
            return false;
    }

    public void addStateToQueue(int priority, NPCState state)
    {
        Queue<NPCState> queue = new Queue<NPCState>();
        stateQueue.TryGetValue(priority, out queue);
        queue.Enqueue(state);
    }

    public void setMyStateFromQueue()
    {
        taskCompleted = false;
        Queue<NPCState> queue = new Queue<NPCState>();

        //Dequeue a task from priority 3 queue if it has a task and the current task is less important
        stateQueue.TryGetValue(3, out queue);
        if (queue.Count > 0)
        {
            prevState = myState;
            myState = queue.Dequeue();
        }
        else
        {
            //Dequeue a task from priority 2 queue if it has a task and the current task is less important
            stateQueue.TryGetValue(2, out queue);
            if(queue.Count > 0)
            {
                prevState = myState;
                myState = queue.Dequeue();
            }
            else
            {
                //Dequeue a task from priority 2 queue if it has a task and the current task is less important
                stateQueue.TryGetValue(1, out queue);
                if(queue.Count > 0)
                {
                    prevState = myState;
                    myState = queue.Dequeue();
                }
                else
                {
                    prevState = myState;
                    myState = NPCState.STATE_IDLE;
                }
            }
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
            hpTimer += Time.deltaTime;
            if (hpTimer >= GET_HP_TIME)
            {
                hpTimer = 0;
                myHp++;
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
                addStateToQueue(3, NPCState.STATE_DEAD);
            }
        }
    }

    public void giveMed()
    {
        // TODO: check if given medicine is correct
        print("medicine given!");
        gotMed = true;
    }
    public void initChild()
    {
        dialogZone = transform.FindChild("ContactZone").transform.gameObject;
    }
}
