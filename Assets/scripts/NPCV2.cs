using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NPCV2 : MonoBehaviour
{
    /* states */
    public enum NPCState
    {
        STATE_ARRIVED = 0,
        STATE_QUE,
        STATE_IDLE,
        STATE_DEAD,
        STATE_TALK_TO_OTHER_NPC, 
        STATE_TALK_TO_PLAYER,
        STATE_SLEEP,
        STATE_SLEEP_ON_FLOOR
    }

    /* basic stuff */
    public string myName;
    public string myId;
    public int myHp = 50;
    public int myHappiness = 50;
    public Vector3 bedloc;

    /* Reference to player */
    private GameObject player;
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
    ObjectInteraction interactionComponent;
    ObjectManager objectManager;
    public bool talking = false;
    public bool sleeping = false;
    private bool sitting = false;
    public bool cantFindBed = false;
    public float oldy = 0.0f;

    private bool sleepingqueued = false;

    /* medicine stuff */
    public bool gotMed;
    float deathTimer; // time without medicine
    float medTimer; // time with medicine
    float hpTimer;
    const int LOSE_HP_TIME = 2; // lose one hitpoint every X seconds 
    const int GET_HP_TIME = 2; // get one hitpoint every X seconds 
    const float MED_DURATION = 30f;
    public string myProblem;
    public string myMedicine;

    /* position stuff */
    Vector3 dest; // current destination position
    NavMeshAgent agent;
    QueManagerV2 queManager;
    NPCManagerV2 npcManager;
    Vector3 receptionPos = new Vector3(49, 0, 124); // position of reception
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
        player = GameObject.FindGameObjectWithTag("Player");
        bedloc = Vector3.zero;
        stateQueue = new Dictionary<int, Queue<NPCState>>();
        agent = GetComponent<NavMeshAgent>();
        queManager = GameObject.Find("QueManager").GetComponent<QueManagerV2>();
        npcManager = GameObject.Find("NPCManager").GetComponent<NPCManagerV2>();
        dest = Vector3.zero;
        stateQueue.Add(1, new Queue<NPCState>());
        stateQueue.Add(2, new Queue<NPCState>());
        stateQueue.Add(3, new Queue<NPCState>());
        addStateToQueue(2, NPCState.STATE_ARRIVED);
        interactionComponent = GetComponent<ObjectInteraction>();
        objectManager = GameObject.FindGameObjectWithTag("ObjectManager").GetComponent<ObjectManager>();
    }
    // Update is called once per frame
    void Update()
    {
        if (dialogZone.GetComponent<DialogV2>().playerInZone && !sleeping && !sitting)
        {
            myState = NPCState.STATE_TALK_TO_PLAYER;
        }

        //check status only if has visited the doctor
        if (diagnosed)
        {
            //check medication every update if player is diagnosed
            checkMed();
            //Increase fatigue every x seconds if state is not sleep
            if (!sleepingqueued)
            {
                fatiquetimer += Time.deltaTime;
                if (fatiquetimer > 5.0f)
                {
                    fatique += 3f;
                    fatiquetimer = 0;
                }
                //if fatigue is too high and can't find bed, lose happiness, reset fatique
                if (fatique > 30.0f && cantFindBed)
                {
                    myHappiness -= 10;
                    fatique = 0;
                }
                //if has bed and fatique over x, queue sleep task
                if (fatique > 10.0f && !cantFindBed)
                {
                    addStateToQueue(2, NPCState.STATE_SLEEP);
                    sleepingqueued = true;
                }

            }

        }

        if (taskCompleted)
            setMyStateFromQueue();
        actAccordingToState();

    }
    private void actAccordingToState()
    {
        /* act according to myState */
        switch (myState)
        {
            case NPCState.STATE_SLEEP_ON_FLOOR:
                sleepOnFloor();
                break;
            case NPCState.STATE_SLEEP:
                sleep();
                break;
            case NPCState.STATE_ARRIVED:
                arrival();
                break;
            case NPCState.STATE_QUE:
                queue();
                break;
            case NPCState.STATE_IDLE:
                idle();
                break;
            case NPCState.STATE_DEAD:
                die();
                break;
            case NPCState.STATE_TALK_TO_PLAYER:
                talkToPlayer();
                break;
            case NPCState.STATE_TALK_TO_OTHER_NPC:
                talkToNPC();
                break;
        }
    }
    private void sleepOnFloor()
    {
        //slam npc face to floor
        transform.rotation = Quaternion.Euler(90, transform.eulerAngles.y, transform.eulerAngles.z);
        if (!sleeping)
        {
            //stop the navmesh agent movement
            agent.updateRotation = false;
            agent.Stop();
            sleeping = true;
        }
        if (sleeping)
        {

            timer += Time.deltaTime;
            if (timer > 10.0f)
            {
                timer = 0;
                //go back to idle after sleeping, reset all values, lose health
                addStateToQueue(2, NPCState.STATE_IDLE);
                dest = Vector3.zero;
                cantFindBed = false;
                sleeping = false;
                myHp -= 10;
                fatique = 0;
                taskCompleted = true;

                //navmesh agent resume
                agent.updateRotation = true;
                agent.Resume();
            }
        }
    }
    private void sleep()
    {
        //if npc doesn't have bed, try to find one
        if (myBed == null)
        {
            //check if bed is available
            myBed = objectManager.bookBed(gameObject);
            //if still null, bed not found
            if (myBed == null)
            {
                //back to idle
                addStateToQueue(2, NPCState.STATE_IDLE);

                //mark that this npc doesn't have bed, so next loop will not go to sleep(), 
                //but stay in idle until 
                //fatique is too high
                cantFindBed = true;
            }
        }

        //if has no destination and has a bed
        if (dest == Vector3.zero && myBed != null)
        {
            interactionComponent.setTarget(myBed);
            dest = interactionComponent.getDestToTargetObjectSide(1, 20.0f);
            if(dest == Vector3.zero)
            {
                cantFindBed = true;
                taskCompleted = true;
            }
            else
            {
                moveTo(dest);
            }
            
        }
        //if at the bed and not sleeping yet, stop navmeshagent and start animation
        if (myBed != null && arrivedToDestination(10.0f) && !sleeping)
        {
            agent.Stop();
            //rotate until looking away from mybed
            if (RotateAwayFrom(myBed.transform))
            {
                sleeping = true;
            }

        }

        if (sleeping)
        {
            GetComponent<IiroAnimBehavior>().goToSleep = true;
            timer += Time.deltaTime;
            if (timer > SLEEP_TIME)
            {
                //stop animation
                GetComponent<IiroAnimBehavior>().goToSleep = false;
                sleeping = false;
                taskCompleted = true;
                timer = 0;
                fatique = 0;
                //resume agent movement
                agent.Resume();
                sleepingqueued = false;
            }
        }
    }

    //arrives at hospital
    private void arrival()
    {
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
                taskCompleted = true;
            }
        }
    }
    //queue to doctor
    private void queue()
    {
        if (dest == Vector3.zero)
        {
            interactionComponent.setTarget(objectManager.bookRandomQueueChair(gameObject));
            interactionComponent.setCurrentChair(interactionComponent.getTarget());
            // set destination to queue chair
            dest = interactionComponent.getDestToTargetObjectSide(0, 20.0f);
            // move to the queue position received
            moveTo(dest);
        }

        if (arrivedToDestination(10.0f) && !sitting)
        {
            agent.Stop();
            sitting = true;
        }

        if (sitting)
        {
            //rotate to look away from the bed so animation will move the player on the bed
            if (RotateAwayFrom(interactionComponent.getTarget().transform))
            {
                agent.GetComponent<IiroAnimBehavior>().sit = true;
                timer += Time.deltaTime;
                if (timer > QUE_WAITING_TIME)
                {
                    GetComponent<IiroAnimBehavior>().sit = false;
                    diagnosed = true;
                    timer = 0;
                    taskCompleted = true;
                    objectManager.unbookObject(interactionComponent.getCurrentChair());
                    sitting = false;
                    agent.Resume();
                }
            }

        }
    }
    //just wander around the hospital, if npc has nothing else to do it will go to this state
    private void idle()
    {
        //check if there's something else to do
        setMyStateFromQueue();
        timer += Time.deltaTime;
        GameObject target = interactionComponent.getTarget();
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
    }
    private void die()
    {
        print(myName + " lähti teho-osastolle...");
        Destroy(gameObject);
    }
    //if player is close and player has target on this npc, talk to player
    private void talkToPlayer()
    {
        agent.Stop();
        RotateTowards(player.transform);
        if (!dialogZone.GetComponent<DialogV2>().playerInZone)
        {
            myState = prevState;
            agent.Resume();
        }
    }
    private void talkToNPC()
    {
        GameObject target = interactionComponent.getTarget();
        //check that the target is actually capable of talking
        if (target == null || target.tag != "NPC" || !target.GetComponent<NPCV2>().isIdle() || target.GetComponent<NPCV2>().talking)
        {
            findOtherIdleNPC();
        }

        //check if at target & set destination
        if  (target.tag == "NPC" && target != null && walkToTarget())
        {
            //set both npc's to talking
            target.GetComponent<NPCV2>().talking = true;
            talking = true;

            //stop moving
            agent.Stop();
            target.GetComponent<NPCV2>().agent.Stop();

            //rotate to look the target
            RotateTowards(target.transform);
            timer += Time.deltaTime;
            if (timer > MAX_TIME_TALK_TO_OTHER)
            {
                timer = 0;
                target.GetComponent<NPCV2>().stopTalking();
                stopTalking();

            }
        }
        if(target == null || target.tag != "NPC")
        {
            stopTalking();
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
        interactionComponent.setTarget(target);
    }
    public GameObject getTarget()
    {
        return interactionComponent.getTarget();
    }
    //returns true if npc is already at target, sets the agent destination
    private bool walkToTarget()
    {
        GameObject target = interactionComponent.getTarget();
        if (Vector3.Distance(transform.position, target.transform.position) < 30.0f)
        {
            return true;
        }
        else
        {
            moveTo(target.transform.position);
            return false;
        }
    }
    //looks for other npcs who are idle
    private void findOtherIdleNPC()
    {
        GameObject[] npcs = GameObject.FindGameObjectsWithTag("NPC");
        List<GameObject> idlenpcs = new List<GameObject>();
        foreach (GameObject npc in npcs)
        {
            //Check that the npc is not self and is idle
            if (npc.gameObject != gameObject && npc.GetComponent<NPCV2>().isIdle() && !npc.GetComponent<NPCV2>().talking)
            {
                idlenpcs.Add(npc.gameObject);
            }
        }
        if(idlenpcs.Count > 0)
        {
            interactionComponent.setTarget(idlenpcs[0]);
        }
        else
        {
            stopTalking();
        }
    }
    public void stopTalking()
    {
        agent.Resume();
        talking = false;
        taskCompleted = true;
    }
    public bool isIdle()
    {
        if (myState == NPCState.STATE_IDLE || myState == NPCState.STATE_TALK_TO_OTHER_NPC)
        {
            return true;
        }
        else return false;
    }
    //rotates towards a position
    private void RotateTowards(Transform target)
    {
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 10.0f);
    }
    //same as rotateTowards, but inverse look direction
    private bool RotateAwayFrom(Transform target)
    {
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(-direction);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, 5.0f);
        float transformy = Mathf.Abs(transform.rotation.eulerAngles.y);
        float looky = Mathf.Abs(lookRotation.eulerAngles.y);
        if (approx(transformy, looky, 0.1f))
        {
            return true;
        }
        return false;
    }
    //checks if navmesh is within accuracy zone of his destination
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
    //finds the highest priority task and selects it as current stage
    //called only when task is completed
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
            if (queue.Count > 0)
            {
                prevState = myState;
                myState = queue.Dequeue();
                dest = Vector3.zero;
            }
            else
            {
                //Dequeue a task from priority 2 queue if it has a task and the current task is less important
                stateQueue.TryGetValue(1, out queue);
                if (queue.Count > 0)
                {
                    prevState = myState;
                    myState = queue.Dequeue();
                    dest = Vector3.zero;
                }
                else
                {
                    if(myState != NPCState.STATE_IDLE)
                    {
                        prevState = myState;
                        myState = NPCState.STATE_IDLE;
                        dest = Vector3.zero;
                    }
                }
            }
        }
    }
    public void Init(string myName, string myId, string myProblem, string myMedicine)
    {
        this.myName = myName;
        this.myId = myId;
        this.myProblem = myProblem;
        this.myMedicine = myMedicine;
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
               // myHp--;
                deathTimer = 0;
            }
            if (myHp <= 0)
            {
               // addStateToQueue(3, NPCState.STATE_DEAD);
                taskCompleted = true;
            }
        }
    }
    public bool giveMed(string med)
    {
        if (diagnosed && dialogZone.GetComponent<DialogV2>().playerInZone)
        {
            if (string.Equals(med, myMedicine, System.StringComparison.CurrentCultureIgnoreCase))
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
    private bool approx(float a, float b, float accuracy)
    {
        float sub = a - b;
        if(Mathf.Abs(sub) < accuracy)
        {
            return true;
        }
        return false;

    }
}
