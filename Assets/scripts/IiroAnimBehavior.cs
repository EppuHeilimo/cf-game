using UnityEngine;
using System.Collections;

public class IiroAnimBehavior : MonoBehaviour {

    // Use this for initialization
    NavMeshAgent agent;
    Rigidbody rbody;
    Animator animator;
    float walkspeed = 1f;
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

    public void stopfall()
    {
        falling = false;
        agent.enabled = true;
        animator.SetBool("fall", false);
    }

    public void sleep()
    {
        sleeping = true;
        agent.enabled = false;
        animator.SetBool("sleep", true);
    }

    public void stopSleep()
    {
        sleeping = false;
        agent.enabled = true;
        animator.SetBool("sleep", false);
    }

    public void sit()
    {
        sitting = true;
        agent.enabled = false;
        animator.SetBool("sit", true);
    }

    public void stopSit()
    {
        sitting = false;
        agent.enabled = true;
        animator.SetBool("sit", false);
    }

    public void pickwithboth()
    {
        pickingup = true;
        agent.enabled = false;
        animator.SetBool("pickupbothhands", true);
    }

    public void stoppickwithboth()
    {
        pickingup = false;
        agent.enabled = true;
        animator.SetBool("pickupbothhands", false);
    }

    public void pickfromfloor()
    {
        pickingup = true;
        agent.enabled = false;
        animator.SetBool("pickupfloor", true);
    }

    public void stoppickfromfloor()
    {
        pickingup = false;
        agent.enabled = true;
        animator.SetBool("pickupfloor", false);
    }

    public void sitwithrotation()
    {
        sittingwithrotation = true;
        agent.enabled = false;
        animator.SetBool("sitAndRotate", true);
    }

    public void stopSitwithrotation()
    {
        sittingwithrotation = false;
        agent.enabled = true;
        animator.SetBool("sitAndRotate", false);
    }

    public void pickup()
    {
        pickingup = true;
        agent.enabled = false;
        animator.SetBool("pickup", true);
    }

    public void stopPickup()
    {
        pickingup = false;
        agent.enabled = true;
        animator.SetBool("pickup", false);
    }

    public void setWalkAnimSpeed(float speed)
    {
        //calc % of normal speed
        float temp = (speed / 100.0f) ;
        walkspeed = temp * 1f;
    }
}
