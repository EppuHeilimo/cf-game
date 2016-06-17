using UnityEngine;
using System.Collections;

public class IiroAnimBehavior : MonoBehaviour {

    // Use this for initialization
    NavMeshAgent agent;
    Rigidbody rbody;
    public bool isWalking = false;
    public bool goToSleep = false;
    public bool sit = false;
    public bool sitwithrotation = false;
    public bool fall = false;
    Animator animator;
    float walkspeed = 1f;
    void Start () {
        agent = GetComponent<NavMeshAgent>();
        rbody = GetComponent<Rigidbody>();
        animator = transform.FindChild("Iiro").GetComponent<Animator>();
    }   
	
	// Update is called once per frame
	void Update () {

        if(goToSleep || sit || sitwithrotation)
        {
            agent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
        }
        else
        {
            agent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
        }
        animator.SetBool("sleep", goToSleep);
        animator.SetBool("IsWalking", isWalking);
        animator.SetBool("sit", sit);
        animator.SetBool("fall", fall);
        animator.SetBool("sitAndRotate", sitwithrotation);
        if (agent.velocity.magnitude < 10.0f)
        {
            isWalking = false;
            animator.speed = 1f;
        }
        else
        {
            isWalking = true;
            animator.speed = walkspeed;
        }
    }

    public void setWalkAnimSpeed(float speed)
    {
        //calc % of normal speed
        float temp = (speed / 100.0f) ;
        walkspeed = temp * 1f;
    }
}
