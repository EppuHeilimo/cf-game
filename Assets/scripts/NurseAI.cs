using UnityEngine;
using System.Collections;

public class NurseAI : MonoBehaviour {


    GameObject targetNPC;
    int id;
    NavMeshAgent agent;
    ObjectInteraction interaction;
    IiroAnimBehavior anim;
    Vector3 dest = Vector3.zero;
    bool initialized = false;
    Transform trolley;
    bool readyToLeave = false;
    public NurseAI partner;
    float timer = 0;
	// Use this for initialization
	void Start ()
    {

	}
	
    void moveToDest()
    {
        agent.SetDestination(dest);
    }

	// Update is called once per frame
	void Update () {

	    if(initialized)
        {
            if (id == 0)
            {
                if(dest == Vector3.zero)
                {
                    interaction.setTarget(targetNPC);
                    dest = interaction.getDestToTargetNPCSide(3, 32.0f);
                    moveToDest();
                }
                else if(arrivedToDestination(10.0f) && !readyToLeave)
                {
                    agent.Stop();
                    if(partner.readyToLeave)
                    {
                        targetNPC.GetComponent<NavMeshAgent>().enabled = false;
                        targetNPC.transform.position = new Vector3(trolley.position.x, 24.0f, trolley.position.z);
                        targetNPC.transform.rotation = Quaternion.Euler(new Vector3(trolley.eulerAngles.x, trolley.eulerAngles.y + 90.0f, trolley.eulerAngles.z));
                   
                        targetNPC.transform.SetParent(trolley);
                        targetNPC.transform.localPosition = new Vector3(targetNPC.transform.localPosition.x -20f, targetNPC.transform.localPosition.y, targetNPC.transform.localPosition.z);
                        readyToLeave = true;
                        dest = new Vector3(733, 0, -742);
                        moveToDest();
                        agent.Resume();
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
                else if (arrivedToDestination(5.0f))
                {
                    if(interaction.RotateTowards(targetNPC.transform) && !anim.pickingup && !readyToLeave)
                    {
                        anim.pickfromfloor();
                    }
                    else if(anim.pickingup)
                    {
                        timer += Time.deltaTime;
                    }
                    if (timer > 1.0f)
                    {
                        readyToLeave = true;
                        anim.stoppickfromfloor();
                        dest = new Vector3(733, 0, -742);
                        moveToDest();
                    }
                }
            }
            if(readyToLeave && arrivedToDestination(30.0f))
            {
                Destroy(gameObject);
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
