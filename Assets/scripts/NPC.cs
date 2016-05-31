using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NPC : MonoBehaviour
{
    /* states */
    public enum NPCState
    {
        STATE_ARRIVED = 0,
        STATE_QUE,
        STATE_IDLE,
        STATE_DEAD,
        STATE_TALK_TO_OTHER_NPC, //Pause everything and continue last task
        STATE_TALK_TO_PLAYER,
        STATE_SLEEP,
        STATE_SLEEP_ON_FLOOR
    }

    /* basic stuff */
    public string myName;
    public string myId;
    public int myHp = 50;
    public int myHappiness = 50;
    //has the npc visited the doctor
    public bool diagnosed = false;
    //how tired the npc is
    public float fatique = 0;
    public float fatiquetimer = 0;
    public NPCState prevState;
    public NPCState myState;
    public Dictionary<int, Queue<NPCState>> stateQueue;
    public GameObject myBed;
    //how far from destination player can be to start the task
    private float minDistanceToDestination = 30.0f;
    private bool taskCompleted = true;
    GameObject dialogZone;
    GameObject target;
    public bool talking = false;
    private bool sleeping = false;
    private bool cantFindBed = false;
    public float oldy = 0.0f;

    /* medicine stuff */
    public bool gotMed;
    float deathTimer; // time without medicine
    float medTimer; // time with medicine
    float hpTimer;
    const int LOSE_HP_TIME = 10; // lose one hitpoint every X seconds 
    const int GET_HP_TIME = 10; // get one hitpoint every X seconds 
    const float MED_DURATION = 10;
    const string CORRECT_MED = "Burana";

    /* position stuff */
    Vector3 dest; // current destination position
    NavMeshAgent agent;
    QueManager queManager;
    NPCManager npcManager;
    Vector3 receptionPos = new Vector3(-155, 0, -23); // position of reception
    const float QUE_POS_Y = 130; // y-position of queue

    /* timing stuff */
    float timer; // time NPC has been in the current state
    const float RECEPTION_WAITING_TIME = 2f;
    const float QUE_WAITING_TIME = 10f;
    const float IDLE_IN_THIS_PLACE_TIME = 2f;
    const float MAX_TIME_TALK_TO_OTHER = 5f;
    const int WALK_RADIUS = 500;
    const float SLEEP_TIME = 10f;
    


    // Use this for initialization
    void Start()
    {
        stateQueue = new Dictionary<int, Queue<NPCState>>();
        agent = GetComponent<NavMeshAgent>();
        queManager = GameObject.Find("QueManager").GetComponent<QueManager>();
        npcManager = GameObject.Find("NPCManager").GetComponent<NPCManager>();
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
            myState = NPCState.STATE_TALK_TO_PLAYER;
        }

        //check status only if has visited the doctor
        if(diagnosed)
        {
            checkMed();

            //Increase fatigue every x seconds if state is not sleep
            if(!sleeping)
            {
                fatiquetimer += Time.deltaTime;
                if (fatiquetimer > 5.0f)
                {
                    fatique += 3f;
                    fatiquetimer = 0;
                }
                //if fatigue is too high, sleep immidiately
                if (fatique > 30.0f && cantFindBed)
                {
                    /*
                    taskCompleted = true;
                    addStateToQueue(3, NPCState.STATE_SLEEP_ON_FLOOR);
                    oldy = transform.rotation.y;
                    */
                    myHappiness -= 10;
                    fatique = 0;
                }
                if (fatique > 10.0f && !cantFindBed)
                {
                    addStateToQueue(2, NPCState.STATE_SLEEP);
                }
                
            }

        }
            
        if (taskCompleted)
            setMyStateFromQueue();

        switch (myState)
        {
            case NPCState.STATE_SLEEP_ON_FLOOR:

                transform.rotation = Quaternion.Euler(90, transform.eulerAngles.y, transform.eulerAngles.z);
                if (!sleeping)
                {
                    agent.updateRotation = false;
                    agent.Stop();
                    sleeping = true;
                }
                if (sleeping)
                {
                    timer += Time.deltaTime;
                    if(timer > 10.0f)
                    {
                        timer = 0;
                        addStateToQueue(2, NPCState.STATE_IDLE);
                        dest = Vector3.zero;
                        cantFindBed = false;
                        sleeping = false;
                        myHp -= 10;
                        fatique = 0;
                        taskCompleted = true;
                        agent.updateRotation = true;
                        agent.Resume();
                    }
                }

                break;
            case NPCState.STATE_SLEEP:
                if(myBed == null)
                {
                    myBed = npcManager.bookBed(gameObject);
                    if(myBed == null)
                    {
                        addStateToQueue(2, NPCState.STATE_IDLE);
                        cantFindBed = true;
                    }
                }
                if(dest == Vector3.zero && myBed != null)
                {
                    if(Mathf.Approximately(myBed.transform.rotation.y, 0.0f))
                        dest = new Vector3( myBed.transform.position.x, transform.position.y, myBed.transform.position.z + 24.0f );
                    else if (Mathf.Approximately(myBed.transform.rotation.y, 90.0f))
                        dest = new Vector3(myBed.transform.position.x - 16, transform.position.y, myBed.transform.position.z);
                    else if (myBed.transform.rotation.y == 180.0f)
                        dest = new Vector3(myBed.transform.position.x, transform.position.y, myBed.transform.position.z - 16);
                    else if (myBed.transform.rotation.y == 270.0f)
                        dest = new Vector3(myBed.transform.position.x + 16, transform.position.y, myBed.transform.position.z);
                    print(dest);
                    moveTo(dest);
                }
                if( myBed != null && arrivedToDestination(1.0f) && !sleeping)
                {
                    agent.Stop();
                    
                    GetComponent<IiroAnimBehavior>().goToSleep = true;
                    sleeping = true;
                    
                }
                if(sleeping)
                {
                    fatique = 0;
                    RotateAwayFrom(myBed.transform);
                    GetComponent<IiroAnimBehavior>().goToSleep = true;
                    timer += Time.deltaTime;
                    if(timer > SLEEP_TIME)
                    {
                        GetComponent<IiroAnimBehavior>().goToSleep = false;
                        sleeping = false;
                        taskCompleted = true;
                        dest = Vector3.zero;
                        agent.Resume();
                    }
                }
                break;
            case NPCState.STATE_ARRIVED:
                // move to reception when NPC first arrives
                if (dest == Vector3.zero)
                {
                    dest = receptionPos;
                    moveTo(dest);
                }
                
                // NPC has arrived at reception
                if (arrivedToDestination(30))
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
                if (arrivedToDestination(30)) 
                {
                    // wait for a while at queue and then go idle
                    timer += Time.deltaTime;
                    if (timer > QUE_WAITING_TIME)
                    {
                        diagnosed = true;
                        giveMed(CORRECT_MED);
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
                timer += Time.deltaTime;
                if (target != null && talking)
                {
                    agent.Stop();
                    RotateTowards(target.transform);
                }
                else 
                {
                    if (arrivedToDestination(30))
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
                    else if (timer > IDLE_IN_THIS_PLACE_TIME)
                    {
                        timer = 0;
                        Vector3 randomDirection = Random.insideUnitSphere * WALK_RADIUS;
                        randomDirection += transform.position;
                        NavMeshHit hit;
                        NavMesh.SamplePosition(randomDirection, out hit, WALK_RADIUS, 1);
                        Vector3 finalPosition = hit.position;
                        dest = new Vector3(finalPosition.x, 0, finalPosition.z);
                        moveTo(dest);
                        if (Random.Range(0f, 1f) > 0.9f)
                        {
                            if (!talking)
                                addStateToQueue(2, NPCState.STATE_TALK_TO_OTHER_NPC);
                        }
                    }

                }
                break;
            case NPCState.STATE_DEAD:
                print(myName + " lähti teho-osastolle...");
                Destroy(gameObject);
                break;            
            case NPCState.STATE_TALK_TO_PLAYER:
                checkMed();
                agent.Stop();
                RotateTowards(GameObject.FindGameObjectWithTag("Player").transform);
                if (!dialogZone.GetComponent<Dialog>().playerInZone)
                {
                    myState = prevState;
                    agent.Resume();
                }
                break;
            case NPCState.STATE_TALK_TO_OTHER_NPC:
                checkMed();
                //check that the target is actually capable of talking
                if (target == null || target.tag != "NPC" || !target.GetComponent<NPC>().isIdle() && !target.GetComponent<NPC>().talking)
                {
                    findOtherIdleNPC();
                }
                    
                if(target == null)
                {
                    addStateToQueue(2, NPCState.STATE_IDLE);
                }
                //check if at target & set destination
                if(walkToTarget())
                {
                    //set both npc's to talking
                    target.GetComponent<NPC>().talking = true;
                    talking = true;

                    //stop moving
                    agent.Stop();

                    //rotate to look the target
                    RotateTowards(target.transform);
                    timer += Time.deltaTime;
                    if(timer > MAX_TIME_TALK_TO_OTHER)
                    {
                        timer = 0;
                        taskCompleted = true;
                        talking = false;
                        target.GetComponent<NPC>().talking = false;
                        target.GetComponent<NPC>().agent.Resume();
                        agent.Resume();

                    }
                }

                break;


        }
    }

    private void resetStateVariables()
    {
        talking = false;
        dest = Vector3.zero;
        agent.Resume();
    }

    public void setTarget(GameObject target)
    {
        this.target = target;
    }

    public GameObject getTarget()
    {
        return target;
    }

    //returns true if npc is already at target, sets the agent destination
    private bool walkToTarget()
    {

        if(Vector3.Distance(transform.position, target.transform.position) < 30.0f)
        {
            return true;
        }
        else
        {
            agent.SetDestination(target.transform.position);
            return false;
        }
    }
    private void findOtherIdleNPC()
    {
        GameObject[] npcs = GameObject.FindGameObjectsWithTag("NPC");
        
        foreach(GameObject npc in npcs)
        {
            //Check that the npc is not self and is idle
            if(npc.gameObject != gameObject && npc.GetComponent<NPC>().isIdle())
            {
                target = npc.transform.gameObject;
            }
        }
        //target is still null if there is no other idle npcs, so go back to idle
        if(target == null || !target.GetComponent<NPC>().isIdle())
        {
            myState = NPCState.STATE_IDLE;
        }
    }

    public bool isIdle()
    {
        if (myState == NPCState.STATE_IDLE || myState == NPCState.STATE_TALK_TO_OTHER_NPC )
        {
            return true;
        }
        else return false;
    }

    private void RotateTowards(Transform target)
    {
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 10.0f);
    }

    private void RotateAwayFrom(Transform target)
    {
        
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(-direction);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, 5.0f);     
    }

    private bool arrivedToDestination(float accuracy)
    {
        float dist = Vector3.Distance(dest, transform.position);
        if (dist < accuracy)
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
            dest = Vector3.zero;
        }
        else
        {
            //Dequeue a task from priority 2 queue if it has a task and the current task is less important
            stateQueue.TryGetValue(2, out queue);
            if(queue.Count > 0)
            {
                prevState = myState;
                myState = queue.Dequeue();
                dest = Vector3.zero;
            }
            else
            {
                //Dequeue a task from priority 2 queue if it has a task and the current task is less important
                stateQueue.TryGetValue(1, out queue);
                if(queue.Count > 0)
                {
                    prevState = myState;
                    myState = queue.Dequeue();
                    dest = Vector3.zero;
                }
                else
                {
                    prevState = myState;
                    myState = NPCState.STATE_IDLE;
                    dest = Vector3.zero;
                }
            }
        }
    }

    public void Init(string myName, string myId)
    {
        
        this.myName = myName;
        this.myId = myId;

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
                myHp--;
                deathTimer = 0;
            }
            if (myHp <= 0)
            {
                addStateToQueue(3, NPCState.STATE_DEAD);
            }
        }
    }

    public bool giveMed(string med)
    {
        if (diagnosed) //TODO: check if player is near enough the NPC to give medicine
        { 
            if (med == CORRECT_MED)
            {
                gotMed = true;
                medTimer = 0;
                print("Correct medicine!");
                return true;
            }           
            else
            {
                gotMed = false;
                deathTimer = 0;
                myHp = myHp - 20;
                print("Wrong medicine! " + myName + " lost 20HP!");
                return true;
            }
        }
        return false;
    }

    public void initChild()
    {
        dialogZone = transform.FindChild("ContactZone").transform.gameObject;
    }

}
