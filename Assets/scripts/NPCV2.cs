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
        STATE_IDLE_SIT,
        STATE_DEFAULT //Do nothing
    }

    /* basic stuff */
    bool lockstate = false;
    public bool paused = false;
    ClockTime clock;
    LineRenderer DebugPath;
    IiroAnimBehavior animations;
    public string myName;
    public string myId;
    public int myHp = 50;
    public int myHappiness = 50;
    int currentTaskPriority = 0;
    public Sprite myHead2d;
    int prevTaskPriority = 0;
    bool dead = false;
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
    public bool taskCompleted = true;
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
    ScoringSystem scoreSystem;

    /* NEW MEDICINE SYSTEM */
    float deathTimer; // time without medicine
    const int LOSE_HP_TIME = 2; // lose one hitpoint every X seconds if there is no medicine active

    public struct Medicine
    {
        public string title;
        public bool isActive;
        public float dosage;
    };
    
    public Item[] myMedication; // all of this NPC's meds, usages etc.
    public string[] myProblems;
    
    /* specific meds for different times of day */
    public Medicine[] morningMed = new Medicine[4];
    public Medicine[] afternoonMed = new Medicine[4];
    public Medicine[] eveningMed = new Medicine[4];
    public Medicine[] nightMed = new Medicine[4];
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
    const float RECEPTION_WAITING_TIME = 1.5f;
    const float QUE_WAITING_TIME = 10f;
    const float IDLE_IN_THIS_PLACE_TIME = 5f;
    const float MAX_TIME_TALK_TO_OTHER = 10f;
    const int WALK_RADIUS = 500;
    const float SLEEP_TIME = 10f;
    const float AT_DOC = 5f;
    const float IN_WC = 10f;
    const float STAY_ON_FLOOR_ON_FALL = 10.0f;

    //stuck testing
    const float STUCK = 2f;
    //how long doctor will wait for patient
    const float DOC_WAIT_TIME = 10.0f;
    float doctimer = 0;
    float stucktimer = 0f;
    //current distance to destination
    float currentdisttodest;
    //distance to destination at last test
    float lastdisttodest;

    Vector3 lastAgentVelocity = Vector3.zero;
    NavMeshPath lastAgentPath;
    bool agentPaused = false;

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
        animations = agent.GetComponent<IiroAnimBehavior>();
        scoreSystem = GameObject.FindGameObjectWithTag("ScoringSystem").GetComponent<ScoringSystem>();
    }
    // Update is called once per frame
    void Update()
    {
        //Weird bug caused stateQueue to be set to null if game window was not focused, so lets check if dictionary is null and create new empty one to reset npc behavior
        if(stateQueue == null)
        {
            stateQueue = new Dictionary<int, Queue<NPCState>>();
            stateQueue.Add(1, new Queue<NPCState>());
            stateQueue.Add(2, new Queue<NPCState>());
            stateQueue.Add(3, new Queue<NPCState>());
        }

        if(paused)
        {
            if(!agentPaused)
                pause();
        }

        if(!paused)
        {
            if(agentPaused)
            {
                resume();
            }
            if (playersResponsibility)
            {
                responsibilityIndicatorclone.transform.position = new Vector3(transform.position.x, transform.position.y + 64, transform.position.z);
                responsibilityIndicatorclone.transform.rotation = transform.rotation;
            }
            //check if there are some natural needs, or unstucking needs
            checkNeeds();
            //Set current state to highest priority currently queued
            //if higher priority job compared to current state is found, current state will be paused
            if(!lockstate)
                setMyStateFromQueue();
            //Act according to the myState (Current state)
            actAccordingToState();
        }

    }

    void pause()
    {
        if(agent.enabled)
        {
            lastAgentVelocity = agent.velocity;
            lastAgentPath = agent.path;
            agent.velocity = Vector3.zero;
            agent.ResetPath();
        }

        agentPaused = true;
    }

    void resume()
    {
        if(agent.enabled)
        {
            agent.velocity = lastAgentVelocity;
            agent.SetPath(lastAgentPath);
        }

        agentPaused = false;
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
            case NPCState.STATE_DEFAULT:
                break;
        }
    }

    void checkNeeds()
    {
        //stuck test
        stucktimer += Time.deltaTime;
        //test every STUCK seconds if npc hasn't moved towards it's dest
        /*
        if (stucktimer > STUCK && dest != Vector3.zero && !arrivedToDestination(10.0f))
        {
            if(myState != NPCState.STATE_TALK_TO_PLAYER)
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
                        if (myState != NPCState.STATE_TRY_UNSTUCK)
                        {
                            lastdisttodest = 0;
                            addStateToQueue(3, NPCState.STATE_TRY_UNSTUCK);
                        }
                        else
                        {
                            dest = Vector3.zero;
                        }
                    }
                    lastdisttodest = 0;
                }
            }
        }*/

        if (myState != NPCState.STATE_TALK_TO_PLAYER && myState != NPCState.STATE_DEAD && !sleeping && !sitting && dialogZone.GetComponent<DialogV2>().playerInZone)
        {
            addStateToQueue(3, NPCState.STATE_TALK_TO_PLAYER);
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
            if (!sleepingqueued && !sleeping)
            {
                fatiquetimer += Time.deltaTime;
                if (fatiquetimer > 5.0f)
                {
                    fatique += 1f;
                    fatiquetimer = 0;
                }
                //if has bed and fatique over x, queue sleep task
                if (fatique > 10.0f)
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
                    callofnaturetimer = 0;
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
        else if (arrivedToDestination(10.0f) && !sitting)
        {
            //rotate to look away from the chair so animation will move the player on the chair
            if (interactionComponent.RotateAwayFrom(interactionComponent.getTarget().transform))
            {
                sitting = true;
                agent.Stop();
                if (interactionComponent.getCurrentChair().tag == "Chair2")
                {
                    agent.GetComponent<IiroAnimBehavior>().sitwithrotation();
                }
                else
                {
                    agent.GetComponent<IiroAnimBehavior>().sit();
                }
            }
        }
        else if (sitting)
        {
            timer += Time.deltaTime;
            if(timer > 2 * IDLE_IN_THIS_PLACE_TIME)
            {
                if(interactionComponent.getCurrentChair().tag == "Chair2")
                {
                    GetComponent<IiroAnimBehavior>().stopSitwithrotation();
                }
                else
                {
                    GetComponent<IiroAnimBehavior>().stopSit();
                }
                
                taskCompleted = true;
                if ((interactionComponent.getCurrentChair() != null))
                {
                    objectManager.unbookObject(interactionComponent.getCurrentChair());
                    interactionComponent.setCurrentChair(null);
                }
                    
                sitting = false;
                if (!agent.enabled)
                    agent.enabled = true;
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
        else if (arrivedToDestination(10.0f) && !sitting)
        {
            agent.Stop();
            sitting = true;
        }

        else if (sitting)
        {
            if (arrivedToDestination(20.0f))
            {
                if (interactionComponent.getTarget())
                {
                    //rotate to look away from the bed so animation will move the player on the bed
                    if (interactionComponent.RotateAwayFrom(interactionComponent.getTarget().transform))
                    {
                        agent.GetComponent<IiroAnimBehavior>().sit();
                        timer += Time.deltaTime;
                        if (timer > IN_WC)
                        {
                            GetComponent<IiroAnimBehavior>().stopSit();
                            taskCompleted = true;
                            if ((interactionComponent.getCurrentChair() != null))
                            {
                                objectManager.unbookObject(interactionComponent.getCurrentChair());
                                interactionComponent.setCurrentChair(null);
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
        agent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
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
            agent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
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
        if(arrivedToDestination(100.0f))
        {
            npcManager.deleteNpcFromList(gameObject);
            Destroy(gameObject);
        }
    }

    private void goToDoc()
    {
        if(!diagnosed)
        {
            doctimer += Time.deltaTime;
            if (dest == Vector3.zero)
            {
                dest = new Vector3(-50.0f, transform.position.y, 393.0f);
                moveTo(dest);
                timer = 0;
                doctimer = 0;
            }
            if (arrivedToDestination(30.0f))
            {
                doctimer = 0;
                timer += Time.deltaTime;
                if (timer > AT_DOC)
                {
                    int r = Random.Range(1, 10);
                    if (r > 1 && npcManager.currentNpcsInWard < NPCManagerV2.MAX_NPCS_IN_WARD_AREA)
                    {
                        // Randomize 1-4 DIFFERENT problems for the NPC
                        int numProblems = UnityEngine.Random.Range(1, 5);
                        Item[] randMeds = new Item[4];
                        // Fetch random medicine items from database
                        for (int i = 0; i < randMeds.Length; i++)
                        {
                            if (numProblems > 0)
                            {
                                randMeds[i] = npcManager.RandomItem(randMeds);
                                numProblems--;
                            }
                            else
                                randMeds[i] = null;
                        }
                        InitMedication(randMeds);
                        addStateToQueue(2, NPCState.STATE_MOVE_TO_WARD_AREA);
                        diagnosed = true;
                        if (!npcManager.isPlayerResponsibilityLevelFulfilled())
                        {
                            npcManager.addNpcToPlayersResponsibilities(gameObject);
                            playersResponsibility = true;
                            responsibilityIndicatorclone = (GameObject)Instantiate(responsibilityIndicator, transform.position, new Quaternion(0, 0, 0, 0));
                        }
                        npcManager.currentNpcsInWard++;
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
        else
        {
            taskCompleted = true;
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
            dest = interactionComponent.getDestToTargetObjectSide(1, 25.0f);
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
            GetComponent<IiroAnimBehavior>().sleep();
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
                        GetComponent<IiroAnimBehavior>().stopSleep();
                        sleeping = false;
                        taskCompleted = true;
                        timer = 0;
                        fatique = 0;
                        //resume agent movement
                        if (!agent.enabled)
                            agent.enabled = true;
                        agent.Resume();
                        sleepingqueued = false;
                    }
                }
                else
                {
                    //stop animation
                    GetComponent<IiroAnimBehavior>().stopSleep();
                    sleeping = false;
                    taskCompleted = true;
                    timer = 0;
                    fatique = 0;
                    //resume agent movement
                    if (!agent.enabled)
                        agent.enabled = true;
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
        if (arrivedToDestination(50.0f))
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
                            agent.GetComponent<IiroAnimBehavior>().sit();
                        }
                    }

                }
            }
        }
        else
        {
            if(sitting)
            {
                GetComponent<IiroAnimBehavior>().stopSit();
            }
           
            taskCompleted = true;
            if((interactionComponent.getCurrentChair() != null))
            {
                objectManager.unbookObject(interactionComponent.getCurrentChair());
                interactionComponent.setCurrentChair(null);
            }
                
            sitting = false;
            if (!agent.enabled)
                agent.enabled = true;
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
        //if there is a npc in dialogzone and it has target to this, assume it's talking to this
        if (talking)
        {
            interactionComponent.setTarget(dialogZone.GetComponent<DialogV2>().getNpcTargetingMe());
            interactionComponent.RotateTowards(interactionComponent.getTarget().transform);
        }
        else if (dest == Vector3.zero)
        {
            // move to idle at random position
            Vector3 randomDirection = Random.insideUnitSphere * WALK_RADIUS;
            randomDirection += transform.position;
            NavMeshHit hit;
            NavMesh.SamplePosition(randomDirection, out hit, WALK_RADIUS, 1);
            Vector3 finalPosition = hit.position;
            dest = new Vector3(finalPosition.x, 0, finalPosition.z);
            moveTo(dest);
            talking = false;
        }
        else
        {
            if (arrivedToDestination(30.0f))
            {
                if((timer > IDLE_IN_THIS_PLACE_TIME))
                {
                    timer = 0;
                    taskCompleted = true;
                    talking = false;
                }
            }
        }
    }

    private void die()
    {

        lockstate = true;
        if (sleeping)
        {
            animations.stopSleep();
            sleeping = false;
        }
        else if (sitting)
        {
            animations.stopSit();
            sitting = false;
        }
        if (!dead)
        {
            objectManager.unbookObject(myBed);
            //first iteration will set destination to area where fetching nurses can actually move
            if (dest == Vector3.zero)
            {
                NavMeshHit hit;
                NavMesh.SamplePosition(transform.position, out hit, 2000.0f, (1 << 7));
                dest = hit.position;
                agent.SetDestination(dest);
                interactionComponent.setTarget(null);
            }
            //When arrived to good position, set npc to dead and rmeove from npclists
            else if (arrivedToDestination(5.0f))
            {
                npcManager.npcList.Remove(gameObject);
                //if this npc is players target, make sure textboxmanager disables UI showing npc status
                if (GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControl>().getTarget() == gameObject)
                {
                    GameObject.FindGameObjectWithTag("TextBoxManager").GetComponent<TextBoxManager>().DisableTextBox();
                }
                //if this npc was players responsibility, remove from responsibilities
                if(playersResponsibility)
                {
                    playersResponsibility = false;
                    npcManager.removeNpcFromPlayersResponsibilities(gameObject);
                    Destroy(responsibilityIndicatorclone);
                }
                    
                dead = true;
            }
        }
        else
        {
            if (gameObject == null)
                return;
            //if dead and nurses aren't already deployed to fetch someone, release the nurses
            if (!npcManager.nursesDeployed)
                npcManager.spawnNurseToFetchNPC(gameObject);
            timer += Time.deltaTime;
            //if not already fallen, fall
            if (!animations.falling)
            {
                agent.Stop();
                agent.GetComponent<IiroAnimBehavior>().fall();
            }
        }
    }
    //if player is close and player has target on this npc, talk to player
    private void talkToPlayer()
    {
        agent.Stop();
        interactionComponent.RotateTowards(player.transform);
        if (!dialogZone.GetComponent<DialogV2>().playerInZone)
        {
            taskCompleted = true;
            agent.Resume();
            if(!diagnosed)
            {
                addStateToQueue(2, NPCState.STATE_QUE);
            }
        }
    }
    private void talkToNPC()
    {
        GameObject target = interactionComponent.getTarget();
        //check that the target is actually capable of talking
        if (target == null || target.tag != "NPC" || !target.GetComponent<NPCV2>().isIdle() || target.GetComponent<NPCV2>().dead || target.GetComponent<NPCV2>().sleeping || target.GetComponent<NPCV2>().sitting)
        {
            findOtherIdleNPC();
        }
        //check if at target & set destination
        else if(walkToTarget())
        {
            if(!talking)
            {
                talking = true;
                target.GetComponent<NPCV2>().talking = true;
                //stop moving
                agent.Stop();
                target.GetComponent<NPCV2>().agent.Stop();
            }
            //rotate to look the target
            interactionComponent.RotateTowards(target.transform);
            timer += Time.deltaTime;
            if (timer > MAX_TIME_TALK_TO_OTHER)
            {
                timer = 0;
                stopTalking();
                target.GetComponent<NPCV2>().stopTalking();
            }
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
            if(!agent.hasPath)
                moveTo(target.transform.position);
            return false;
        }
    }
    //looks for other npcs who are idle
    private void findOtherIdleNPC()
    {
        List<GameObject> npcs = npcManager.npcList;
        List<GameObject> idlenpcs = new List<GameObject>();
        foreach (GameObject npc in npcs)
        {
            if (npc.gameObject != gameObject)
            {
                NPCV2 script = npc.GetComponent<NPCV2>();
                if (script.isIdle() && !script.sitting && !script.sleeping)
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
        if(!queue.Contains(state))
        {
            queue.Enqueue(state);
        }
        
    }

    void unbookAllMyObjects()
    {
        
    }

    //finds the highest priority task and selects it as current stage
    //called only when task is completed
    public void setMyStateFromQueue()
    {
        if(taskCompleted)
        {
            timer = 0;
            //fool proof
            if(interactionComponent.getCurrentChair() != null)
            {
                objectManager.unbookObject(interactionComponent.getCurrentChair());
                interactionComponent.setCurrentChair(null);
            }
            if (prevStateUncompleted)
            {
                prevStateUncompleted = false;
                addStateToQueue(3, prevState);
                myState = NPCState.STATE_DEFAULT;
                currentTaskPriority = 0;
                dest = Vector3.zero;
                taskCompleted = true;
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
                            //if nothing to do choose randomly from talking with npcs, sitting, idle walking
                            
                            if (Random.Range(1, 11) > 7 && !talking)
                            {
                                prevState = myState;
                                myState = NPCState.STATE_TALK_TO_OTHER_NPC;
                            } 
                            else if (Random.Range(1, 11) > 7 && !talking && !sitting)
                            {
                                prevState = myState;
                                myState = NPCState.STATE_IDLE_SIT;
                            }
                            else
                            {
                                prevState = myState;
                                myState = NPCState.STATE_IDLE;
                            }
                            dest = Vector3.zero;
                            taskCompleted = false;
                            currentTaskPriority = 1;
                        }
                    }
                }
            }
        }
        else if (!prevStateUncompleted)
        {
            Queue<NPCState> queue = new Queue<NPCState>();
            //Dequeue a task from priority 3 queue if it has a task and the current task is less important
            stateQueue.TryGetValue(3, out queue);
            if (currentTaskPriority < 3 && queue.Count > 0)
            {
                if (talking)
                {
                    stopTalking();
                    interactionComponent.getTarget().GetComponent<NPCV2>().stopTalking();
                }
                //if currenttaskpriority is higher than 1 we will resume the task after new task is complete
                if (currentTaskPriority > 1)
                    prevStateUncompleted = true;
                //save current state info so it can be done after the prioritized task is complete
                prevState = myState;
                myState = queue.Dequeue();
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
                    if (talking)
                    {
                        stopTalking();
                        interactionComponent.getTarget().GetComponent<NPCV2>().stopTalking();
                    }
                    //save current state info so it can be done after the prioritized task is complete
                    prevState = myState;
                    myState = queue.Dequeue();
                    dest = Vector3.zero;
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

    float getRandomDosage(Item medicine)
    {
        float ret = medicine.DefaultDosage;
        int range = 0;
        if (medicine.canSplit == 0)
        {
           range = Random.Range(2, 4);
        }
        else
        {
            range = Random.Range(1, 4);
        }
        
        switch(range)
        {
            case 1:
                ret = medicine.SmallDosage;
                break;
            case 2:
                ret = medicine.MediumDosage;
                break;
            case 3:
                ret = medicine.HighDosage;
                break;
        }
        return ret;
    }

    // Init 1-4 random problems and their corresponding medicines
    public void InitMedication(Item[] randMeds)
    {
        int drugscount = 0;
        //count how many drugs were generated
        foreach(Item item in randMeds)
        {
            if (item != null)
            {
                drugscount++;
            }
        }
        myMedication = new Item[drugscount];
        myProblems = new string[drugscount];
        //move drugs to locals
        for (int i = 0; i < drugscount; i++)
        {
            myMedication[i] = randMeds[i];
            myProblems[i] = randMeds[i].Usage;
        }

        for (int i = 0; i < myMedication.Length; i++)
        {
            float dosage = getRandomDosage(myMedication[i]);
            if (myMedication[i].timesPerDay > 0)
            {
                List<int> daytimes = new List<int>();
                daytimes.Add(0);
                daytimes.Add(1);
                daytimes.Add(2);
                daytimes.Add(3);
                int[] rnddaytimes = new int[myMedication[i].timesPerDay];
                int n = daytimes.Count;
                //shuffle daytimes
                while (n > 1)
                {
                    n--;
                    int k = Random.Range(0, n + 1);
                    int value = daytimes[k];
                    daytimes[k] = daytimes[n];
                    daytimes[n] = value;
                }
                for (int r = 0; r < rnddaytimes.Length; r++)
                {
                    rnddaytimes[r] = daytimes[r];
                }
                for (int r = 0; r < rnddaytimes.Length; r++)
                {
                    daytimes.Remove(rnddaytimes[r]);
                }
                //add medication to each random daytime array
                foreach (int daytime in rnddaytimes)
                {
                    switch(daytime)
                    {
                        case 0:
                            morningMed[i].title = myMedication[i].Title;
                            morningMed[i].dosage = dosage;
                            break;
                        case 1:
                            afternoonMed[i].title = myMedication[i].Title;
                            afternoonMed[i].dosage = dosage;
                            break;
                        case 2:
                            eveningMed[i].title = myMedication[i].Title;
                            eveningMed[i].dosage = dosage;
                            break;
                        case 3:
                            nightMed[i].title = myMedication[i].Title;
                            nightMed[i].dosage = dosage;
                            break;
                    }
                }
                //nullify remaining daytimes for this medicine
                if(daytimes.Count > 0)
                {
                    foreach(int daytime in daytimes)
                    {
                        switch (daytime)
                        {
                            case 0:
                                morningMed[i].title = null;
                                morningMed[i].dosage = 0;
                                break;
                            case 1:
                                afternoonMed[i].title = null;
                                afternoonMed[i].dosage = 0;
                                break;
                            case 2:
                                eveningMed[i].title = null;
                                eveningMed[i].dosage = 0;
                                break;
                            case 3:
                                nightMed[i].title = null;
                                nightMed[i].dosage = 0;
                                break;
                        }
                    }
                }
            }
            else
            {
                List<int> daytimes = new List<int>();
                daytimes.Add(0);
                daytimes.Add(1);
                daytimes.Add(2);
                daytimes.Add(3);
                //randomize how many times this med will be given in a day
                int rnd = Random.Range(1, 4);
                //shuffle daytimes
                int n = daytimes.Count;
                while (n > 1)
                {
                    n--;
                    int k = Random.Range(0, n + 1);
                    int value = daytimes[k];
                    daytimes[k] = daytimes[n];
                    daytimes[n] = value;
                }
                int[] rnddaytimes = new int[rnd];
                int sub = daytimes.Count - rnd;
                for(int r = 0; r < rnd; r++)
                {
                    rnddaytimes[r] = daytimes[r];
                }
                for (int r = 0; r < rnddaytimes.Length; r++)
                {
                    daytimes.Remove(rnddaytimes[r]);
                }


                //add medication to each random daytime array
                foreach (int daytime in rnddaytimes)
                {
                    switch (daytime)
                    {
                        case 0:
                            morningMed[i].title = myMedication[i].Title;
                            morningMed[i].dosage = dosage;
                            break;
                        case 1:
                            afternoonMed[i].title = myMedication[i].Title;
                            afternoonMed[i].dosage = dosage;
                            break;
                        case 2:
                            eveningMed[i].title = myMedication[i].Title;
                            eveningMed[i].dosage = dosage;
                            break;
                        case 3:
                            nightMed[i].title = myMedication[i].Title;
                            nightMed[i].dosage = dosage;
                            break;
                    }
                }
                //nullify remaining daytimes for this medicine
                if (daytimes.Count > 0)
                {
                    foreach (int daytime in daytimes)
                    {
                        switch (daytime)
                        {
                            case 0:
                                morningMed[i].title = null;
                                morningMed[i].dosage = 0;
                                break;
                            case 1:
                                afternoonMed[i].title = null;
                                afternoonMed[i].dosage = 0;
                                break;
                            case 2:
                                eveningMed[i].title = null;
                                eveningMed[i].dosage = 0;
                                break;
                            case 3:
                                nightMed[i].title = null;
                                nightMed[i].dosage = 0;
                                break;
                        }
                    }
                }
            }
        }
    }
    public void moveTo(Vector3 dest)
    {
        if(agent.enabled)
        {
            NavMeshHit hit;
            if (NavMesh.SamplePosition(dest, out hit, 10.0f, agent.areaMask))
                agent.SetDestination(hit.position);
            else
                agent.SetDestination(dest);
        }
            
    }
    void checkMed()
    {
        if(!paused)
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
                    foreach (Medicine med in morningMed)
                    {
                        if (med.isActive)
                            isLosingHp = false;
                    }
                }
                // AFTERNOON
                if (currTime == ClockTime.DayTime.AFTERNOON)
                {
                    foreach (Medicine med in afternoonMed)
                    {
                        if (med.isActive)
                            isLosingHp = false;
                    }
                }
                // EVENING
                if (currTime == ClockTime.DayTime.EVENING)
                {
                    foreach (Medicine med in eveningMed)
                    {
                        if (med.isActive)
                            isLosingHp = false;
                    }
                }
                // NIGHT
                if (currTime == ClockTime.DayTime.NIGHT)
                {
                    foreach (Medicine med in nightMed)
                    {
                        if (med.isActive)
                            isLosingHp = false;
                    }
                }
            }
            if (myHp <= 0)
            {
                addStateToQueue(3, NPCState.STATE_DEAD);
                taskCompleted = true;
            }
        }
        
    }
    public bool giveMed(string[] med, float[] dosage)
    {
        // make sure the NPC is diagnosed and player is near enough
        if (diagnosed && dialogZone.GetComponent<DialogV2>().playerInZone && myState != NPCState.STATE_DEAD)
        {
            // check the current time of the day to do the correct comparsion
            ClockTime.DayTime currTime = GameObject.FindGameObjectWithTag("Clock").GetComponent<ClockTime>().currentDayTime;
            //incorrect rating, add 1 for every error
            int incorrect = 0;
            int correct = 0;
            Medicine[] meds = new Medicine[4];
            // MORNING
            if (currTime == ClockTime.DayTime.MORNING)
            {
                meds = morningMed;
            }
            else if (currTime == ClockTime.DayTime.AFTERNOON)
            {
                meds = afternoonMed;
            }
            else if (currTime == ClockTime.DayTime.EVENING)
            {
                meds = eveningMed;
            }
            else if (currTime == ClockTime.DayTime.NIGHT)
            {
                meds = nightMed;
            }


            List<string> wrongmeds = new List<string>();
            for (int i = 0; i < med.Length; i++)
            {
                bool found = false;
                //iterate through all morning meds
                for (int j = 0; j < meds.Length; j++)
                {
                    if (string.Equals(med[i], meds[j].title, System.StringComparison.CurrentCultureIgnoreCase))
                    {
                        //if the med was already given, giving more means overdose
                        if(meds[j].isActive)
                        {
                            incorrect++;
                            GetComponent<FloatTextNPC>().addFloatText(FloatText.IncorrectMedicine);
                        }
                        else if (dosage[i] == meds[j].dosage)
                        {
                            found = true;
                            meds[j].isActive = true;
                            correct++;
                        }
                    }
                    if (found)
                    {
                        break;
                    }
                }
                if (!found)
                {
                    wrongmeds.Add(med[i]);
                    GetComponent<FloatTextNPC>().addFloatText(FloatText.IncorrectMedicine);
                }
            }
            for (int i = 0; i < wrongmeds.Count; i++)
            {
                incorrect++;
            }

            int medcount = correct + incorrect;

            float temp = correct / medcount;
            float correctratio = temp - (incorrect / medcount);

            if(correctratio > 0)
            {
                if (correctratio > 0 && correctratio <= 0.25f)
                {
                    GetComponent<FloatTextNPC>().addFloatText(FloatText.Plus5);
                    myHp += 5;
                }
                else if (correctratio > 0.25f && correctratio <= 0.5f)
                {
                    GetComponent<FloatTextNPC>().addFloatText(FloatText.Plus10);
                    myHp += 10;
                }
                else if (correctratio > 0.5f && correctratio <= 0.75f)
                {
                    GetComponent<FloatTextNPC>().addFloatText(FloatText.Plus10);
                    GetComponent<FloatTextNPC>().addFloatText(FloatText.Plus5);
                    myHp += 15;
                }
                else if (correctratio > 0.75f && correctratio <= 1f)
                {
                    GetComponent<FloatTextNPC>().addFloatText(FloatText.Plus10);
                    myHp += 20;
                }
            }
            else if(correctratio < 0)
            {
                if (correctratio < 0 && correctratio >= -0.25f)
                {
                    GetComponent<FloatTextNPC>().addFloatText(FloatText.Minus5);
                    myHp -= 5;
                }
                else if (correctratio < -0.25f && correctratio >= -0.5f)
                {
                    GetComponent<FloatTextNPC>().addFloatText(FloatText.Minus10);
                    myHp -= 10;
                }
                else if (correctratio < -0.5f && correctratio >= -0.75f)
                {
                    GetComponent<FloatTextNPC>().addFloatText(FloatText.Minus10);
                    GetComponent<FloatTextNPC>().addFloatText(FloatText.Minus5);
                    myHp -= 15;
                }
                else if (correctratio < -0.75f && correctratio >= -1f)
                {
                    GetComponent<FloatTextNPC>().addFloatText(FloatText.Minus20);
                    myHp -= 20;
                }
            }

            scoreSystem.addToScore(correctratio);
            return true;
        }
        else return false;
    }

    public void disableAllMeds()
    {
        for(int i = 0; i < morningMed.Length; i++)
        {
            morningMed[i].isActive = false;
        }
        for (int i = 0; i < afternoonMed.Length; i++)
        {
            afternoonMed[i].isActive = false;
        }
        for (int i = 0; i < eveningMed.Length; i++)
        {
            eveningMed[i].isActive = false;
        }
        for (int i = 0; i < nightMed.Length; i++)
        {
            nightMed[i].isActive = false;
        }
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

