using UnityEngine;
using System.Collections;

/* Handles Iiro model's animations */

public class IiroAnimBehavior : MonoBehaviour {

    // Use this for initialization
    NavMeshAgent agent;
    Rigidbody rbody;
    Animator animator;
    float walkspeed = 1f;
    public bool waitforanim = false;
    float animWaitTime = 0f;
    public float timer = 0;
    public bool isWalking = false;
    public bool sleeping = false;
    public bool sitting = false;
    public bool sittingwithrotation = false;
    public bool falling = false;
    public bool pickingup = false;
    void Start () {
        agent = GetComponent<NavMeshAgent>();
        rbody = GetComponent<Rigidbody>();
        animator = transform.FindChild("Iiro").GetComponent<Animator>();
    }   
	
	// Update is called once per frame
	void Update ()
    {
        if (agent.velocity.magnitude < 10.0f)
        {
            stopWalking();
        }
        else
        {
            walk();
        }
        if(waitforanim)
        {
            timer += Time.deltaTime;
            if(timer > animWaitTime)
            {
                stopWaitForAnim();
            }
        }
    }

    public void stopWaitForAnim()
    {
        waitforanim = false;
        agent.enabled = true;
        agent.Resume();
        timer = 0;
    }

    public void StopAll()
    {
        
        if(sleeping)
        {
            animator.SetBool("sleep", false);
            animWaitTime = 1.5f;
            waitforanim = true;
            sleeping = false;
        }
        if(sitting)
        {
            waitforanim = true;
            animWaitTime = 0.8125f;
            animator.SetBool("sit", false);
            sitting = false;
        }
        if(sittingwithrotation)
        {
            waitforanim = true;
            animWaitTime = 1.5f;
            animator.SetBool("sitAndRotate", false);
            sittingwithrotation = false;
        }           
        animator.SetBool("fall", false);        
        animator.SetBool("pickupbothhands", false);
        animator.SetBool("pickupfloor", false);       
        animator.SetBool("pickup", false);   
        falling = false;
        pickingup = false;

        if(!waitforanim)
        {
            agent.enabled = true;
            agent.Resume();
        }        

    }

    public void walk()
    {
        isWalking = true;
        animator.SetBool("IsWalking", true);
        animator.speed = walkspeed;
    }

    public void stopWalking()
    {
        isWalking = false;
        animator.SetBool("IsWalking", false);
        animator.speed = 1f;
    }

    public void fall()
    {
        falling = true;
        agent.enabled = false;
        animator.SetBool("fall", true);
    }


    public void sleep()
    {
        sleeping = true;
        agent.enabled = false;
        animator.SetBool("sleep", true);
    }


    public void sit()
    {
        sitting = true;
        agent.enabled = false;
        animator.SetBool("sit", true);
    }


    public void pickwithboth()
    {
        pickingup = true;
        agent.enabled = false;
        animator.SetBool("pickupbothhands", true);
    }


    public void pickfromfloor()
    {
        pickingup = true;
        agent.enabled = false;
        animator.SetBool("pickupfloor", true);
    }


    public void sitwithrotation()
    {
        sittingwithrotation = true;
        agent.enabled = false;
        animator.SetBool("sitAndRotate", true);
    }


    public void pickup()
    {
        pickingup = true;
        agent.enabled = false;
        animator.SetBool("pickup", true);
    }


    public void setWalkAnimSpeed(float speed)
    {
        //calc % of normal speed
        float temp = (speed / 100.0f) ;
        walkspeed = temp * 1f;
    }
}
