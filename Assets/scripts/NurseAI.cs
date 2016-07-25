using UnityEngine;
using System.Collections;

/* Fetching nurses are working in pairs to fetch passed out npc's */

public class NurseAI : MonoBehaviour {
    /* NPC who needs to be fetched to ER */
    GameObject targetNPC;
    /* Indicates if nurse is with trolley or the one who picks the patient up
     * 0 - trolley, 1 - picker 
    */
    int id;
    NavMeshAgent agent;
    ObjectInteraction interaction;
    IiroAnimBehavior anim;
    Vector3 dest = Vector3.zero;
    NavMeshPath path;
    bool initialized = false;
    Transform trolley;
    bool readyToLeave = false;
    /*Has the nurse done everything*/
    public bool allDone = false;
    /*This nurse is ready to lift*/
    bool readyForLift = false;
    /*Nurse's partner reference*/
    public NurseAI partner;
    float timer = 0;
    NPCManager npcManager;
    Vector3 startPos;

	// Use this for initialization
	void Start ()
    {
        npcManager = GameObject.FindGameObjectWithTag("NPCManager").GetComponent<NPCManager>();
        startPos = transform.position;
    }
	
    void moveToDest()
    {
        agent.SetDestination(dest);
    }

    /* Debug draw line for agent path in unity editor if gizmos are selected*/
    void OnDrawGizmosSelected()
    {

        if (agent == null || agent.path == null)
            return;

        var line = this.GetComponent<LineRenderer>();
        if (line == null)
        {
            line = this.gameObject.AddComponent<LineRenderer>();
            line.material = new Material(Shader.Find("Sprites/Default")) { color = Color.yellow };
            line.SetWidth(0.5f, 0.5f);
            line.SetColors(Color.yellow, Color.yellow);
        }

        var path = agent.path;

        line.SetVertexCount(path.corners.Length);

        for (int i = 0; i < path.corners.Length; i++)
        {
            line.SetPosition(i, path.corners[i]);
        }

    }

    // Update is called once per frame
    void Update () {

        if (initialized && !allDone)
        {
            /* Behavior for trolley nurse */
            if (id == 0)
            {
                if(dest == Vector3.zero)
                {
                    agent.stoppingDistance = 200.0f;
                    dest = targetNPC.transform.position;
                    interaction.setTarget(targetNPC);
                    agent.SetDestination(dest);
                }

                else if(arrivedToDestination(250.0f) && !readyToLeave)
                {
                    if (agent.stoppingDistance > 100.0f)
                        agent.stoppingDistance = 90.0f;
                    if(arrivedToDestination(100.0f))
                    {
                        agent.Stop();
                        readyForLift = true;
                        if (partner.readyToLeave)
                        {
                            targetNPC.GetComponent<NavMeshAgent>().enabled = false;
                            targetNPC.transform.position = new Vector3(trolley.position.x, 24.0f, trolley.position.z);
                            targetNPC.transform.rotation = Quaternion.Euler(new Vector3(trolley.eulerAngles.x, trolley.eulerAngles.y + 90.0f, trolley.eulerAngles.z));

                            targetNPC.transform.SetParent(trolley);
                            targetNPC.transform.localPosition = new Vector3(targetNPC.transform.localPosition.x - 20f, targetNPC.transform.localPosition.y, targetNPC.transform.localPosition.z);
                            readyToLeave = true;
                            NavMeshHit hit;
                            dest = startPos;
                            NavMesh.SamplePosition(dest, out hit, 50.0f, NavMesh.AllAreas);
                            agent.stoppingDistance = 200.0f;
                            dest = hit.position;
                            moveToDest();
                            agent.Resume();
                        }
                    }

                }
                else if (partner.readyForLift && !arrivedToDestination(100.0f) && !readyToLeave)
                {
                    timer += Time.deltaTime;
                    if (timer > 5.0f)
                    {
                        timer = 0;
                        agent.Warp(agent.destination);
                    }
                }
            }

            /* Behavior for picker nurse */
            else if(id == 1)
            {
                if (dest == Vector3.zero)
                {
                    agent.stoppingDistance = 150.0f;
                    interaction.setTarget(targetNPC);
                    dest = interaction.getDestToTargetNPCSide(1, 16.0f);
                    moveToDest();
                }
                else if (arrivedToDestination(200.0f))
                {
                    agent.stoppingDistance = 50.0f;
                    if (!readyForLift)
                        readyForLift = true;
                    if (!anim.pickingup && !readyToLeave && partner.readyForLift)
                    {
                        if(interaction.RotateTowards(targetNPC.transform))
                            anim.pickfromfloor();
                    }
                    else if (anim.pickingup)
                    {
                        timer += Time.deltaTime;
                    }
                    if (timer > 2.0f)
                    {
                        readyToLeave = true;
                        timer = 0;
                        anim.StopAll();
                        dest = startPos;
                        moveToDest();
                    } 
                }
            }
            /* if npc is picked up and arrived to ER */
            if(readyToLeave && arrivedToDestination(200.0f))
            {
                /* Lower the stoppingDistance to make nurse with trolley more accurate with stopping */
                agent.stoppingDistance = 50.0f;
                if (readyToLeave && arrivedToDestination(100.0f))
                {
                    allDone = true;
                }
            }
            /*If stuck and partner already left, mark as allDone*/
            else if(readyToLeave && partner == null)
            {
                timer += Time.deltaTime;
                if(timer > 5.0f)
                {
                    allDone = true;
                }
            }
            
        }
	}

    public void Init(GameObject npc, int id)
    {
        targetNPC = npc;
        this.id = id;
        initialized = true;
        agent = GetComponent<NavMeshAgent>();
        interaction = GetComponent<ObjectInteraction>();
        anim = GetComponent<IiroAnimBehavior>();
        interaction.setTarget(targetNPC);
        if(id == 0)
        {
            trolley = transform.FindChild("Trolley");
        }
    }

    private bool arrivedToDestination(float accuracy)
    {
        float dist = Vector3.Distance(dest, transform.position);
        if (dist < accuracy)
            return true;
        else
            return false;
    }
}
