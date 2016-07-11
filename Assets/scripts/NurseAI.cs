using UnityEngine;
using System.Collections;

public class NurseAI : MonoBehaviour {


    GameObject targetNPC;
    int id;
    NavMeshAgent agent;
    ObjectInteraction interaction;
    IiroAnimBehavior anim;
    Vector3 dest = Vector3.zero;
    NavMeshPath path;
    bool initialized = false;
    Transform trolley;
    bool readyToLeave = false;
    bool readyForLift = false;
    public NurseAI partner;
    float timer = 0;
    NPCManagerV2 npcManager;
	// Use this for initialization
	void Start ()
    {
        npcManager = GameObject.FindGameObjectWithTag("NPCManager").GetComponent<NPCManagerV2>();
        
    }
	
    void moveToDest()
    {
        agent.SetDestination(dest);
    }

	// Update is called once per frame
	void Update () {

        if (initialized)
        {
            if (targetNPC == null || partner == null)
            {
                npcManager.nursesDeployed = false;
                Destroy(gameObject);
            }
            if (id == 0)
            {
                if(dest == Vector3.zero)
                {
                    dest = targetNPC.transform.position;
                    interaction.setTarget(targetNPC);
                    agent.SetDestination(dest);
                }

                else if(arrivedToDestination(50.0f) && !readyToLeave)
                {
                    agent.Stop();
                    readyForLift = true;
                    if (partner.readyToLeave)
                    {
                        targetNPC.GetComponent<NavMeshAgent>().enabled = false;
                        targetNPC.transform.position = new Vector3(trolley.position.x, 24.0f, trolley.position.z);
                        targetNPC.transform.rotation = Quaternion.Euler(new Vector3(trolley.eulerAngles.x, trolley.eulerAngles.y + 90.0f, trolley.eulerAngles.z));
                   
                        targetNPC.transform.SetParent(trolley);
                        targetNPC.transform.localPosition = new Vector3(targetNPC.transform.localPosition.x -20f, targetNPC.transform.localPosition.y, targetNPC.transform.localPosition.z);
                        readyToLeave = true;
                        NavMeshHit hit;
                        dest = new Vector3(733, 0, -742);
                        NavMesh.SamplePosition(dest, out hit, 50.0f, NavMesh.AllAreas);
                        dest = hit.position;
                        moveToDest();
                        agent.Resume();
                    }
                }
                else if (partner.readyForLift && !arrivedToDestination(50.0f) && !readyToLeave)
                {
                    timer += Time.deltaTime;
                    if (timer > 4.0f)
                    {
                        timer = 0;
                        agent.Warp(agent.destination);
                    }
                }
            }
            else if(id == 1)
            {
                if (dest == Vector3.zero)
                {
                    interaction.setTarget(targetNPC);
                    dest = interaction.getDestToTargetNPCSide(1, 16.0f);
                    moveToDest();
                }
                else if (arrivedToDestination(15.0f))
                {
                    if(interaction.RotateTowards(targetNPC.transform))
                    {
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
                            anim.stoppickfromfloor();
                            dest = new Vector3(733, 0, -742);
                            moveToDest();
                        }
                    }
                }
            }
            if(readyToLeave && arrivedToDestination(30.0f))
            {
                if(id == 0)
                {
                    npcManager.nursesDeployed = false;
                }

                Destroy(gameObject);
            }
            else if(readyToLeave && partner == null)
            {
                timer += Time.deltaTime;
                if(timer > 5.0f)
                {
                    if (id == 0)
                    {
                        npcManager.nursesDeployed = false;
                    }
                    Destroy(gameObject);
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
        anim.setWalkAnimSpeed(150f);
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
