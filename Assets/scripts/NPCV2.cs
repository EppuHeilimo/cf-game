using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

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
        STATE_SLEEP_ON_FLOOR,
        STATE_GO_TO_DOC,
        STATE_LEAVE_HOSPITAL,
        STATE_MOVE_TO_WARD_AREA,
        STATE_TRY_UNSTUCK,
        STATE_GO_WC,
        STATE_IDLE_SIT
    }

    /* basic stuff */
    ClockTime clock;
    LineRenderer DebugPath;
    public string myName;
    public string myId;
    public int myHp = 50;
    public int myHappiness = 50;
    int currentTaskPriority = 0;
    public Sprite myHead2d;
    int prevTaskPriority = 0;
    /* Reference to player */
    private GameObject player;
    public GameObject responsibilityIndicator;
    private GameObject responsibilityIndicatorclone;
    //has the npc visited the doctor
    public bool diagnosed = false;
    //how tired the npc is
    public float fatique = 0;
    //float to indicate the need to go pee
    public float callofnature = 0;
    public float callofnaturetimer = 0;
    public float fatiquetimer = 0;
    public NPCState prevState;
    public NPCState myState;
    public Dictionary<int, Queue<NPCState>> stateQueue;
    public GameObject myBed;
    //how far from destination player can be to start the task
    private bool taskCompleted = true;
    GameObject dialogZone;
    ObjectInteraction interactionComponent;
    ObjectManager objectManager;
    public bool talking = false;
    public bool sleeping = false;
    private bool sitting = false;
    public bool cantFindBed = false;
    private bool wcqueued = false;
    bool prevStateUncompleted = false;
    bool playersResponsibility = false;
    private bool sleepingqueued = false;

    /* NEW MEDICINE SYSTEM */
    float deathTimer; // time without medicine
    const int LOSE_HP_TIME = 2; // lose one hitpoint every X seconds if there is no medicine active

    public struct Medicine
    {
        public string title;
        public bool isActive;
    };

    public Item[] myMedication = new Item[4]; // all of this NPC's meds, usages etc.
    public string[] myProblems;
    /* specific meds for different times of day */
    public Medicine morningMed;
    public Medicine afternoonMed;
    public Medicine eveningMed;
    public Medicine nightMed;
    /* dosages */
    public int morningDos;
    public int afternoonDos;
    public int eveningDos;
    public int nightDos;
    /* if correct med is not active at the correct time of the day, start losing hp */
    public bool isLosingHp = false;

    /* position stuff */
    public Vector3 dest; // current destination position
    NavMeshAgent agent;
    NPCManagerV2 npcManager;
    Vector3 receptionPos = new Vector3(49, 0, 124); // position of reception

    /* timing stuff */
    float timer; // time NPC has been in the current state
    const float RECEPTION_WAITING_TIME = 3f;
    const float QUE_WAITING_TIME = 10f;
    const float IDLE_IN_THIS_PLACE_TIME = 5f;
    const float MAX_TIME_TALK_TO_OTHER = 10f;
    const int WALK_RADIUS = 500;
    const float SLEEP_TIME = 10f;
    const float AT_DOC = 5f;
    const float IN_WC = 10f;
    const float STAY_ON_FLOOR_ON_FALL = 10.0f;

    //stuck testing
    const float STUCK = 5f;
    //how long doctor will wait for patient
    const float DOC_WAIT_TIME = 10.0f;
    float doctimer = 0;
    float stucktimer = 0f;
    //current distance to destination
    float currentdisttodest;
    //distance to destination at last test
    float lastdisttodest;

    
    // Use this for initialization
    void Start()
    {
        DebugPath = GetComponent<LineRenderer>(); //get the line renderer
        player = GameObject.FindGameObjectWithTag("Player");
        stateQueue = new Dictionary<int, Queue<NPCState>>();
        agent = GetComponent<NavMeshAgent>();
        //Set npc speed randomly
        agent.speed = Random.Range(60f, 100f);
        //Set animation speed to match the walk speed.
        GetComponent<IiroAnimBehavior>().setWalkAnimSpeed(agent.speed);
        npcManager = GameObject.Find("NPCManager").GetComponent<NPCManagerV2>();
        dest = Vector3.zero;
        stateQueue.Add(1, new Queue<NPCState>());
        stateQueue.Add(2, new Queue<NPCState>());
        stateQueue.Add(3, new Queue<NPCState>());
        addStateToQueue(2, NPCState.STATE_ARRIVED);
        interactionComponent = GetComponent<ObjectInteraction>();
        lastdisttodest = 0;
        objectManager = GameObject.FindGameObjectWithTag("ObjectManager").GetComponent<ObjectManager>();
        clock = GameObject.FindGameObjectWithTag("Clock").GetComponent<ClockTime>();
    }
    // Update is called once per frame
    void Update()
    {
        if(playersResponsibility)
        {
            responsibilityIndicatorclone.transform.position = new Vector3(transform.position.x, transform.position.y + 64, transform.position.z);
            responsibilityIndicatorclone.transform.rotation = transform.rotation;
        }
        //check if there are some natural needs, or unstucking needs
        checkNeeds();
        //Set current state to highest priority currently queued
        //if higher priority job compared to current state is found, current state will be paused
        setMyStateFromQueue();
        //Act according to the myState (Current state)
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
            case NPCState.STATE_GO_TO_DOC:
                goToDoc();
                break;
            case NPCState.STATE_LEAVE_HOSPITAL:
                leaveHospital();
                break;
            case NPCState.STATE_MOVE_TO_WARD_AREA:
                goToWardArea();
                break;
            case NPCState.STATE_TRY_UNSTUCK:
                tryUnstuck();
                break;
            case NPCState.STATE_GO_WC:
                goWC();
                break;
            case NPCState.STATE_IDLE_SIT:
                idleSit();
                break;
        }
    }

    void checkNeeds()
    {
        //stuck test
        stucktimer += Time.deltaTime;
        //test every STUCK seconds if npc hasn't moved towards it's dest
        if (stucktimer > STUCK && dest != Vector3.zero && !arrivedToDestination(10.0f) && myState != NPCState.STATE_TRY_UNSTUCK)
        {
            stucktimer = 0;
            currentdisttodest = Vector3.Distance(transform.position, dest);
            if (lastdisttodest == 0)
            {
                lastdisttodest = currentdisttodest;
            }
            else
            {
                float sub = currentdisttodest - lastdisttodest;
                if (Mathf.Abs(sub) < 40.0f)
                {
                    lastdisttodest = 0;
                    addStateToQueue(3, NPCState.STATE_TRY_UNSTUCK);
                    taskCompleted = true;
                }
            }

        }

        if (dialogZone.GetComponent<DialogV2>().playerInZone && !sleeping && !sitting)
        {
            myState = NPCState.STATE_TALK_TO_PLAYER;
        }

        //check status only if has visited the doctor
        if (diagnosed)
        {
            //check medication every update if player is diagnosed
            checkMed();
            if (!sleepingqueued && clock.currentDayTime == ClockTime.DayTime.NIGHT)
            {
                addStateToQueue(2, NPCState.STATE_SLEEP);
                sleepingqueued = true;
            }
            //Increase fatigue every x seconds if state is not sleep
            //if sleeping already queued, just skip
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
            //Don't test need to go to wc if already queued
            if (!wcqueued)
            {
                callofnaturetimer += Time.deltaTime;
                if (callofnaturetimer > 5.0f)
                {
                    callofnature += 1;
                }
                if (callofnature > 10.0f)
                {
                    addStateToQueue(2, NPCState.STATE_GO_WC);
                    wcqueued = true;
                }
            }

        }
    }

    void idleSit()
    {
        if (dest == Vector3.zero)
        {
            GameObject targetChair = objectManager.bookRandomChair(gameObject);
            if (targetChair == null)
            {
                taskCompleted = true;
            }
            else
            {
                interactionComponent.setTarget(targetChair);
                interactionComponent.setCurrentChair(interactionComponent.getTarget());
                // set destination to queue chair
                if(targetChair.tag == "Chair2")
                {
                    dest = interactionComponent.getDestToTargetObjectSide(1, 20.0f);
                }
                else
                {
                    dest = interactionComponent.getDestToTargetObjectSide(0, 20.0f);
                }
                
                // move to the queue position received
                moveTo(dest);
            }

        }

        if (arrivedToDestination(10.0f) && !sitting)
        {
            agent.Stop();
            sitting = true;
        }

        if (sitting)
        {
            if (arrivedToDestination(10.0f))
            {
                if (interactionComponent.getTarget())
                {
                    //rotate to look away from the bed so animation will move the player on the bed
                    if (interactionComponent.RotateAwayFrom(interactionComponent.getTarget().transform))
                    {
                        if (interactionComponent.getTarget().tag == "Chair2")
                        {
                            agent.GetComponent<IiroAnimBehavior>().sitwithrotation = true;
                        }
                        else
                        {
                            agent.GetComponent<IiroAnimBehavior>().sit = true;
                        }
                            
                    }
                }

            }
            timer += Time.deltaTime;
            if(timer > 2 * IDLE_IN_THIS_PLACE_TIME)
            {
                if(interactionComponent.getCurrentChair().tag == "Chair2")
                {
                    GetComponent<IiroAnimBehavior>().sitwithrotation = false;
                }
                else
                {
                    GetComponent<IiroAnimBehavior>().sit = false;
                }
                
                taskCompleted = true;
                if ((interactionComponent.getCurrentChair() != null))
                    objectManager.unbookObject(interactionComponent.getCurrentChair());
                sitting = false;
                agent.Resume();
            }
        }
    }
    void goWC()
    {
        if (dest == Vector3.zero)
        {
            GameObject targetToilet = objectManager.bookRandomPublicToilet(gameObject);
            if (targetToilet == null)
            {
                taskCompleted = true;
                wcqueued = false;
                callofnature = 0;
            }
            else
            {
                interactionComponent.setTarget(targetToilet);
                interactionComponent.setCurrentToilet(interactionComponent.getTarget());
                // set destination to queue chair
                dest = interactionComponent.getDestToTargetObjectSide(2, 20.0f);
                // move to the queue position received
                moveTo(dest);
            }
         }
        if (arrivedToDestination(10.0f) && !sitting)
        {
            agent.Stop();
            sitting = true;
        }

        if (sitting)
        {
            if (arrivedToDestination(20.0f))
            {
                if (interactionComponent.getTarget())
                {
                    //rotate to look away from the bed so animation will move the player on the bed
                    if (interactionComponent.RotateAwayFrom(interactionComponent.getTarget().transform))
                    {
                        agent.GetComponent<IiroAnimBehavior>().sit = true;
                        timer += Time.deltaTime;
                        if (timer > IN_WC)
                        {
                            GetComponent<IiroAnimBehavior>().sit = false;
                            taskCompleted = true;
                            if ((interactionComponent.getCurrentChair() != null))
                            {
                                objectManager.unbookObject(interactionComponent.getCurrentChair());
                            }  
                            sitting = false;
                            agent.Resume();
                            wcqueued = false;
                            callofnature = 0;
                        }
                    }
                }
            }
        }
    }

    bool isDestInvalid()
    {
        if(agent.pathStatus == NavMeshPathStatus.PathInvalid)
        {
            return true;
        }
        if(dest.x == Mathf.Infinity || dest.z == Mathf.Infinity)
        {
            return true;
        }
        return false;
    }

    private void tryUnstuck()
    {
        if (dest == Vector3.zero || isDestInvalid())
        {
            Vector3 randomDirection = Random.insideUnitSphere * 80.0f;
            randomDirection += transform.position;
            NavMeshHit hit;
            
            NavMesh.SamplePosition(randomDirection, out hit, 80.0f, 1);
            Vector3 finalPosition = hit.position;
            dest = new Vector3(finalPosition.x, transform.position.y, finalPosition.z);
            moveTo(dest);
        }
        if(arrivedToDestination(30.0f))
        {
            taskCompleted = true;
            addStateToQueue(2, prevState);
        }
    }

    private void goToWardArea()
    {
        if(dest == Vector3.zero)
        {
            dest = new Vector3(637.0f, transform.position.y, -76.0f);
            moveTo(dest);
        }
        if(arrivedToDestination(100.0f))
        {
            taskCompleted = true;
        }
    }

    private void leaveHospital()
    {
        if(dest == Vector3.zero)
        {
            dest = new Vector3(-620, transform.position.y, 0);
            moveTo(dest); 
        }
        if(arrivedToDestination(30))
        {
            npcManager.deleteNpcFromList(gameObject);
            Destroy(gameObject);
        }
    }

    private void goToDoc()
    {
        doctimer += Time.deltaTime;
        if (dest == Vector3.zero)
        {
            dest = new Vector3(-50.0f, transform.position.y, 393.0f);
            moveTo(dest);
            timer = 0;
            doctimer = 0;
        }
        if(arrivedToDestination(30.0f))
        {
            doctimer = 0;
            timer += Time.deltaTime;
            if(timer > AT_DOC)
            {

                int r = Random.Range(1, 10);
                if(r > 1)
                {
                    addStateToQueue(2, NPCState.STATE_MOVE_TO_WARD_AREA);
                    diagnosed = true;
                    if(!npcManager.isPlayerResponsibilityLevelFulfilled())
                    {
                        npcManager.addNpcToPlayersResponsibilities(gameObject);
                        playersResponsibility = true;
                        responsibilityIndicatorclone = (GameObject)Instantiate(responsibilityIndicator, transform.position, new Quaternion(0, 0, 0, 0));
                    }
                }
                else
                {
                    addStateToQueue(3, NPCState.STATE_LEAVE_HOSPITAL);
                }
                timer = 0;
                taskCompleted = true;
                dest = Vector3.zero;
                npcManager.setDocFree();

            }
        }
    }

    private void debugDrawPath(NavMeshPath path)
    {
        if (path.corners.Length < 2) //if the path has 1 or no corners, there is no need
            return;

        DebugPath.SetVertexCount(path.corners.Length); //set the array of positions to the amount of corners

        for (var i = 1; i < path.corners.Length; i++)
        {
            DebugPath.SetPosition(i, path.corners[i]); //go through each corner and set that to the line renderer's position
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
            if (interactionComponent.RotateAwayFrom(myBed.transform))
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
                if(clock.currentDayTime == ClockTime.DayTime.NIGHT)
                {
                    timer = 0;
                    //randomly wake some people up
                    if(Random.Range(0, 100) > 90)
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
                else
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
        if(npcManager.isDocBusy())
        {
            if (dest == Vector3.zero)
            {
                GameObject targetChair = objectManager.bookRandomQueueChair(gameObject);
                if(targetChair == null)
                {
                    addStateToQueue(3, NPCState.STATE_LEAVE_HOSPITAL);
                    taskCompleted = true;
                }
                else
                {
                    interactionComponent.setTarget(targetChair);
                    interactionComponent.setCurrentChair(interactionComponent.getTarget());
                    // set destination to queue chair
                    dest = interactionComponent.getDestToTargetObjectSide(0, 20.0f);
                    // move to the queue position received
                    moveTo(dest);
                }

            }

            if (arrivedToDestination(10.0f) && !sitting)
            {
                agent.Stop();
                sitting = true;
            }

            if (sitting)
            {
                if(arrivedToDestination(10.0f))
                {
                    if(interactionComponent.getTarget())
                    {
                        //rotate to look away from the bed so animation will move the player on the bed
                        if (interactionComponent.RotateAwayFrom(interactionComponent.getTarget().transform))
                        {
                            agent.GetComponent<IiroAnimBehavior>().sit = true;
                        }
                    }

                }
            }
        }
        else
        {
            if(sitting)
            {
                GetComponent<IiroAnimBehavior>().sit = false;
            }
           
            taskCompleted = true;
            if((interactionComponent.getCurrentChair() != null))
                objectManager.unbookObject(interactionComponent.getCurrentChair());
            sitting = false;
            agent.Resume();
            addStateToQueue(3, NPCState.STATE_GO_TO_DOC);
            npcManager.setDocBusy();
        }
    }
    //just wander around the hospital, if npc has nothing else to do it will go to this state
    private void idle()
    {
        //check if there's something else to do
        timer += Time.deltaTime;
        GameObject target = interactionComponent.getTarget();
        if (target != null && talking)
        {
            agent.Stop();
            interactionComponent.RotateTowards(target.transform);
        }
        else
        {
            if (arrivedToDestination(30.0f))
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

                //10% chance every IDLE_IN_THIS_PLACE_TIME to start talking to somoene or go sit
                if (Random.Range(0f, 1f) > 0.90f)
                {
                    if (!talking)
                        addStateToQueue(2, NPCState.STATE_TALK_TO_OTHER_NPC);
                }
                if (Random.Range(0f, 1f) > 0.90f)
                {
                    if (!talking && !sitting)
                        addStateToQueue(2, NPCState.STATE_IDLE_SIT);
                }
            }

        }
    }
    private void die()
    {
        
        timer += Time.deltaTime;
        if(!agent.GetComponent<IiroAnimBehavior>().fall)
        {
            agent.Stop();
            agent.GetComponent<IiroAnimBehavior>().fall = true;
        }
        if (timer > STAY_ON_FLOOR_ON_FALL)
        {
            
            Destroy(responsibilityIndicatorclone);
            List<GameObject> npcList = GameObject.Find("NPCManager").GetComponent<NPCManagerV2>().npcList;
            npcList.Remove(gameObject);
            if (player.GetComponent<PlayerControl>().getTarget() == gameObject)
            {
                GameObject.FindGameObjectWithTag("TextBoxManager").GetComponent<TextBoxManager>().DisableTextBox();
            }
            npcManager.removeNpcFromPlayersResponsibilities(gameObject);
            print(myName + " lähti teho-osastolle...");
            Destroy(gameObject);
        }
        

    }
    //if player is close and player has target on this npc, talk to player
    private void talkToPlayer()
    {
        agent.Stop();
        interactionComponent.RotateTowards(player.transform);
        if (!dialogZone.GetComponent<DialogV2>().playerInZone)
        {
            if(prevState != NPCState.STATE_TALK_TO_PLAYER)
            {
                myState = prevState;
            }
            else
            {
                taskCompleted = true;
            }
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
        if  (target != null && target.tag == "NPC" && walkToTarget())
        {
            //set both npc's to talking
            target.GetComponent<NPCV2>().talking = true;
            talking = true;

            //stop moving
            agent.Stop();
            target.GetComponent<NPCV2>().agent.Stop();

            //rotate to look the target
            interactionComponent.RotateTowards(target.transform);
            timer += Time.deltaTime;
            if (timer > MAX_TIME_TALK_TO_OTHER)
            {
                timer = 0;
                target.GetComponent<NPCV2>().stopTalking();
                stopTalking();

            }
        }
        //fail proof
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
            if (npc.gameObject != gameObject)
            {
                NPCV2 script = npc.GetComponent<NPCV2>();
                if (script.isIdle())
                {
                    if(!script.talking)
                    {
                        idlenpcs.Add(npc.gameObject);
                    }
                }
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
        if (myState == NPCState.STATE_IDLE)
        {
            return true;
        }
        else return false;
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
        if(taskCompleted)
        {
            if(prevStateUncompleted)
            {
                prevStateUncompleted = false;
                myState = prevState;
                dest = Vector3.zero;
                taskCompleted = false;
                currentTaskPriority = 3;
            }
            else
            {
                Queue<NPCState> queue = new Queue<NPCState>();
                //Dequeue a task from priority 3 queue if it has a task and the current task is less important
                stateQueue.TryGetValue(3, out queue);
                if (queue.Count > 0)
                {
                    prevState = myState;
                    myState = queue.Dequeue();
                    dest = Vector3.zero;
                    taskCompleted = false;
                    currentTaskPriority = 3;
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
                        taskCompleted = false;
                        currentTaskPriority = 2;
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
                            taskCompleted = false;
                            currentTaskPriority = 1;
                        }
                        else
                        {
                            if (myState != NPCState.STATE_IDLE)
                            {
                                prevState = myState;
                                myState = NPCState.STATE_IDLE;
                                dest = Vector3.zero;
                                taskCompleted = false;
                                currentTaskPriority = 0;
                            }
                        }
                    }
                }
            }
        }
        else
        {
            Queue<NPCState> queue = new Queue<NPCState>();
            //Dequeue a task from priority 3 queue if it has a task and the current task is less important
            stateQueue.TryGetValue(3, out queue);
            if (currentTaskPriority < 3 && queue.Count > 0)
            {
                //save current state info so it can be done after the prioritized task is complete
                prevState = myState;
                myState = queue.Dequeue();
                prevStateUncompleted = true;
                prevTaskPriority = currentTaskPriority;
                dest = Vector3.zero;
                taskCompleted = false;
            }
            else
            {
                //Dequeue a task from priority 2 queue if it has a task and the current task is less important
                stateQueue.TryGetValue(2, out queue);
                if (currentTaskPriority < 2 && queue.Count > 0)
                {
                    //save current state info so it can be done after the prioritized task is complete
                    prevState = myState;
                    myState = queue.Dequeue();
                    dest = Vector3.zero;
                    prevStateUncompleted = true;
                    prevTaskPriority = currentTaskPriority;
                    taskCompleted = false;
                }
            }
        }
        
    }

    public void Init(string myName, string myId)
    {
        this.myName = myName;
        this.myId = myId;    
    }

    public void rePath()
    {
        Vector3 temp = agent.destination;
        agent.ResetPath();
        agent.SetDestination(temp);
    }

    // Init 1-4 random problems and their corresponding medicines
    public void InitMedication(Item[] randMeds)
    {
        myProblems = new string[4];
        for (int i = 0; i < myMedication.Length; i++)
        {
            if (randMeds[i] != null)
            {
                myMedication[i] = randMeds[i];
                myProblems[i] = randMeds[i].Usage;
            }
            else
            {
                myMedication[i] = null;
                myProblems[i] = null;
            }       
        }

        // assign meds to different times of day and randomize one of three dosages
        if (myMedication[0] != null)
        {
            morningMed.title = myMedication[0].Title;
            int rnd = Random.Range(1, 4);
            switch (rnd)
            {
                case 1:
                    morningDos = myMedication[0].SmallDosage;
                    break;
                case 2:
                    morningDos = myMedication[0].MediumDosage;
                    break;
                case 3:
                    morningDos = myMedication[0].HighDosage;
                    break;
            }
        }
        morningMed.isActive = false;

        if (myMedication[1] != null)
        {
            afternoonMed.title = myMedication[1].Title;
            int rnd = Random.Range(1, 4);
            switch (rnd)
            {
                case 1:
                    afternoonDos = myMedication[0].SmallDosage;
                    break;
                case 2:
                    afternoonDos = myMedication[0].MediumDosage;
                    break;
                case 3:
                    afternoonDos = myMedication[0].HighDosage;
                    break;
            }
        }
        afternoonMed.isActive = false;

        if (myMedication[2] != null)
        {
            eveningMed.title = myMedication[2].Title;
            int rnd = Random.Range(1, 4);
            switch (rnd)
            {
                case 1:
                    eveningDos = myMedication[0].SmallDosage;
                    break;
                case 2:
                    eveningDos = myMedication[0].MediumDosage;
                    break;
                case 3:
                    eveningDos = myMedication[0].HighDosage;
                    break;
            }
        }
        eveningMed.isActive = false;

        if (myMedication[3] != null)
        {
            nightMed.title = myMedication[3].Title;
            int rnd = Random.Range(1, 4);
            switch (rnd)
            {
                case 1:
                    nightDos = myMedication[0].SmallDosage;
                    break;
                case 2:
                    nightDos = myMedication[0].MediumDosage;
                    break;
                case 3:
                    nightDos = myMedication[0].HighDosage;
                    break;
            }
        }
        nightMed.isActive = false;

        // print 'em (temporary)
        if (myMedication[0] != null)
            print("aamu: " + morningMed.title + " -- " + morningDos + " -- " + myMedication[0].Usage);
        else
            print("aamu: N/A");
        if (myMedication[1] != null)
            print("päivä: " + afternoonMed.title + " -- " + afternoonDos + " -- " + myMedication[1].Usage);
        else
            print("päivä: N/A");
        if (myMedication[2] != null)
            print("ilta: " + eveningMed.title + " -- " + eveningDos + " -- " + myMedication[2].Usage);
        else
            print("ilta: N/A");
        if (myMedication[3] != null)
            print("yö: " + nightMed.title + " -- " + nightDos + " -- " + myMedication[3].Usage);
        else
            print("yö: N/A");

    }
    public void moveTo(Vector3 dest)
    {
        agent.SetDestination(dest);
    }
    void checkMed()
    {
        if (isLosingHp)
        {
            // lose hp if no medicine is active, if hp reaches zero -> die
            deathTimer += Time.deltaTime;
            if (deathTimer >= LOSE_HP_TIME)
            {
                myHp--;
                deathTimer = 0;
            }

            // check if medicine has been activated and stop losing hp if so
            ClockTime.DayTime currTime = GameObject.FindGameObjectWithTag("Clock").GetComponent<ClockTime>().currentDayTime;

            // MORNING
            if (currTime == ClockTime.DayTime.MORNING)
            {
                if (morningMed.isActive)
                    isLosingHp = false;
            }
            // AFTERNOON
            if (currTime == ClockTime.DayTime.AFTERNOON)
            {
                if (afternoonMed.isActive)
                    isLosingHp = false;
            }
            // EVENING
            if (currTime == ClockTime.DayTime.EVENING)
            {
                if (eveningMed.isActive)
                    isLosingHp = false;
            }
            // NIGHT
            if (currTime == ClockTime.DayTime.NIGHT)
            {
                if (nightMed.isActive)
                    isLosingHp = false;
            }
        }
        if (myHp <= 0)
        {
            addStateToQueue(3, NPCState.STATE_DEAD);
            taskCompleted = true;
        }
    }
    public bool giveMed(string med)
    {
        // make sure the NPC is diagnosed and player is near enough
        if (diagnosed && dialogZone.GetComponent<DialogV2>().playerInZone)
        {
            // check the current time of the day to do the correct comparsion
            ClockTime.DayTime currTime = GameObject.FindGameObjectWithTag("Clock").GetComponent<ClockTime>().currentDayTime;

            // MORNING
            if (currTime == ClockTime.DayTime.MORNING)
            {
                if (string.Equals(med, morningMed.title, System.StringComparison.CurrentCultureIgnoreCase))
                {
                    morningMed.isActive = true;
                    myHp = myHp + 20;
                    print("Correct medicine!");
                }
                else
                {
                    morningMed.isActive = false;
                    myHp = myHp - 20;
                    print("Wrong medicine! " + myName + " lost 20HP!");
                }
                return true;
            }
            // AFTERNOON
            else if (currTime == ClockTime.DayTime.AFTERNOON)
            {
                if (string.Equals(med, afternoonMed.title, System.StringComparison.CurrentCultureIgnoreCase))
                {
                    afternoonMed.isActive = true;
                    myHp = myHp + 20;
                    print("Correct medicine!");
                }
                else
                {
                    afternoonMed.isActive = false;
                    myHp = myHp - 20;
                    print("Wrong medicine! " + myName + " lost 20HP!");
                }
                return true;
            }
            // EVENING
            else if (currTime == ClockTime.DayTime.EVENING)
            {
                if (string.Equals(med, eveningMed.title, System.StringComparison.CurrentCultureIgnoreCase))
                {
                    eveningMed.isActive = true;
                    myHp = myHp + 20;
                    print("Correct medicine!");
                }
                else
                {
                    eveningMed.isActive = false;
                    myHp = myHp - 20;
                    print("Wrong medicine! " + myName + " lost 20HP!");
                }
                return true;
            }
            // NIGHT
            else if (currTime == ClockTime.DayTime.NIGHT)
            {
                if (string.Equals(med, nightMed.title, System.StringComparison.CurrentCultureIgnoreCase))
                {
                    nightMed.isActive = true;
                    myHp = myHp + 20;
                    print("Correct medicine!");
                }
                else
                {
                    nightMed.isActive = false;
                    myHp = myHp - 20;
                    print("Wrong medicine! " + myName + " lost 20HP!");
                }
                return true;
            }
        }
        return false;
    }

    public void disableAllMeds()
    {
        morningMed.isActive = false;
        afternoonMed.isActive = false;
        eveningMed.isActive = false;
        nightMed.isActive = false;
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
